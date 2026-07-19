# Entity Framework Core Command Reference

This project uses EF Core 8 with SQL Server. Run the commands below from the repository root.

## Project values

```powershell
$Infrastructure = "Eldoria.Infrastructure/Eldoria.Infrastructure.csproj"
$Startup = "Eldoria.Api/Eldoria.Api.csproj"
$Context = "ApplicationDbContext"
```

The design-time context currently targets the LocalDB database named `Lunoria`.

## One-time CLI setup

Check whether the EF CLI is installed:

```powershell
dotnet ef --version
```

Install or update it for EF Core 8:

```powershell
dotnet tool install --global dotnet-ef --version 8.*
dotnet tool update --global dotnet-ef --version 8.*
```

Restore and build the solution:

```powershell
dotnet restore
dotnet build Eldoria.sln
```

## Common migration commands

Add a migration after changing entities or EF configurations:

```powershell
dotnet ef migrations add AddExampleTable --project $Infrastructure --startup-project $Startup --context $Context
```

Apply all pending migrations to the database:

```powershell
dotnet ef database update --project $Infrastructure --startup-project $Startup --context $Context
```

List migrations:

```powershell
dotnet ef migrations list --project $Infrastructure --startup-project $Startup --context $Context
```

Check for model changes that do not have a migration:

```powershell
dotnet ef migrations has-pending-model-changes --project $Infrastructure --startup-project $Startup --context $Context
```

Remove the most recently created migration if it has not been applied:

```powershell
dotnet ef migrations remove --project $Infrastructure --startup-project $Startup --context $Context
```

Force removal only when you understand the consequences:

```powershell
dotnet ef migrations remove --force --project $Infrastructure --startup-project $Startup --context $Context
```

## Add, alter, rename, or drop a table

EF Core does not provide separate CLI commands such as `add table` or `drop table`. Change the entity model/configuration, create a migration, inspect it, and then update the database.

### Add a table

1. Add the entity class.
2. Add its `DbSet<TEntity>` to `ApplicationDbContext` if desired.
3. Add or update its `IEntityTypeConfiguration<TEntity>` configuration.
4. Generate and apply a migration:

```powershell
dotnet ef migrations add AddProductsTable --project $Infrastructure --startup-project $Startup --context $Context
dotnet ef database update --project $Infrastructure --startup-project $Startup --context $Context
```

The generated migration normally uses:

```csharp
migrationBuilder.CreateTable(
    name: "Products",
    columns: table => new
    {
        Id = table.Column<int>(nullable: false)
            .Annotation("SqlServer:Identity", "1, 1"),
        Name = table.Column<string>(maxLength: 200, nullable: false)
    },
    constraints: table =>
    {
        table.PrimaryKey("PK_Products", x => x.Id);
    });
```

### Add or alter a column

Change the entity property/configuration, then run:

```powershell
dotnet ef migrations add AddDescriptionToProducts --project $Infrastructure --startup-project $Startup --context $Context
dotnet ef database update --project $Infrastructure --startup-project $Startup --context $Context
```

Generated migrations use methods such as `AddColumn`, `AlterColumn`, and `DropColumn`.

### Rename a table or column

After renaming the model member, verify that EF generated a rename instead of a destructive drop-and-create operation:

```csharp
migrationBuilder.RenameTable(name: "OldProducts", newName: "Products");
migrationBuilder.RenameColumn(name: "OldName", table: "Products", newName: "Name");
```

Then apply the migration normally.

### Drop a table

Remove the entity, its configuration, relationships, and its `DbSet`, then create and apply a migration:

```powershell
dotnet ef migrations add DropProductsTable --project $Infrastructure --startup-project $Startup --context $Context
dotnet ef database update --project $Infrastructure --startup-project $Startup --context $Context
```

The generated migration should contain:

```csharp
migrationBuilder.DropTable(name: "Products");
```

Dropping a table permanently deletes its data. Review the migration and back up important data before applying it.

## Roll back migrations

First list migrations, then update to the last migration you want to keep:

```powershell
dotnet ef migrations list --project $Infrastructure --startup-project $Startup --context $Context
dotnet ef database update PreviousMigrationName --project $Infrastructure --startup-project $Startup --context $Context
```

Roll back every migration to an empty database schema:

```powershell
dotnet ef database update 0 --project $Infrastructure --startup-project $Startup --context $Context
```

After rolling back the latest migration, remove its migration files if they should no longer exist:

```powershell
dotnet ef migrations remove --project $Infrastructure --startup-project $Startup --context $Context
```

Do not rewrite migrations that have already been applied to shared or production databases. Create a new corrective migration instead.

## Drop or recreate the entire database

Confirm before dropping the database:

```powershell
dotnet ef database drop --project $Infrastructure --startup-project $Startup --context $Context
```

Drop without an interactive prompt:

```powershell
dotnet ef database drop --force --project $Infrastructure --startup-project $Startup --context $Context
```

Recreate it by applying all migrations:

```powershell
dotnet ef database update --project $Infrastructure --startup-project $Startup --context $Context
```

## Generate SQL scripts

Generate a script for all migrations:

```powershell
dotnet ef migrations script --project $Infrastructure --startup-project $Startup --context $Context --output migration.sql
```

Generate a safer idempotent deployment script:

```powershell
dotnet ef migrations script --idempotent --project $Infrastructure --startup-project $Startup --context $Context --output migration-idempotent.sql
```

Generate SQL between two migrations:

```powershell
dotnet ef migrations script FromMigration ToMigration --project $Infrastructure --startup-project $Startup --context $Context --output migration-range.sql
```

Generate a rollback script by placing the newer migration first:

```powershell
dotnet ef migrations script NewerMigration OlderMigration --project $Infrastructure --startup-project $Startup --context $Context --output rollback.sql
```

## Inspect the model and context

List available contexts:

```powershell
dotnet ef dbcontext list --project $Infrastructure --startup-project $Startup
```

Show context information:

```powershell
dotnet ef dbcontext info --project $Infrastructure --startup-project $Startup --context $Context
```

Generate SQL from the current model without applying it:

```powershell
dotnet ef dbcontext script --project $Infrastructure --startup-project $Startup --context $Context --output current-model.sql
```

## Visual Studio Package Manager Console equivalents

Set `Eldoria.Infrastructure` as the Default project and `Eldoria.Api` as the Startup Project, then use:

```powershell
Add-Migration AddExampleTable -Context ApplicationDbContext
Update-Database -Context ApplicationDbContext
Get-Migration -Context ApplicationDbContext
Remove-Migration -Context ApplicationDbContext
Update-Database PreviousMigrationName -Context ApplicationDbContext
Drop-Database -Context ApplicationDbContext
Script-Migration -Idempotent -Context ApplicationDbContext
```

## Typical workflow

```powershell
# 1. Change entities and configurations.
# 2. Build to catch compile errors.
dotnet build Eldoria.sln

# 3. Create a clearly named migration.
dotnet ef migrations add DescribeTheSchemaChange --project $Infrastructure --startup-project $Startup --context $Context

# 4. Review the generated Up(), Down(), and model snapshot changes.
# 5. Apply it locally.
dotnet ef database update --project $Infrastructure --startup-project $Startup --context $Context

# 6. Confirm there are no untracked model changes.
dotnet ef migrations has-pending-model-changes --project $Infrastructure --startup-project $Startup --context $Context
```

## Troubleshooting

- **`dotnet ef` is not recognized:** install the `dotnet-ef` global tool and restart the terminal.
- **Build failed:** run `dotnet build Eldoria.sln` and fix compilation errors first.
- **Unable to create a DbContext:** confirm `ApplicationDbContextFactory` builds and that LocalDB is available.
- **Wrong database:** inspect the connection string in `Eldoria.Infrastructure/Db/ApplicationDbContextFactory.cs` before applying or dropping anything.
- **Migration name already exists:** choose a unique, descriptive migration name.
- **Unexpected data loss warning:** inspect the generated migration carefully; a rename may have been interpreted as a drop and add.
