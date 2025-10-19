using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eldoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class characteritem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JourneyCharacterItems_Items_JourneyCharacterId",
                table: "JourneyCharacterItems");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyCharacterItems_ItemId",
                table: "JourneyCharacterItems",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_JourneyCharacterItems_Items_ItemId",
                table: "JourneyCharacterItems",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JourneyCharacterItems_Items_ItemId",
                table: "JourneyCharacterItems");

            migrationBuilder.DropIndex(
                name: "IX_JourneyCharacterItems_ItemId",
                table: "JourneyCharacterItems");

            migrationBuilder.AddForeignKey(
                name: "FK_JourneyCharacterItems_Items_JourneyCharacterId",
                table: "JourneyCharacterItems",
                column: "JourneyCharacterId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
