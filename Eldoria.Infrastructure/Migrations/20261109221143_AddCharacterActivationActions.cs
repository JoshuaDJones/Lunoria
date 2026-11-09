using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eldoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterActivationActions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CharacterActivationActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharacterId = table.Column<int>(type: "int", nullable: true),
                    SceneEventActionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterActivationActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterActivationActions_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CharacterActivationActions_SceneEventActions_SceneEventActionId",
                        column: x => x.SceneEventActionId,
                        principalTable: "SceneEventActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PTCharacterActivationActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlaythroughCharacterId = table.Column<int>(type: "int", nullable: true),
                    ScenePTActionEventId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PTCharacterActivationActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PTCharacterActivationActions_PlaythroughCharacters_PlaythroughCharacterId",
                        column: x => x.PlaythroughCharacterId,
                        principalTable: "PlaythroughCharacters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PTCharacterActivationActions_ScenePTActionEvents_ScenePTActionEventId",
                        column: x => x.ScenePTActionEventId,
                        principalTable: "ScenePTActionEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterActivationActions_CharacterId",
                table: "CharacterActivationActions",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterActivationActions_SceneEventActionId",
                table: "CharacterActivationActions",
                column: "SceneEventActionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PTCharacterActivationActions_PlaythroughCharacterId",
                table: "PTCharacterActivationActions",
                column: "PlaythroughCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_PTCharacterActivationActions_ScenePTActionEventId",
                table: "PTCharacterActivationActions",
                column: "ScenePTActionEventId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterActivationActions");

            migrationBuilder.DropTable(
                name: "PTCharacterActivationActions");
        }
    }
}
