# A Practical Guide to EF Core Entity Configurations

This guide explains how to write Entity Framework Core configurations yourself. It focuses on the reasoning that applies to any entity rather than prescribing the final configuration for a particular domain model.

The examples use EF Core 8 and SQL Server. In this repository, entity classes live in `Eldoria.Core/Entities`, configurations live in `Eldoria.Infrastructure/Db/Configurations`, and `ApplicationDbContext` discovers configurations automatically.

## 1. What an EF configuration is responsible for

An entity class describes an object in C#. Its EF configuration describes how that object is represented and protected in the database.

A configuration normally defines:

- The table and primary key
- Required and optional columns
- String lengths, numeric precision, defaults, and conversions
- Relationships and foreign keys
- Delete behavior
- Unique and non-unique indexes
- Check constraints
- Concurrency behavior

The basic structure is:

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations;

public sealed class ExampleEntityConfig
    : IEntityTypeConfiguration<ExampleEntity>
{
    public void Configure(EntityTypeBuilder<ExampleEntity> builder)
    {
        builder.HasKey(x => x.Id);
    }
}
```

The type passed to `IEntityTypeConfiguration<T>` is the entity being configured. Everything inside `Configure` applies to that entity.

## 2. How configurations are discovered

`ApplicationDbContext` contains:

```csharp
modelBuilder.ApplyConfigurationsFromAssembly(
    typeof(ApplicationDbContext).Assembly);
```

EF scans the Infrastructure assembly for implementations of `IEntityTypeConfiguration<T>`. A new configuration does not need to be registered individually as long as:

- It is compiled into `Eldoria.Infrastructure`.
- It implements `IEntityTypeConfiguration<T>`.
- Its `Configure` method is valid.

A `DbSet<T>` is useful for querying an entity directly, but it is not what registers a configuration. EF can discover entity types through `DbSet` properties, relationships, and configurations.

## 3. Start by reading the entity

Before writing configuration code, classify every property:

```csharp
public class ExampleChild
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Notes { get; set; }

    public int ParentId { get; set; }
    public ExampleParent Parent { get; set; } = null!;
}
```

This tells you:

- `Id` is probably the primary key.
- `Name` is a required string.
- `Notes` is an optional string.
- `ParentId` is a required foreign key because it is an `int`, not an `int?`.
- `Parent` is the reference navigation associated with `ParentId`.
- `ExampleChild` is the dependent because it contains the foreign key.

Do not begin with fluent API syntax. First identify the database rule you are trying to express.

## 4. Primary keys

Most entities in this repository use an integer `Id`:

```csharp
builder.HasKey(x => x.Id);
```

SQL Server normally makes an integer primary key an identity column automatically.

### Composite keys

A composite key uses multiple properties:

```csharp
builder.HasKey(x => new { x.ParentId, x.ChildId });
```

Composite keys are common for simple join entities. If the entity already has its own `Id`, keep that as the primary key and use a unique index to prevent duplicate pairs:

```csharp
builder.HasIndex(x => new { x.ParentId, x.ChildId })
    .IsUnique();
```

Choose one approach intentionally. A surrogate `Id` is convenient when other rows may reference the relationship itself. A composite key directly represents the relationship's identity.

## 5. Configuring scalar properties

### Required and optional properties

```csharp
builder.Property(x => x.Name)
    .IsRequired();

builder.Property(x => x.Notes)
    .IsRequired(false);
```

With nullable reference types enabled, EF usually infers requiredness:

- `string` is required.
- `string?` is optional.
- `int` is required.
- `int?` is optional.

It is still helpful to configure important required properties explicitly. The entity's nullability and the configuration must agree.

`= string.Empty` and `= null!` only satisfy C# initialization rules. They do not create database defaults and do not change column nullability.

### String lengths

```csharp
builder.Property(x => x.Name)
    .IsRequired()
    .HasMaxLength(250);
