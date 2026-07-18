using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eldoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnforceCatalogOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                -- ExpandGameplayDomain creates an ownerless fallback spell type even for a
                -- brand-new database. If there are no users, remove only ownerless spell
                -- types that are not referenced by any spell before enforcing ownership.
                IF NOT EXISTS (SELECT 1 FROM [Users])
                BEGIN
                    DELETE st
                    FROM [SpellTypes] st
                    WHERE st.[UserId] IS NULL
                      AND NOT EXISTS (
                          SELECT 1 FROM [Spells] s WHERE s.[SpellTypeId] = st.[Id]
                      );
                END;

                IF (
                    EXISTS (SELECT 1 FROM [Characters] WHERE [UserId] IS NULL)
                    OR EXISTS (SELECT 1 FROM [Items] WHERE [UserId] IS NULL)
                    OR EXISTS (SELECT 1 FROM [Spells] WHERE [UserId] IS NULL)
                    OR EXISTS (SELECT 1 FROM [SpellTypes] WHERE [UserId] IS NULL)
                    OR EXISTS (SELECT 1 FROM [EquippableItems] WHERE [UserId] IS NULL)
                )
                AND (SELECT COUNT(*) FROM [Users]) <> 1
                BEGIN
                    THROW 51000, 'Catalog ownership backfill is ambiguous. Assign all null UserId values before applying this migration.', 1;
                END;

                DECLARE @CatalogOwnerId int = (SELECT MIN([Id]) FROM [Users]);

                UPDATE [Characters] SET [UserId] = @CatalogOwnerId WHERE [UserId] IS NULL;
                UPDATE [Items] SET [UserId] = @CatalogOwnerId WHERE [UserId] IS NULL;
                UPDATE [Spells] SET [UserId] = @CatalogOwnerId WHERE [UserId] IS NULL;
                UPDATE [SpellTypes] SET [UserId] = @CatalogOwnerId WHERE [UserId] IS NULL;
                UPDATE [EquippableItems] SET [UserId] = @CatalogOwnerId WHERE [UserId] IS NULL;
                """);

            migrationBuilder.DropIndex(
                name: "IX_SpellTypes_UserId_TypeName",
                table: "SpellTypes");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "SpellTypes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Spells",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "EquippableItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Characters",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpellTypes_UserId_TypeName",
                table: "SpellTypes",
                columns: new[] { "UserId", "TypeName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SpellTypes_UserId_TypeName",
                table: "SpellTypes");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "SpellTypes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Spells",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Items",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "EquippableItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Characters",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_SpellTypes_UserId_TypeName",
                table: "SpellTypes",
                columns: new[] { "UserId", "TypeName" },
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }
    }
}
