using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eldoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CascadeSpellAssignmentDeletes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JourneyCharacterSpells_Spells_SpellId",
                table: "JourneyCharacterSpells");

            migrationBuilder.AddForeignKey(
                name: "FK_JourneyCharacterSpells_Spells_SpellId",
                table: "JourneyCharacterSpells",
                column: "SpellId",
                principalTable: "Spells",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JourneyCharacterSpells_Spells_SpellId",
                table: "JourneyCharacterSpells");

            migrationBuilder.AddForeignKey(
                name: "FK_JourneyCharacterSpells_Spells_SpellId",
                table: "JourneyCharacterSpells",
                column: "SpellId",
                principalTable: "Spells",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