```

Choose a length based on an actual product rule:

- Short labels and names should have a reasonable limit.
- URLs commonly need a larger limit, such as 2048.
- File names commonly use 250 or 255.
- Long narrative text may intentionally use `nvarchar(max)`.
- JSON commonly uses `nvarchar(max)`.

Do not add arbitrary limits merely because another entity uses them. The API and UI validation should use the same limits as the database.

### Numeric precision

Use precision for decimal values:

```csharp
builder.Property(x => x.Price)
    .HasPrecision(18, 2);
```

The first number is the total number of digits. The second is the number after the decimal point.

### Defaults

```csharp
builder.Property(x => x.IsActive)
    .HasDefaultValue(true);

builder.Property(x => x.CreatedAt)
    .HasDefaultValueSql("SYSUTCDATETIME()");
```

Use a database default only if inserts that omit the column should receive that value. An initializer such as `IsActive = true` applies only when C# constructs the entity.

Avoid giving a property competing defaults in C# and SQL unless they intentionally mean the same thing.

### Enums

Enums are stored as integers by convention:

```csharp
builder.Property(x => x.Status)
    .IsRequired();
```

They can be stored as names:

```csharp
builder.Property(x => x.Status)
    .HasConversion<string>()
    .HasMaxLength(50);
```

Integer storage is compact and is the existing repository convention. String storage is easier to read in SQL but makes enum renames a data concern. Pick one convention and use it consistently.

### Date and time values

SQL Server does not preserve a `DateTime` time-zone identity by itself. Establish an application rule—normally UTC—and follow it consistently.

Configuration expresses whether a date is required:

```csharp
builder.Property(x => x.StartedAt)
    .IsRequired();
```

It does not automatically convert local time to UTC.

### JSON and large text

```csharp
builder.Property(x => x.Config)
    .IsRequired()
    .HasColumnType("nvarchar(max)");
```

EF and SQL Server will treat this as text unless you add custom serialization or mapping. Validate the JSON in application code if malformed data would be harmful.

## 6. Relationship vocabulary

- **Principal:** the row being referenced.
- **Dependent:** the row containing the foreign key.
- **Primary key:** uniquely identifies a row.
- **Foreign key:** contains the key of a related row.
- **Reference navigation:** points to one related object.
- **Collection navigation:** points to many related objects.
- **Required relationship:** the foreign key cannot be null.
- **Optional relationship:** the foreign key can be null.
- **Cardinality:** one-to-one, one-to-many, or many-to-many.

The most important question is:

> Which entity owns the foreign key?

That entity is the dependent.

## 7. One-to-many relationships

Example entities:

```csharp
public class Parent
{
    public int Id { get; set; }
    public ICollection<Child> Children { get; set; } = [];
}

public class Child
{
    public int Id { get; set; }
    public int ParentId { get; set; }
    public Parent Parent { get; set; } = null!;
}
```

Configuration:

```csharp
builder.HasOne(x => x.Parent)
    .WithMany(x => x.Children)
    .HasForeignKey(x => x.ParentId)
    .OnDelete(DeleteBehavior.Cascade);
```

Read it as a sentence:

> A child has one parent. A parent has many children. `ParentId` is the child's foreign key.

Normally configure this once in the dependent's configuration.

### No inverse navigation

If the parent has no collection navigation:

```csharp
builder.HasOne(x => x.Parent)
    .WithMany()
    .HasForeignKey(x => x.ParentId);
```

An omitted inverse navigation does not change the database cardinality.

### Optional relationship

Make both the FK and navigation nullable:

```csharp
public int? ParentId { get; set; }
public Parent? Parent { get; set; }
```

The fluent relationship syntax stays the same.

## 8. One-to-one relationships

In a one-to-one relationship, EF must know which entity contains the FK:

```csharp
builder.HasOne(x => x.Settings)
    .WithOne(x => x.Owner)
    .HasForeignKey<Settings>(x => x.OwnerId)
    .OnDelete(DeleteBehavior.Cascade);
