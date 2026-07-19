# EF Core Entities, Configuration, and Relationships

This guide shows how to define common EF Core relationships using entity classes in `Eldoria.Core/Entities` and configuration classes in `Eldoria.Infrastructure/Db/Configurations`.

## Relationship vocabulary

- **Principal:** the parent or referenced entity.
- **Dependent:** the entity containing the foreign key.
- **Primary key:** uniquely identifies an entity, commonly `Id`.
- **Foreign key:** stores the primary key of a related entity, such as `CharacterId`.
- **Reference navigation:** points to one entity, such as `Spell Spell`.
- **Collection navigation:** points to many entities, such as `ICollection<Spell>`.
- **Required relationship:** the foreign key is non-nullable, such as `int CharacterId`.
- **Optional relationship:** the foreign key is nullable, such as `int? CharacterId`.

The physical folders containing the classes do not determine relationships. EF Core uses the entity types, keys, navigation properties, foreign keys, and configuration.

## Basic entity

An entity is normally a public class with a primary key:

```csharp
namespace Eldoria.Core.Entities;

public class SpellType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

Its configuration implements `IEntityTypeConfiguration<TEntity>`:

```csharp
using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eldoria.Infrastructure.Db.Configurations;

public sealed class SpellTypeConfig : IEntityTypeConfiguration<SpellType>
{
    public void Configure(EntityTypeBuilder<SpellType> builder)
    {
        builder.ToTable("SpellTypes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => x.Name);
    }
}
```

Useful property configuration methods include:

```csharp
builder.Property(x => x.Name).IsRequired();
builder.Property(x => x.Name).HasMaxLength(100);
builder.Property(x => x.Price).HasPrecision(18, 2);
builder.Property(x => x.IsActive).HasDefaultValue(true);
builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(30);
builder.HasIndex(x => x.Name).IsUnique();
```

`ApplicationDbContext` already discovers configuration classes with:

```csharp
modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
```

Consequently, a configuration does not need to be registered individually as long as it is in the same assembly and implements `IEntityTypeConfiguration<T>`.

## One-to-many and many-to-one

These are the same relationship viewed from opposite directions:

- One `SpellType` has many `Spell` records.
- Each `Spell` belongs to one `SpellType`.

### Entities

```csharp
public class SpellType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Spell> Spells { get; set; } = [];
}

