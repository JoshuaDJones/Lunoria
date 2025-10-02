using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eldoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixCharacterSpellFks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterSpells_Characters_SpellId",
                table: "CharacterSpells");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterSpells_CharacterId",
                table: "CharacterSpells",
                column: "CharacterId");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterSpells_Characters_CharacterId",
                table: "CharacterSpells",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterSpells_Characters_CharacterId",
                table: "CharacterSpells");

            migrationBuilder.DropIndex(
                name: "IX_CharacterSpells_CharacterId",
                table: "CharacterSpells");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterSpells_Characters_SpellId",
                table: "CharacterSpells",
                column: "SpellId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