```

`HasForeignKey<Settings>` identifies `Settings` as the dependent. EF creates a unique index on `OwnerId`, preventing multiple settings rows for one owner.

Without a reverse navigation:

```csharp
builder.HasOne(x => x.SelectedItem)
    .WithOne()
    .HasForeignKey<Owner>(x => x.SelectedItemId);
```

Use one-to-one only when uniqueness is a real rule. If many rows may point to the same referenced row, use `WithMany()` even when there is no collection navigation.

## 9. Many-to-many relationships

### Explicit join entity

Use an explicit join entity when the relationship has its own properties or identity:

```csharp
public class CharacterSpell
{
    public int Id { get; set; }

    public int CharacterId { get; set; }
    public Character Character { get; set; } = null!;

    public int SpellId { get; set; }
    public Spell Spell { get; set; } = null!;

    public DateTime LearnedAt { get; set; }
}
```

Configure it as two one-to-many relationships:

```csharp
builder.HasOne(x => x.Character)
    .WithMany(x => x.Spells)
    .HasForeignKey(x => x.CharacterId)
    .OnDelete(DeleteBehavior.Cascade);

builder.HasOne(x => x.Spell)
    .WithMany(x => x.Characters)
    .HasForeignKey(x => x.SpellId)
    .OnDelete(DeleteBehavior.NoAction);

builder.HasIndex(x => new { x.CharacterId, x.SpellId })
    .IsUnique();
```

Do not add pair uniqueness if duplicate rows represent separate instances. Inventory commonly allows multiple copies of the same item, while a learned-spell assignment normally does not allow duplicates.

### Skip-navigation many-to-many

When the relationship has no data of its own:

```csharp
builder.HasMany(x => x.Spells)
    .WithMany(x => x.Items)
    .UsingEntity(join => join.ToTable("ItemSpells"));
```

EF creates a hidden join table. Use an explicit entity instead if the relationship may need source IDs, ordering, timestamps, state, or direct repository access.

## 10. Self-referencing relationships

An entity can reference another row of its own type:

```csharp
builder.HasOne(x => x.AlternateForm)
    .WithMany()
    .HasForeignKey(x => x.AlternateFormId)
    .OnDelete(DeleteBehavior.NoAction);
```

This means many rows may reference the same alternate form. Use `WithOne()` only if the referenced row may be used by at most one owner.

Self-references should rarely cascade because they can create cycles or unexpectedly delete a large graph.

## 11. Multiple relationships between the same entity types

EF needs explicit help when two types are connected in more than one way. For example, a container may own many entries while also pointing to the selected entry:

```csharp
builder.HasOne(x => x.Container)
    .WithMany(x => x.Entries)
    .HasForeignKey(x => x.ContainerId)
    .OnDelete(DeleteBehavior.Cascade);
```

The selected-entry relationship is separate:

```csharp
builder.HasOne(x => x.SelectedEntry)
    .WithOne()
    .HasForeignKey<Container>(x => x.SelectedEntryId)
    .OnDelete(DeleteBehavior.NoAction);
