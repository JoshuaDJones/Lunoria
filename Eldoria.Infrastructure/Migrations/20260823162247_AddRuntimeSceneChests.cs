using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eldoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRuntimeSceneChests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ScenePTChests_ScenePlaythroughId_SourceSceneChestId",
                table: "ScenePTChests");

            migrationBuilder.DropIndex(
                name: "IX_ScenePTChestLootEntries_ScenePTChestId_SourceSceneChestLootEntryId",
                table: "ScenePTChestLootEntries");

            migrationBuilder.AlterColumn<int>(
                name: "SourceSceneChestId",
                table: "ScenePTChests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "SourceSceneChestLootEntryId",
                table: "ScenePTChestLootEntries",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTChests_ScenePlaythroughId_SourceSceneChestId",
                table: "ScenePTChests",
                columns: new[] { "ScenePlaythroughId", "SourceSceneChestId" },
                unique: true,
                filter: "[SourceSceneChestId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTChestLootEntries_ScenePTChestId_SourceSceneChestLootEntryId",
                table: "ScenePTChestLootEntries",
                columns: new[] { "ScenePTChestId", "SourceSceneChestLootEntryId" },
                unique: true,
                filter: "[SourceSceneChestLootEntryId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ScenePTChests_ScenePlaythroughId_SourceSceneChestId",
                table: "ScenePTChests");

            migrationBuilder.DropIndex(
                name: "IX_ScenePTChestLootEntries_ScenePTChestId_SourceSceneChestLootEntryId",
                table: "ScenePTChestLootEntries");

            migrationBuilder.AlterColumn<int>(
                name: "SourceSceneChestId",
                table: "ScenePTChests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SourceSceneChestLootEntryId",
                table: "ScenePTChestLootEntries",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTChests_ScenePlaythroughId_SourceSceneChestId",
                table: "ScenePTChests",
                columns: new[] { "ScenePlaythroughId", "SourceSceneChestId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTChestLootEntries_ScenePTChestId_SourceSceneChestLootEntryId",
                table: "ScenePTChestLootEntries",
                columns: new[] { "ScenePTChestId", "SourceSceneChestLootEntryId" },
                unique: true);
        }
    }
}
