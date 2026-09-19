using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eldoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterInAlternateFormActions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CharacterInAlternateFormActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharacterId = table.Column<int>(type: "int", nullable: true),
                    SceneEventActionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterInAlternateFormActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterInAlternateFormActions_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CharacterInAlternateFormActions_SceneEventActions_SceneEventActionId",
                        column: x => x.SceneEventActionId,
                        principalTable: "SceneEventActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PTCharacterInAlternateFormActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlaythroughCharacterId = table.Column<int>(type: "int", nullable: true),
                    ScenePTActionEventId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PTCharacterInAlternateFormActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PTCharacterInAlternateFormActions_PlaythroughCharacters_PlaythroughCharacterId",
                        column: x => x.PlaythroughCharacterId,
                        principalTable: "PlaythroughCharacters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PTCharacterInAlternateFormActions_ScenePTActionEvents_ScenePTActionEventId",
                        column: x => x.ScenePTActionEventId,
                        principalTable: "ScenePTActionEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterInAlternateFormActions_CharacterId",
                table: "CharacterInAlternateFormActions",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterInAlternateFormActions_SceneEventActionId",
                table: "CharacterInAlternateFormActions",
                column: "SceneEventActionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PTCharacterInAlternateFormActions_PlaythroughCharacterId",
                table: "PTCharacterInAlternateFormActions",
                column: "PlaythroughCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_PTCharacterInAlternateFormActions_ScenePTActionEventId",
                table: "PTCharacterInAlternateFormActions",
                column: "ScenePTActionEventId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterInAlternateFormActions");

            migrationBuilder.DropTable(
                name: "PTCharacterInAlternateFormActions");
        }
    }
}
