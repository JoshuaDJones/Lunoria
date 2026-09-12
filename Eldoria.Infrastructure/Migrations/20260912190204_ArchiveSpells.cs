using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eldoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ArchiveSpells : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Spells_UserId_Name",
                table: "Spells");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Spells",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Spells",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Spells_UserId_Name",
                table: "Spells",
                columns: new[] { "UserId", "Name" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Spells_UserId_Name",
                table: "Spells");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Spells");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Spells");

            migrationBuilder.CreateIndex(
                name: "IX_Spells_UserId_Name",
                table: "Spells",
                columns: new[] { "UserId", "Name" },
                unique: true);
        }
    }
}