```

Configure every navigation and FK explicitly in cases like this. Otherwise EF may create a shadow FK such as `ContainerId1`.

## 12. Delete behavior

Choose delete behavior based on ownership and lifetime, not convenience.

### Cascade

```csharp
.OnDelete(DeleteBehavior.Cascade)
```

Use when the dependent has no meaning without its parent. Deleting the parent deletes the dependents in the database.

Typical examples are owned pages, log entries, join rows, and runtime child state.

### NoAction

```csharp
.OnDelete(DeleteBehavior.NoAction)
```

The database does not cascade or clear the FK. It rejects a delete if a surviving dependent would reference a missing principal.

Use it for shared references, cross-references, and relationships that would create cascade cycles.

### Restrict

```csharp
.OnDelete(DeleteBehavior.Restrict)
```

This also prevents deleting a principal while dependents exist. SQL Server behavior is similar to `NoAction`; use the repository's convention consistently.

### SetNull

```csharp
.OnDelete(DeleteBehavior.SetNull)
```

The database sets the FK to `NULL` when the principal is deleted. The FK property must be nullable, and the dependent must remain valid without the relationship.

### ClientSetNull and ClientCascade

These behaviors depend on entities being loaded and tracked by EF. They are less reliable for database-side operations or unloaded graphs. Use them only when their tracked-entity semantics are deliberate.

### A delete-behavior decision test

For each relationship, ask:

1. Is the dependent wholly owned by the principal?
2. Should it survive if the principal is removed?
3. Is the principal shared by several parts of the model?
4. Is there another cascade route to the same table?
5. Does this relationship point back into an owned graph?

Use cascade only when the answer clearly follows ownership downward. Use `NoAction` for sideways and backward references.

SQL Server rejects some schemas with cycles or multiple cascade paths. The solution is usually to keep cascades on ownership edges and use `NoAction` on cross-references.

## 13. Indexes

Indexes either enforce a rule or improve a query.

### Unique index

```csharp
builder.HasIndex(x => x.Email)
    .IsUnique();
```

Composite uniqueness:

```csharp
builder.HasIndex(x => new { x.ParentId, x.SortOrder })
    .IsUnique();
```

This guarantees that one parent cannot have two children at the same position.

### Filtered unique index

Use a filter when uniqueness applies only to some rows:

```csharp
builder.HasIndex(x => new { x.OwnerId, x.ResourceId })
    .IsUnique()
    .HasFilter("[CompletedAt] IS NULL");
```

The SQL filter uses database column names, not C# expressions.

Filtered indexes are especially useful for optional FKs and “only one active record” rules.

### Non-unique index

```csharp
builder.HasIndex(x => new { x.ParentId, x.CreatedAt });
```

Add one when common queries filter or order by those columns. Foreign keys normally receive indexes by convention, so inspect the generated migration before adding duplicates.

### Questions before adding uniqueness

- Is the rule globally unique or only unique within a parent?
- Can two legitimate rows have the same value?
- Are duplicates separate instances or accidental assignments?
- Does the rule apply to completed/inactive rows?
- Does a nullable value require a filtered index?

## 14. Check constraints

A check constraint protects a rule involving columns in the same row:

```csharp
builder.ToTable(table =>
{
    table.HasCheckConstraint(
        "CK_Example_Range",
        "[Minimum] >= 1 AND [Maximum] >= [Minimum]");
});
```

Exactly one of two optional columns:

```csharp
builder.ToTable(table =>
{
    table.HasCheckConstraint(
        "CK_Example_ExactlyOneTarget",
        "([FirstTargetId] IS NOT NULL AND [SecondTargetId] IS NULL) OR " +
        "([FirstTargetId] IS NULL AND [SecondTargetId] IS NOT NULL)");
});
```

Good uses include:

- Positive quantities
- Minimum/maximum ranges
- Nonnegative capacities
- Exactly one of two optional targets
- Valid combinations of fields in one row

A normal SQL Server check constraint cannot query another table. Rules involving a parent row, another child row, or an entire collection require service validation or a different key design.

Give every constraint a stable, descriptive, unique name.

## 15. Same-owner and same-root integrity

A foreign key proves that a referenced row exists. It may not prove that both rows belong to the same owner, aggregate, or playthrough.

For example, a character-spell FK can point to an existing spell from the wrong root unless the schema or application prevents it.

There are two main approaches.

### Application validation

Before saving the relationship, query both rows and verify their owner/root IDs match. This keeps the schema simple but requires every write path to use trusted validation.

### Composite foreign key

Repeat the scope ID on the dependent and reference a composite principal key:

```csharp
principal.HasAlternateKey(x => new { x.Id, x.RootId });

dependent.HasOne(x => x.Reference)
    .WithMany()
    .HasForeignKey(x => new { x.ReferenceId, x.RootId })
    .HasPrincipalKey(x => new { x.Id, x.RootId });
