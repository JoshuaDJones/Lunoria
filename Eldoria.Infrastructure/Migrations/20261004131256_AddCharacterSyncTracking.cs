using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eldoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterSyncTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Revision",
                table: "Spells",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Spells",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<int>(
                name: "AcknowledgedSpellRevision",
                table: "SceneCharacterSpells",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AcknowledgedAlternateFormRevision",
                table: "SceneCharacters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AcknowledgedSpellAssignmentsRevision",
                table: "SceneCharacters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AcknowledgedStatsRevision",
                table: "SceneCharacters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "SceneCharacters",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<int>(
                name: "SyncedAlternateFormRevision",
                table: "SceneCharacters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SyncedSpellAssignmentsRevision",
                table: "SceneCharacters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SyncedStatsRevision",
                table: "SceneCharacters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AcknowledgedSpellRevision",
                table: "JourneyCharacterSpells",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AcknowledgedAlternateFormRevision",
                table: "JourneyCharacters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AcknowledgedSpellAssignmentsRevision",
                table: "JourneyCharacters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AcknowledgedStatsRevision",
                table: "JourneyCharacters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "JourneyCharacters",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<int>(
                name: "SyncedAlternateFormRevision",
                table: "JourneyCharacters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SyncedSpellAssignmentsRevision",
                table: "JourneyCharacters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SyncedStatsRevision",
                table: "JourneyCharacters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AlternateFormRevision",
                table: "Characters",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Characters",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<int>(
                name: "SpellAssignmentsRevision",
                table: "Characters",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "StatsRevision",
                table: "Characters",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Revision",
                table: "Spells");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Spells");

            migrationBuilder.DropColumn(
                name: "AcknowledgedSpellRevision",
                table: "SceneCharacterSpells");

            migrationBuilder.DropColumn(
                name: "AcknowledgedAlternateFormRevision",
                table: "SceneCharacters");

            migrationBuilder.DropColumn(
                name: "AcknowledgedSpellAssignmentsRevision",
                table: "SceneCharacters");

            migrationBuilder.DropColumn(
                name: "AcknowledgedStatsRevision",
                table: "SceneCharacters");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "SceneCharacters");

            migrationBuilder.DropColumn(
                name: "SyncedAlternateFormRevision",
                table: "SceneCharacters");

            migrationBuilder.DropColumn(
                name: "SyncedSpellAssignmentsRevision",
                table: "SceneCharacters");

            migrationBuilder.DropColumn(
                name: "SyncedStatsRevision",
                table: "SceneCharacters");

            migrationBuilder.DropColumn(
                name: "AcknowledgedSpellRevision",
                table: "JourneyCharacterSpells");

            migrationBuilder.DropColumn(
                name: "AcknowledgedAlternateFormRevision",
                table: "JourneyCharacters");

            migrationBuilder.DropColumn(
                name: "AcknowledgedSpellAssignmentsRevision",
                table: "JourneyCharacters");

            migrationBuilder.DropColumn(
                name: "AcknowledgedStatsRevision",
                table: "JourneyCharacters");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "JourneyCharacters");

            migrationBuilder.DropColumn(
                name: "SyncedAlternateFormRevision",
                table: "JourneyCharacters");

            migrationBuilder.DropColumn(
                name: "SyncedSpellAssignmentsRevision",
                table: "JourneyCharacters");

            migrationBuilder.DropColumn(
                name: "SyncedStatsRevision",
                table: "JourneyCharacters");

            migrationBuilder.DropColumn(
                name: "AlternateFormRevision",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "SpellAssignmentsRevision",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "StatsRevision",
                table: "Characters");
        }
    }
}
