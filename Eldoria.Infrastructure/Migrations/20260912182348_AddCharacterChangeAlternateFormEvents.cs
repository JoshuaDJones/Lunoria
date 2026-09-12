using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eldoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterChangeAlternateFormEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CharacterChangeAlternateFormActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharacterId = table.Column<int>(type: "int", nullable: true),
                    AlternateFormId = table.Column<int>(type: "int", nullable: false),
                    SceneEventActionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterChangeAlternateFormActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterChangeAlternateFormActions_Characters_AlternateFormId",
                        column: x => x.AlternateFormId,
                        principalTable: "Characters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CharacterChangeAlternateFormActions_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CharacterChangeAlternateFormActions_SceneEventActions_SceneEventActionId",
                        column: x => x.SceneEventActionId,
                        principalTable: "SceneEventActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PTCharacterChangeAlternateFormActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceCharacterChangeAlternateFormActionId = table.Column<int>(type: "int", nullable: false),
                    PlaythroughCharacterId = table.Column<int>(type: "int", nullable: true),
                    AlternateFormId = table.Column<int>(type: "int", nullable: false),
                    ScenePTActionEventId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PTCharacterChangeAlternateFormActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PTCharacterChangeAlternateFormActions_PlaythroughCharacters_AlternateFormId",
                        column: x => x.AlternateFormId,
                        principalTable: "PlaythroughCharacters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PTCharacterChangeAlternateFormActions_PlaythroughCharacters_PlaythroughCharacterId",
                        column: x => x.PlaythroughCharacterId,
                        principalTable: "PlaythroughCharacters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PTCharacterChangeAlternateFormActions_ScenePTActionEvents_ScenePTActionEventId",
                        column: x => x.ScenePTActionEventId,
                        principalTable: "ScenePTActionEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterChangeAlternateFormActions_AlternateFormId",
                table: "CharacterChangeAlternateFormActions",
                column: "AlternateFormId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterChangeAlternateFormActions_CharacterId",
                table: "CharacterChangeAlternateFormActions",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterChangeAlternateFormActions_SceneEventActionId",
                table: "CharacterChangeAlternateFormActions",
                column: "SceneEventActionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PTCharacterChangeAlternateFormActions_AlternateFormId",
                table: "PTCharacterChangeAlternateFormActions",
                column: "AlternateFormId");

            migrationBuilder.CreateIndex(
                name: "IX_PTCharacterChangeAlternateFormActions_PlaythroughCharacterId",
                table: "PTCharacterChangeAlternateFormActions",
                column: "PlaythroughCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_PTCharacterChangeAlternateFormActions_ScenePTActionEventId",
                table: "PTCharacterChangeAlternateFormActions",
                column: "ScenePTActionEventId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterChangeAlternateFormActions");

            migrationBuilder.DropTable(
                name: "PTCharacterChangeAlternateFormActions");
        }
    }
}