```

This gives stronger database protection but makes the schema and entities more verbose. The dependent must contain all composite FK properties.

Do not assume ordinary single-column FKs enforce a common root.

## 16. Alternate keys

An alternate key is a candidate key that another FK may reference:

```csharp
builder.HasAlternateKey(x => new { x.Id, x.RootId });
```

Use an alternate key when another relationship must target those columns. If you only need uniqueness and nothing references the columns as a key, use a unique index instead.

## 17. Concurrency

Concurrency protection prevents one update from silently overwriting another.

SQL Server row-version example:

```csharp
builder.Property(x => x.RowVersion)
    .IsRowVersion();
```

The entity needs a property such as:

```csharp
public byte[] RowVersion { get; set; } = [];
```

EF includes the original row version in updates and throws `DbUpdateConcurrencyException` if another writer changed the row first.

Use concurrency tokens for genuinely concurrent mutable records. Adding them requires application code to handle conflicts.

## 18. Value conversions

Value conversions change how a property is stored:

```csharp
builder.Property(x => x.Status)
    .HasConversion<string>();
```

They can also support custom value objects. A conversion is not validation and does not automatically make a complex object queryable. Use it only when the persisted representation is stable and understood.

## 19. Owned types versus normal entities

An owned type is part of its owner's identity and cannot exist independently:

```csharp
builder.OwnsOne(x => x.Address, owned =>
{
    owned.Property(x => x.City)
        .HasMaxLength(100);
});
```

Owned types are useful for true value objects. Do not use them merely to avoid writing a configuration for a child that has its own identity, lifecycle, repository access, or relationships.

## 20. Query filters

A global query filter automatically limits queries:

```csharp
builder.HasQueryFilter(x => !x.IsDeleted);
```

This is useful for soft deletion or tenant isolation but affects every normal query, including navigation loading. Callers can bypass it with `IgnoreQueryFilters()`.

Treat filters as query behavior, not security by themselves. Ownership must still be enforced deliberately.

## 21. Table and column names

EF conventions can choose table and column names. Configure them explicitly only when there is a reason:

```csharp
builder.ToTable("ExampleEntities");

builder.Property(x => x.Name)
    .HasColumnName("DisplayName");
```

Explicit names help when preserving an existing schema. Unnecessary renaming adds noise and can cause migrations to interpret a rename as drop-and-add if history is unclear.

## 22. A reusable configuration template

Use this as a worksheet, deleting sections that do not apply:

```csharp
public sealed class ExampleConfig : IEntityTypeConfiguration<Example>
{
    public void Configure(EntityTypeBuilder<Example> builder)
    {
        // 1. Table and key
        builder.HasKey(x => x.Id);

        // 2. Scalar properties
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(250);

        // 3. Indexes
        builder.HasIndex(x => new { x.ParentId, x.Name })
            .IsUnique();

        // 4. Relationships owned by this dependent
        builder.HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Cascade);

        // 5. Check constraints
        builder.ToTable(table =>
        {
            table.HasCheckConstraint(
                "CK_Examples_Quantity",
                "[Quantity] >= 0");
        });
    }
}
```

The order is a readability convention, not an EF requirement.

## 23. A repeatable process for every entity

### Step 1: inventory the properties

List:

- Primary key
- Ordinary scalar properties
- Foreign key properties
- Reference navigations
- Collection navigations
- Nullable properties
- Runtime defaults

### Step 2: write each relationship in plain English

For every navigation, state:

> A ___ has one/many ___; the foreign key is ___ on the ___ entity.

If that sentence is unclear, do not write the fluent API yet.

### Step 3: decide ownership and deletion

Ask whether the dependent can exist without the principal. Map true ownership downward with cascade. Map shared or cross references with `NoAction`, `Restrict`, or a nullable relationship as appropriate.

### Step 4: identify database invariants

List what must never be duplicated and what values must remain in valid ranges. Decide whether each rule belongs in:

- A unique index
- A check constraint
- A foreign key
- Application validation
- Both database and application validation

### Step 5: configure properties

Apply intentional string limits, precision, defaults, conversions, and requiredness.

### Step 6: write the configuration once

Keep each relationship in one place, normally the dependent configuration.

### Step 7: review the generated model and migration

Look for shadow FKs, unexpected nullability, missing uniqueness, and cascade paths.

## 24. Migration workflow

From the repository root:

```powershell
dotnet build Eldoria.sln

