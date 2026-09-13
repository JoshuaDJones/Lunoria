using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eldoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSceneCounterattacks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CounterattackTargetId",
                table: "ScenePTs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CounterattackToken",
                table: "ScenePTs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CounterattackerId",
                table: "ScenePTs",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CounterattackTargetId",
                table: "ScenePTs");

            migrationBuilder.DropColumn(
                name: "CounterattackToken",
                table: "ScenePTs");

            migrationBuilder.DropColumn(
                name: "CounterattackerId",
                table: "ScenePTs");
        }
    }
}
