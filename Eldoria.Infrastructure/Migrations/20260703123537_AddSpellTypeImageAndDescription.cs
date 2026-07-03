using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eldoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSpellTypeImageAndDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SpellTypes_UserId_TypeName",
                table: "SpellTypes");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "SpellTypes",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "SpellTypes",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "SpellTypes",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_SpellTypes_UserId_TypeName",
                table: "SpellTypes",
                columns: new[] { "UserId", "TypeName" },
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SpellTypes_UserId_TypeName",
                table: "SpellTypes");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "SpellTypes");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "SpellTypes");

            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "SpellTypes");

            migrationBuilder.CreateIndex(
                name: "IX_SpellTypes_UserId_TypeName",
                table: "SpellTypes",
                columns: new[] { "UserId", "TypeName" },
                unique: true);
        }
    }
}
