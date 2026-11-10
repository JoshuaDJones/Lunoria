using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eldoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSceneTurnUndo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MovementRecorded",
                table: "ScenePTs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TurnCheckpointJson",
                table: "ScenePTs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TurnStarted",
                table: "ScenePTs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "UndoRevision",
                table: "ScenePTs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<Guid>(
                name: "UndoToken",
                table: "ScenePTs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "GameplayRevision",
                table: "Playthroughs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MovementRecorded",
                table: "ScenePTs");

            migrationBuilder.DropColumn(
                name: "TurnCheckpointJson",
                table: "ScenePTs");

            migrationBuilder.DropColumn(
                name: "TurnStarted",
                table: "ScenePTs");

            migrationBuilder.DropColumn(
                name: "UndoRevision",
                table: "ScenePTs");

            migrationBuilder.DropColumn(
                name: "UndoToken",
                table: "ScenePTs");

            migrationBuilder.DropColumn(
                name: "GameplayRevision",
                table: "Playthroughs");
        }
    }
}
