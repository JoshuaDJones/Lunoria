using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eldoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PauseSceneStartForInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InventoryResolutionToken",
                table: "ScenePTs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PendingInventoryActionId",
                table: "ScenePTs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GrantItemsHandled",
                table: "ScenePTActionEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GrantRecipientIndex",
                table: "ScenePTActionEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "ScenePTActionEvents",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InventoryResolutionToken",
                table: "ScenePTs");

            migrationBuilder.DropColumn(
                name: "PendingInventoryActionId",
                table: "ScenePTs");

            migrationBuilder.DropColumn(
                name: "GrantItemsHandled",
                table: "ScenePTActionEvents");

            migrationBuilder.DropColumn(
                name: "GrantRecipientIndex",
                table: "ScenePTActionEvents");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "ScenePTActionEvents");
        }
    }
}
