using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eldoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class sceneItemIncorrectForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SceneCharacterItems_Items_SceneCharacterId",
                table: "SceneCharacterItems");

            migrationBuilder.CreateIndex(
                name: "IX_SceneCharacterItems_ItemId",
                table: "SceneCharacterItems",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_SceneCharacterItems_Items_ItemId",
                table: "SceneCharacterItems",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SceneCharacterItems_Items_ItemId",
                table: "SceneCharacterItems");

            migrationBuilder.DropIndex(
                name: "IX_SceneCharacterItems_ItemId",
                table: "SceneCharacterItems");

            migrationBuilder.AddForeignKey(
                name: "FK_SceneCharacterItems_Items_SceneCharacterId",
                table: "SceneCharacterItems",
                column: "SceneCharacterId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