public class Spell
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Foreign key on the dependent entity
    public int SpellTypeId { get; set; }

    // Reference navigation to the principal entity
    public SpellType SpellType { get; set; } = null!;
}
```

### Configuration

```csharp
public sealed class SpellConfig : IEntityTypeConfiguration<Spell>
{
    public void Configure(EntityTypeBuilder<Spell> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.SpellType)
            .WithMany(x => x.Spells)
            .HasForeignKey(x => x.SpellTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

Read the configuration as: “A spell has one spell type; a spell type has many spells; `SpellTypeId` is the foreign key.”

The foreign key should usually be configured in the dependent entity's configuration file—in this example, `SpellConfig`.

### Optional many-to-one

Make the foreign key and navigation nullable:

```csharp
public int? SpellTypeId { get; set; }
public SpellType? SpellType { get; set; }
```

The relationship configuration remains the same. The nullable foreign key tells EF Core the relationship is optional.

## One-to-one

Example: one character has one set of dialog settings, and each dialog-settings record belongs to one character.

### Entities

```csharp
public class Character
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public CharacterDialogSettings? CharacterDialogSettings { get; set; }
}

public class CharacterDialogSettings
{
    public int Id { get; set; }
    public string DialogActiveColor { get; set; } = string.Empty;

    // The dependent contains the foreign key.
    public int CharacterId { get; set; }
    public Character Character { get; set; } = null!;
}
```

### Configuration

```csharp
builder.HasOne(x => x.CharacterDialogSettings)
    .WithOne(x => x.Character)
    .HasForeignKey<CharacterDialogSettings>(x => x.CharacterId)
    .OnDelete(DeleteBehavior.Cascade);
```

`HasForeignKey<CharacterDialogSettings>` identifies `CharacterDialogSettings` as the dependent. EF Core creates a unique constraint on `CharacterId`, ensuring that a character cannot have multiple settings records.

For an optional one-to-one relationship, use `int? CharacterId` and `Character? Character` on the dependent.

## Many-to-many with an explicit join entity

Use an explicit join entity when the relationship needs its own ID, properties, dates, ordering, quantities, soft deletion, or behavior. Eldoria's `CharacterSpell` pattern is an example.

- A character can know many spells.
- A spell can belong to many characters.
- `CharacterSpell` connects them.

### Entities

```csharp
public class Character
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<CharacterSpell> CharacterSpells { get; set; } = [];
}

public class Spell
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<CharacterSpell> CharacterSpells { get; set; } = [];
}

public class CharacterSpell
{
    public int Id { get; set; }

    public int CharacterId { get; set; }
    public Character Character { get; set; } = null!;

    public int SpellId { get; set; }
    public Spell Spell { get; set; } = null!;

    // Example relationship-specific data
    public DateTime LearnedAt { get; set; }
}
```

### Configuration

```csharp
public sealed class CharacterSpellConfig
    : IEntityTypeConfiguration<CharacterSpell>
{
    public void Configure(EntityTypeBuilder<CharacterSpell> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Character)
            .WithMany(x => x.CharacterSpells)
            .HasForeignKey(x => x.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Spell)
            .WithMany(x => x.CharacterSpells)
            .HasForeignKey(x => x.SpellId)
            .OnDelete(DeleteBehavior.Cascade);

        // Prevent the same spell from being assigned twice.
        builder.HasIndex(x => new { x.CharacterId, x.SpellId })
            .IsUnique();
    }
}
```

Each side of this many-to-many relationship is configured as a one-to-many relationship with the join entity.

You can alternatively use a composite primary key instead of `Id`:

```csharp
builder.HasKey(x => new { x.CharacterId, x.SpellId });
```

Use either a surrogate `Id` plus a unique index or a composite primary key according to how the join record will be referenced.

## Many-to-many without a join class

EF Core can create a hidden join table when the relationship has no additional data.

### Entities

```csharp
public class Character
{
    public int Id { get; set; }
    public ICollection<Tag> Tags { get; set; } = [];
}

public class Tag
{
    public int Id { get; set; }
    public ICollection<Character> Characters { get; set; } = [];
}
```

### Configuration

```csharp
builder.HasMany(x => x.Tags)
    .WithMany(x => x.Characters)
    .UsingEntity(join => join.ToTable("CharacterTags"));
```

This is concise, but an explicit join entity is usually better when the relationship may later need additional fields.

## Self-referencing relationships

An entity can reference another row of the same entity type. For example, a character can have an alternate form.

### One optional alternate form

```csharp
public class Character
{
    public int Id { get; set; }

    public int? BaseAlternateFormId { get; set; }
    public Character? BaseAlternateForm { get; set; }
}
```

```csharp
builder.HasOne(x => x.BaseAlternateForm)
    .WithOne()
    .HasForeignKey<Character>(x => x.BaseAlternateFormId)
    .OnDelete(DeleteBehavior.NoAction);
```

### Parent with many children

```csharp
public class Category
{
    public int Id { get; set; }

    public int? ParentId { get; set; }
    public Category? Parent { get; set; }
    public ICollection<Category> Children { get; set; } = [];
}
```

```csharp
builder.HasOne(x => x.Parent)
    .WithMany(x => x.Children)
    .HasForeignKey(x => x.ParentId)
    .OnDelete(DeleteBehavior.Restrict);
```

Be especially careful with cascade deletes on self-referencing relationships because they can create cycles or delete large object graphs.

## Relationships without navigation properties

A navigation is not mandatory. Use a parameterless `WithOne()` or `WithMany()` when the other entity does not expose a navigation:

```csharp
builder.HasOne(x => x.Owner)
    .WithMany()
    .HasForeignKey(x => x.OwnerId)
    .OnDelete(DeleteBehavior.Restrict);
```

Navigation properties are normally helpful, but omitting an unused reverse navigation can keep the model simpler.

## Delete behaviors

Choose delete behavior deliberately:

- `Cascade`: deleting the principal automatically deletes dependents.
- `Restrict`: prevents deletion while dependent rows exist.
- `NoAction`: lets the database enforce the foreign key without an EF-generated cascading action.
- `SetNull`: sets a nullable foreign key to `NULL` when the principal is deleted.
- `ClientSetNull`: EF changes tracked dependent foreign keys to null, but does not configure a database cascade.

Examples:

```csharp
// Owned child data should disappear with the parent.
.OnDelete(DeleteBehavior.Cascade);

// Shared catalog data must not be deleted while in use.
.OnDelete(DeleteBehavior.Restrict);

// Optional relationship survives deletion of the principal.
.OnDelete(DeleteBehavior.SetNull);
```

Avoid placing `Cascade` on every relationship. SQL Server can reject schemas containing multiple cascade paths, and deleting one record could remove more data than intended.

## Loading related entities

Configuring a relationship does not mean EF Core automatically loads it. Use `Include` for eager loading:

```csharp
var characters = await db.Characters
    .AsNoTracking()
    .Include(x => x.CharacterSpells)
        .ThenInclude(x => x.Spell)
    .ToListAsync(ct);
```

For APIs, projecting directly to DTOs is often more efficient:

```csharp
var characters = await db.Characters
    .AsNoTracking()
    .Select(x => new CharacterDto
    {
        Id = x.Id,
        Name = x.Name,
        SpellNames = x.CharacterSpells
            .Select(cs => cs.Spell.Name)
            .ToList()
    })
    .ToListAsync(ct);
```

## Adding entities to the DbContext

Add a `DbSet<T>` when the entity is a top-level query target:

```csharp
public DbSet<Character> Characters { get; set; }
public DbSet<Spell> Spells { get; set; }
public DbSet<CharacterSpell> CharacterSpells { get; set; }
```

Related entity types can also be discovered through navigation properties and configuration. Explicit `DbSet<T>` properties remain useful for clarity and direct querying.

## Complete checklist for a new relationship

1. Add the foreign key property to the dependent entity.
2. Add reference and collection navigation properties where useful.
3. Decide whether the relationship is required or optional.
4. Configure it with `HasOne`/`WithOne`, `HasOne`/`WithMany`, or `HasMany`/`WithMany`.
5. Specify the foreign key explicitly with `HasForeignKey`.
6. Choose an intentional delete behavior.
7. Add a unique index when duplicate relationships must be prevented.
8. Add relevant `DbSet<T>` properties.
9. Build the solution.
10. Generate a migration, inspect its `Up` and `Down` methods, and apply it.

From the repository root:

```powershell
dotnet build Eldoria.sln

dotnet ef migrations add DescribeRelationshipChange `
    --project Eldoria.Infrastructure/Eldoria.Infrastructure.csproj `
    --startup-project Eldoria.Api/Eldoria.Api.csproj `
    --context ApplicationDbContext

dotnet ef database update `
    --project Eldoria.Infrastructure/Eldoria.Infrastructure.csproj `
    --startup-project Eldoria.Api/Eldoria.Api.csproj `
    --context ApplicationDbContext
```

## Common mistakes

- Using a non-nullable foreign key for a relationship intended to be optional.
- Making the foreign key nullable but leaving the navigation non-nullable.
- Putting the foreign key on the wrong side of a one-to-one relationship.
- Configuring the same relationship differently in two configuration files.
- Forgetting a uniqueness constraint on an explicit join entity.
- Using cascade delete on shared reference data.
- Returning EF entities directly from controllers and creating JSON reference cycles.
- Assuming related data is loaded without `Include` or projection.
- Editing an applied migration instead of creating a new corrective migration.
- Applying a migration without checking whether it will drop columns or existing data.
