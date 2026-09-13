using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eldoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSceneEventItemGrants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CharacterGiveItemActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharacterId = table.Column<int>(type: "int", nullable: true),
                    ConsumableItemId = table.Column<int>(type: "int", nullable: true),
                    EquippableItemId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    SceneEventActionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterGiveItemActions", x => x.Id);
                    table.CheckConstraint("CK_CharacterGiveItemAction_Item", "([ConsumableItemId] IS NULL AND [EquippableItemId] IS NOT NULL) OR ([ConsumableItemId] IS NOT NULL AND [EquippableItemId] IS NULL)");
                    table.CheckConstraint("CK_CharacterGiveItemAction_Quantity", "[Quantity] BETWEEN 1 AND 1000");
                    table.ForeignKey(
                        name: "FK_CharacterGiveItemActions_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CharacterGiveItemActions_ConsumableItems_ConsumableItemId",
                        column: x => x.ConsumableItemId,
                        principalTable: "ConsumableItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CharacterGiveItemActions_EquippableItems_EquippableItemId",
                        column: x => x.EquippableItemId,
                        principalTable: "EquippableItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CharacterGiveItemActions_SceneEventActions_SceneEventActionId",
                        column: x => x.SceneEventActionId,
                        principalTable: "SceneEventActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PTCharacterGiveItemActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceCharacterGiveItemActionId = table.Column<int>(type: "int", nullable: false),
                    PlaythroughCharacterId = table.Column<int>(type: "int", nullable: true),
                    PlaythroughConsumableItemId = table.Column<int>(type: "int", nullable: true),
                    PlaythroughEquippableItemId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ScenePTActionEventId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PTCharacterGiveItemActions", x => x.Id);
                    table.CheckConstraint("CK_PTCharacterGiveItemAction_Item", "([PlaythroughConsumableItemId] IS NULL AND [PlaythroughEquippableItemId] IS NOT NULL) OR ([PlaythroughConsumableItemId] IS NOT NULL AND [PlaythroughEquippableItemId] IS NULL)");
                    table.CheckConstraint("CK_PTCharacterGiveItemAction_Quantity", "[Quantity] BETWEEN 1 AND 1000");
                    table.ForeignKey(
                        name: "FK_PTCharacterGiveItemActions_PlaythroughCharacters_PlaythroughCharacterId",
                        column: x => x.PlaythroughCharacterId,
                        principalTable: "PlaythroughCharacters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PTCharacterGiveItemActions_PlaythroughConsumableItems_PlaythroughConsumableItemId",
                        column: x => x.PlaythroughConsumableItemId,
                        principalTable: "PlaythroughConsumableItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PTCharacterGiveItemActions_PlaythroughEquippableItems_PlaythroughEquippableItemId",
                        column: x => x.PlaythroughEquippableItemId,
                        principalTable: "PlaythroughEquippableItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PTCharacterGiveItemActions_ScenePTActionEvents_ScenePTActionEventId",
                        column: x => x.ScenePTActionEventId,
                        principalTable: "ScenePTActionEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterGiveItemActions_CharacterId",
                table: "CharacterGiveItemActions",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterGiveItemActions_ConsumableItemId",
                table: "CharacterGiveItemActions",
                column: "ConsumableItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterGiveItemActions_EquippableItemId",
                table: "CharacterGiveItemActions",
                column: "EquippableItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterGiveItemActions_SceneEventActionId",
                table: "CharacterGiveItemActions",
                column: "SceneEventActionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PTCharacterGiveItemActions_PlaythroughCharacterId",
                table: "PTCharacterGiveItemActions",
                column: "PlaythroughCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_PTCharacterGiveItemActions_PlaythroughConsumableItemId",
                table: "PTCharacterGiveItemActions",
                column: "PlaythroughConsumableItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PTCharacterGiveItemActions_PlaythroughEquippableItemId",
                table: "PTCharacterGiveItemActions",
                column: "PlaythroughEquippableItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PTCharacterGiveItemActions_ScenePTActionEventId",
                table: "PTCharacterGiveItemActions",
                column: "ScenePTActionEventId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterGiveItemActions");

            migrationBuilder.DropTable(
                name: "PTCharacterGiveItemActions");
        }
    }
}