$Infrastructure = "Eldoria.Infrastructure/Eldoria.Infrastructure.csproj"
$Startup = "Eldoria.Api/Eldoria.Api.csproj"
$Context = "ApplicationDbContext"

dotnet ef migrations add DescribeTheSchemaChange `
    --project $Infrastructure `
    --startup-project $Startup `
    --context $Context
```

Do not apply the migration immediately. Read its `Up`, `Down`, and model snapshot first.

Check that:

- Expected tables and columns were added or changed.
- Required and optional columns are correct.
- Foreign keys point in the intended direction.
- Delete actions are correct.
- Unique and filtered indexes exist.
- Check constraints contain the intended SQL.
- One-to-one FKs are unique.
- No shadow columns such as `ParentId1` appeared.
- Renames were not interpreted as destructive drop-and-add operations.
- Obsolete entity types were not rediscovered through stale `DbSet` properties or navigations.

Only then apply it:

```powershell
dotnet ef database update `
    --project $Infrastructure `
    --startup-project $Startup `
    --context $Context
```

Do not rewrite a migration that has been applied to a shared or deployed database. Create a corrective migration instead.

## 25. Common mistakes and what they mean

### Unexpected `SomethingId1` column

EF believes there are two different relationships. Pair both navigations explicitly and specify the FK.

### “Unable to determine the dependent side”

The one-to-one relationship does not identify its FK owner. Use `HasForeignKey<TDependent>`.

### Multiple cascade-path error

Two cascade routes reach the same table, or a relationship points back into its owning graph. Keep cascade on the ownership path and change the cross-reference to `NoAction`.

### Required relationship produces null errors

The C# nullability, configuration, or incoming data disagrees. Decide whether the relationship is truly required and make all layers consistent.

### Duplicate-key error on legitimate rows

A unique index encodes the wrong business rule. Decide whether the rows are assignments, which should often be unique, or instances, which may legitimately repeat.

### Delete fails with a foreign-key violation

`NoAction` or `Restrict` is protecting a reference. Remove or reassign dependents first, or reconsider whether the relationship represents ownership.

### Configuration exists but is ignored

Check the generic entity type, assembly, namespace imports, and `ApplyConfigurationsFromAssembly`. Also confirm the configuration is compiled into the project.

### Migration tries to recreate an obsolete table

A stale `DbSet`, navigation, configuration, or entity reference is still causing EF to discover the type.

### The schema allows references across owners

A basic FK checks existence, not common ownership. Add service validation or a composite FK.

## 26. Final checklist

For every configuration, confirm:

- [ ] The primary key is explicit.
- [ ] Requiredness matches C# nullability and the business rule.
- [ ] String lengths and numeric precision are intentional.
- [ ] Database defaults are actually needed.
- [ ] Each relationship is configured exactly once.
- [ ] The dependent and FK owner are known.
- [ ] One-to-one relationships identify the dependent type.
- [ ] Delete behavior follows ownership and avoids cascade cycles.
- [ ] Unique indexes represent real invariants.
- [ ] Repeatable instances are not accidentally unique.
- [ ] Common queries have appropriate indexes.
- [ ] Same-row rules use check constraints where useful.
- [ ] Cross-row and same-owner rules are validated elsewhere when ordinary FKs cannot enforce them.
- [ ] The generated migration has no unexpected shadow columns or destructive operations.
- [ ] `ApplicationDbContext` contains no obsolete `DbSet` types.

If you can explain the principal, dependent, foreign key, cardinality, delete behavior, uniqueness rule, and validation boundary for every relationship, you are ready to write and review its EF configuration.
