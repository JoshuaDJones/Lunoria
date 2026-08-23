using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eldoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddScenePlaythroughCombat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DownedTurnsRemaining",
                table: "ScenePTParticipants",
                type: "int",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_ScenePTParticipants_DownedTurnsRemaining",
                table: "ScenePTParticipants",
                sql: "[DownedTurnsRemaining] IS NULL OR [DownedTurnsRemaining] >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_ScenePTParticipants_DownedTurnsRemaining",
                table: "ScenePTParticipants");

            migrationBuilder.DropColumn(
                name: "DownedTurnsRemaining",
                table: "ScenePTParticipants");
        }
    }
}
