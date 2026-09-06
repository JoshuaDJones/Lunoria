using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eldoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentAttackAllowance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AttacksRemaining",
                table: "ScenePTParticipants",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "AdditionalAttacksPerTurn",
                table: "PlaythroughEquippableItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AdditionalAttacksPerTurn",
                table: "EquippableItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "CK_ScenePTParticipants_AttacksRemaining",
                table: "ScenePTParticipants",
                sql: "[AttacksRemaining] >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_ScenePTParticipants_AttacksRemaining",
                table: "ScenePTParticipants");

            migrationBuilder.DropColumn(
                name: "AttacksRemaining",
                table: "ScenePTParticipants");

            migrationBuilder.DropColumn(
                name: "AdditionalAttacksPerTurn",
                table: "PlaythroughEquippableItems");

            migrationBuilder.DropColumn(
                name: "AdditionalAttacksPerTurn",
                table: "EquippableItems");
        }
    }
}
