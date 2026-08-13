using Eldoria.Infrastructure.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eldoria.Infrastructure.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260810000000_AddImmutablePlaythroughSnapshots")]
public partial class AddImmutablePlaythroughSnapshots : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        DropSourceForeignKeys(migrationBuilder);
        DropSourceIndexes(migrationBuilder);

        migrationBuilder.CreateTable(
            name: "JourneyRevisions",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                RevisionNumber = table.Column<int>(type: "int", nullable: false),
                SchemaVersion = table.Column<int>(type: "int", nullable: false),
                ContentHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                SnapshotJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                SourceJourneyId = table.Column<int>(type: "int", nullable: true),
                CreatedByUserId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_JourneyRevisions", x => x.Id);
                table.ForeignKey("FK_JourneyRevisions_Journeys_SourceJourneyId", x => x.SourceJourneyId,
                    "Journeys", "Id", onDelete: ReferentialAction.SetNull);
                table.ForeignKey("FK_JourneyRevisions_Users_CreatedByUserId", x => x.CreatedByUserId,
                    "Users", "Id", onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex("IX_JourneyRevisions_CreatedByUserId_SourceJourneyId_ContentHash",
            "JourneyRevisions", new[] { "CreatedByUserId", "SourceJourneyId", "ContentHash" }, unique: true,
            filter: "[SourceJourneyId] IS NOT NULL");
        migrationBuilder.CreateIndex("IX_JourneyRevisions_CreatedByUserId_SourceJourneyId_RevisionNumber",
            "JourneyRevisions", new[] { "CreatedByUserId", "SourceJourneyId", "RevisionNumber" }, unique: true,
            filter: "[SourceJourneyId] IS NOT NULL");
        migrationBuilder.CreateIndex("IX_JourneyRevisions_SourceJourneyId", "JourneyRevisions", "SourceJourneyId");

        migrationBuilder.AddColumn<int>("JourneyRevisionId", "JourneyPlaythroughs", "int", nullable: true);
        migrationBuilder.AddColumn<int>("SourceJourneyId", "JourneyPlaythroughs", "int", nullable: false, defaultValue: 0);
        migrationBuilder.AddColumn<string>("SnapshotCharacterKey", "JourneyPlaythroughCharacters", "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "");
        migrationBuilder.AddColumn<string>("SnapshotAssignmentKey", "JourneyPlaythroughCharacters", "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "");
        migrationBuilder.AddColumn<string>("SnapshotSpellKey", "JourneyPlaythroughCharacterSpells", "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "");
        migrationBuilder.AddColumn<string>("SnapshotConsumableKey", "JourneyPlaythroughCharacterConsumableItems", "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "");
        migrationBuilder.AddColumn<string>("SnapshotEquipmentKey", "JourneyPlaythroughCharacterEquippableItems", "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "");
        migrationBuilder.AddColumn<bool>("IsEquipped", "JourneyPlaythroughCharacterEquippableItems", "bit", nullable: false, defaultValue: false);
        migrationBuilder.AddColumn<string>("SnapshotSceneKey", "ScenePlaythroughs", "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "");
        migrationBuilder.AddColumn<int>("SnapshotSortOrder", "ScenePlaythroughs", "int", nullable: false, defaultValue: 0);
        migrationBuilder.AddColumn<int>("SourceSceneId", "ScenePlaythroughs", "int", nullable: false, defaultValue: 0);
        migrationBuilder.AddColumn<string>("SnapshotCharacterKey", "ScenePlaythroughCharacters", "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "");
        migrationBuilder.AddColumn<string>("SnapshotAssignmentKey", "ScenePlaythroughCharacters", "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "");
        migrationBuilder.AddColumn<string>("SnapshotSpellKey", "ScenePlaythroughCharacterSpells", "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "");
        migrationBuilder.AddColumn<string>("SnapshotConsumableKey", "ScenePlaythroughCharacterConsumableItems", "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "");
        migrationBuilder.AddColumn<string>("SnapshotEquipmentKey", "ScenePlaythroughCharacterEquippableItems", "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "");
        migrationBuilder.AddColumn<bool>("IsEquipped", "ScenePlaythroughCharacterEquippableItems", "bit", nullable: false, defaultValue: false);
        migrationBuilder.AddColumn<string>("SnapshotEventKey", "ScenePlaythroughEvents", "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "");
        migrationBuilder.AddColumn<string>("SnapshotChestKey", "ScenePlaythroughChests", "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "");
        migrationBuilder.AddColumn<string>("SelectedLootEntrySnapshotKey", "ScenePlaythroughChests", "nvarchar(100)", maxLength: 100, nullable: true);

        migrationBuilder.Sql("""
            INSERT INTO JourneyRevisions
                (RevisionNumber, SchemaVersion, ContentHash, SnapshotJson, CreatedAt, SourceJourneyId, CreatedByUserId)
            SELECT
                ROW_NUMBER() OVER (PARTITION BY p.JourneyId ORDER BY p.Id),
                0,
                CONCAT('LEGACY-', p.Id),
                CONCAT('{"schemaVersion":0,"journey":{"sourceJourneyId":', p.JourneyId,
                    ',"name":"Legacy playthrough","description":"","photoUrl":"","fileName":"","sortOrder":0,"introPages":[],"characters":[],"sceneKeys":[]},',
                    '"characters":[],"spellTypes":[],"spells":[],"consumables":[],"equipment":[],"scenes":[]}'),
                p.StartedAt,
                p.JourneyId,
                j.UserId
            FROM JourneyPlaythroughs p
            INNER JOIN Journeys j ON j.Id = p.JourneyId;

            UPDATE p
            SET JourneyRevisionId = r.Id, SourceJourneyId = p.JourneyId
            FROM JourneyPlaythroughs p
            INNER JOIN JourneyRevisions r ON r.ContentHash = CONCAT('LEGACY-', p.Id);

            UPDATE pc SET SnapshotAssignmentKey = CONCAT('journey-character:', pc.JourneyCharacterId),
                SnapshotCharacterKey = CONCAT('character:', jc.CharacterId)
            FROM JourneyPlaythroughCharacters pc INNER JOIN JourneyCharacters jc ON jc.Id = pc.JourneyCharacterId;
            UPDATE JourneyPlaythroughCharacterSpells SET SnapshotSpellKey = CONCAT('journey-character-spell:', JourneyCharacterSpellId);
            UPDATE JourneyPlaythroughCharacterConsumableItems SET SnapshotConsumableKey = CONCAT('consumable:', ConsumableItemId);
            UPDATE JourneyPlaythroughCharacterEquippableItems SET SnapshotEquipmentKey = CONCAT('equipment:', EquippableItemId);
            UPDATE sp SET SnapshotSceneKey = CONCAT('scene:', sp.SceneId), SnapshotSortOrder = s.SortOrder,
                SourceSceneId = sp.SceneId
            FROM ScenePlaythroughs sp INNER JOIN Scenes s ON s.Id = sp.SceneId;
            UPDATE pc SET SnapshotAssignmentKey = CONCAT('scene-character:', pc.SceneCharacterId),
                SnapshotCharacterKey = CONCAT('character:', sc.CharacterId)
            FROM ScenePlaythroughCharacters pc INNER JOIN SceneCharacters sc ON sc.Id = pc.SceneCharacterId;
            UPDATE ScenePlaythroughCharacterSpells SET SnapshotSpellKey = CONCAT('scene-character-spell:', SceneCharacterSpellId);
            UPDATE ScenePlaythroughCharacterConsumableItems SET SnapshotConsumableKey = CONCAT('consumable:', ConsumableItemId);
            UPDATE ScenePlaythroughCharacterEquippableItems SET SnapshotEquipmentKey = CONCAT('equipment:', EquippableItemId);
            UPDATE ScenePlaythroughEvents SET SnapshotEventKey = CONCAT('event:', SceneEventId);
            UPDATE ScenePlaythroughChests SET SnapshotChestKey = CONCAT('chest:', SceneChestId),
                SelectedLootEntrySnapshotKey = CASE WHEN SelectedLootEntryId IS NULL THEN NULL ELSE CONCAT('loot:', SelectedLootEntryId) END;
            """);

        migrationBuilder.AlterColumn<int>("JourneyRevisionId", "JourneyPlaythroughs", "int", nullable: false, oldClrType: typeof(int), oldType: "int", oldNullable: true);
        AlterSourceColumns(migrationBuilder, nullable: true);

        migrationBuilder.CreateIndex("IX_JourneyPlaythroughs_JourneyRevisionId", "JourneyPlaythroughs", "JourneyRevisionId");
        migrationBuilder.CreateIndex("IX_JourneyPlaythroughs_JourneyId", "JourneyPlaythroughs", "JourneyId");
        migrationBuilder.CreateIndex("IX_JourneyPlaythroughs_SourceJourneyId", "JourneyPlaythroughs", "SourceJourneyId", unique: true,
            filter: "[IsActive] = 1");
        migrationBuilder.CreateIndex("IX_JourneyPlaythroughCharacters_JourneyPlaythroughId_SnapshotAssignmentKey",
            "JourneyPlaythroughCharacters", new[] { "JourneyPlaythroughId", "SnapshotAssignmentKey" }, unique: true);
        migrationBuilder.CreateIndex("IX_JourneyPlaythroughCharacterSpells_JourneyPlaythroughCharacterId_SnapshotSpellKey",
            "JourneyPlaythroughCharacterSpells", new[] { "JourneyPlaythroughCharacterId", "SnapshotSpellKey" }, unique: true);
        migrationBuilder.CreateIndex("IX_ScenePlaythroughs_JourneyPlaythroughId_SnapshotSceneKey",
            "ScenePlaythroughs", new[] { "JourneyPlaythroughId", "SnapshotSceneKey" }, unique: true);
        migrationBuilder.CreateIndex("IX_ScenePlaythroughCharacters_ScenePlaythroughId_SnapshotAssignmentKey",
            "ScenePlaythroughCharacters", new[] { "ScenePlaythroughId", "SnapshotAssignmentKey" }, unique: true);
        migrationBuilder.CreateIndex("IX_ScenePlaythroughCharacterSpells_ScenePlaythroughCharacterId_SnapshotSpellKey",
            "ScenePlaythroughCharacterSpells", new[] { "ScenePlaythroughCharacterId", "SnapshotSpellKey" }, unique: true);
        migrationBuilder.CreateIndex("IX_ScenePlaythroughEvents_ScenePlaythroughId_SnapshotEventKey",
            "ScenePlaythroughEvents", new[] { "ScenePlaythroughId", "SnapshotEventKey" }, unique: true);
        migrationBuilder.CreateIndex("IX_ScenePlaythroughChests_ScenePlaythroughId_SnapshotChestKey",
            "ScenePlaythroughChests", new[] { "ScenePlaythroughId", "SnapshotChestKey" }, unique: true);

        AddSourceForeignKeys(migrationBuilder, ReferentialAction.SetNull);
        migrationBuilder.AddForeignKey("FK_JourneyPlaythroughs_JourneyRevisions_JourneyRevisionId", "JourneyPlaythroughs",
            "JourneyRevisionId", "JourneyRevisions", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey("FK_JourneyPlaythroughs_JourneyRevisions_JourneyRevisionId", "JourneyPlaythroughs");
        DropSourceForeignKeys(migrationBuilder);
        DropSnapshotIndexes(migrationBuilder);
        migrationBuilder.DropIndex("IX_JourneyPlaythroughs_JourneyRevisionId", "JourneyPlaythroughs");

        AlterSourceColumns(migrationBuilder, nullable: false);
        AddSourceForeignKeys(migrationBuilder, ReferentialAction.NoAction, journeyDelete: ReferentialAction.Cascade);

        migrationBuilder.CreateIndex("IX_JourneyPlaythroughs_JourneyId", "JourneyPlaythroughs", "JourneyId", unique: true, filter: "[IsActive] = 1");
        migrationBuilder.CreateIndex("IX_JourneyPlaythroughCharacters_JourneyPlaythroughId_JourneyCharacterId", "JourneyPlaythroughCharacters", new[] { "JourneyPlaythroughId", "JourneyCharacterId" }, unique: true);
        migrationBuilder.CreateIndex("IX_JourneyPlaythroughCharacterSpells_JourneyPlaythroughCharacterId_JourneyCharacterSpellId", "JourneyPlaythroughCharacterSpells", new[] { "JourneyPlaythroughCharacterId", "JourneyCharacterSpellId" }, unique: true);
        migrationBuilder.CreateIndex("IX_ScenePlaythroughs_JourneyPlaythroughId_SceneId", "ScenePlaythroughs", new[] { "JourneyPlaythroughId", "SceneId" }, unique: true);
        migrationBuilder.CreateIndex("IX_ScenePlaythroughCharacters_ScenePlaythroughId_SceneCharacterId", "ScenePlaythroughCharacters", new[] { "ScenePlaythroughId", "SceneCharacterId" }, unique: true);
        migrationBuilder.CreateIndex("IX_ScenePlaythroughCharacterSpells_ScenePlaythroughCharacterId_SceneCharacterSpellId", "ScenePlaythroughCharacterSpells", new[] { "ScenePlaythroughCharacterId", "SceneCharacterSpellId" }, unique: true);
        migrationBuilder.CreateIndex("IX_ScenePlaythroughEvents_ScenePlaythroughId_SceneEventId", "ScenePlaythroughEvents", new[] { "ScenePlaythroughId", "SceneEventId" }, unique: true);
        migrationBuilder.CreateIndex("IX_ScenePlaythroughChests_ScenePlaythroughId_SceneChestId", "ScenePlaythroughChests", new[] { "ScenePlaythroughId", "SceneChestId" }, unique: true);

        migrationBuilder.DropColumn("JourneyRevisionId", "JourneyPlaythroughs");
        migrationBuilder.DropColumn("SourceJourneyId", "JourneyPlaythroughs");
        migrationBuilder.DropColumn("SnapshotCharacterKey", "JourneyPlaythroughCharacters");
        migrationBuilder.DropColumn("SnapshotAssignmentKey", "JourneyPlaythroughCharacters");
        migrationBuilder.DropColumn("SnapshotSpellKey", "JourneyPlaythroughCharacterSpells");
        migrationBuilder.DropColumn("SnapshotConsumableKey", "JourneyPlaythroughCharacterConsumableItems");
        migrationBuilder.DropColumn("SnapshotEquipmentKey", "JourneyPlaythroughCharacterEquippableItems");
        migrationBuilder.DropColumn("IsEquipped", "JourneyPlaythroughCharacterEquippableItems");
        migrationBuilder.DropColumn("SnapshotSceneKey", "ScenePlaythroughs");
        migrationBuilder.DropColumn("SnapshotSortOrder", "ScenePlaythroughs");
        migrationBuilder.DropColumn("SourceSceneId", "ScenePlaythroughs");
        migrationBuilder.DropColumn("SnapshotCharacterKey", "ScenePlaythroughCharacters");
        migrationBuilder.DropColumn("SnapshotAssignmentKey", "ScenePlaythroughCharacters");
        migrationBuilder.DropColumn("SnapshotSpellKey", "ScenePlaythroughCharacterSpells");
        migrationBuilder.DropColumn("SnapshotConsumableKey", "ScenePlaythroughCharacterConsumableItems");
        migrationBuilder.DropColumn("SnapshotEquipmentKey", "ScenePlaythroughCharacterEquippableItems");
        migrationBuilder.DropColumn("IsEquipped", "ScenePlaythroughCharacterEquippableItems");
        migrationBuilder.DropColumn("SnapshotEventKey", "ScenePlaythroughEvents");
        migrationBuilder.DropColumn("SnapshotChestKey", "ScenePlaythroughChests");
        migrationBuilder.DropColumn("SelectedLootEntrySnapshotKey", "ScenePlaythroughChests");
        migrationBuilder.DropTable("JourneyRevisions");
    }

    private static void AlterSourceColumns(MigrationBuilder migrationBuilder, bool nullable)
    {
        AlterInt(migrationBuilder, "JourneyId", "JourneyPlaythroughs", nullable);
        AlterInt(migrationBuilder, "JourneyCharacterId", "JourneyPlaythroughCharacters", nullable);
        AlterInt(migrationBuilder, "JourneyCharacterSpellId", "JourneyPlaythroughCharacterSpells", nullable);
        AlterInt(migrationBuilder, "ConsumableItemId", "JourneyPlaythroughCharacterConsumableItems", nullable);
        AlterInt(migrationBuilder, "EquippableItemId", "JourneyPlaythroughCharacterEquippableItems", nullable);
        AlterInt(migrationBuilder, "SceneId", "ScenePlaythroughs", nullable);
        AlterInt(migrationBuilder, "SceneCharacterId", "ScenePlaythroughCharacters", nullable);
        AlterInt(migrationBuilder, "SceneCharacterSpellId", "ScenePlaythroughCharacterSpells", nullable);
        AlterInt(migrationBuilder, "ConsumableItemId", "ScenePlaythroughCharacterConsumableItems", nullable);
        AlterInt(migrationBuilder, "EquippableItemId", "ScenePlaythroughCharacterEquippableItems", nullable);
        AlterInt(migrationBuilder, "SceneEventId", "ScenePlaythroughEvents", nullable);
        AlterInt(migrationBuilder, "SceneChestId", "ScenePlaythroughChests", nullable);
    }

    private static void AlterInt(MigrationBuilder migrationBuilder, string column, string table, bool nullable) =>
        migrationBuilder.AlterColumn<int>(column, table, type: "int", nullable: nullable,
            oldClrType: typeof(int), oldType: "int", oldNullable: !nullable);

    private static void DropSourceIndexes(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex("IX_JourneyPlaythroughs_JourneyId", "JourneyPlaythroughs");
        migrationBuilder.DropIndex("IX_JourneyPlaythroughCharacters_JourneyPlaythroughId_JourneyCharacterId", "JourneyPlaythroughCharacters");
        migrationBuilder.DropIndex("IX_JourneyPlaythroughCharacterSpells_JourneyPlaythroughCharacterId_JourneyCharacterSpellId", "JourneyPlaythroughCharacterSpells");
        migrationBuilder.DropIndex("IX_ScenePlaythroughs_JourneyPlaythroughId_SceneId", "ScenePlaythroughs");
        migrationBuilder.DropIndex("IX_ScenePlaythroughCharacters_ScenePlaythroughId_SceneCharacterId", "ScenePlaythroughCharacters");
        migrationBuilder.DropIndex("IX_ScenePlaythroughCharacterSpells_ScenePlaythroughCharacterId_SceneCharacterSpellId", "ScenePlaythroughCharacterSpells");
        migrationBuilder.DropIndex("IX_ScenePlaythroughEvents_ScenePlaythroughId_SceneEventId", "ScenePlaythroughEvents");
        migrationBuilder.DropIndex("IX_ScenePlaythroughChests_ScenePlaythroughId_SceneChestId", "ScenePlaythroughChests");
    }

    private static void DropSnapshotIndexes(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex("IX_JourneyPlaythroughs_SourceJourneyId", "JourneyPlaythroughs");
        migrationBuilder.DropIndex("IX_JourneyPlaythroughs_JourneyId", "JourneyPlaythroughs");
        migrationBuilder.DropIndex("IX_JourneyPlaythroughCharacters_JourneyPlaythroughId_SnapshotAssignmentKey", "JourneyPlaythroughCharacters");
        migrationBuilder.DropIndex("IX_JourneyPlaythroughCharacterSpells_JourneyPlaythroughCharacterId_SnapshotSpellKey", "JourneyPlaythroughCharacterSpells");
        migrationBuilder.DropIndex("IX_ScenePlaythroughs_JourneyPlaythroughId_SnapshotSceneKey", "ScenePlaythroughs");
        migrationBuilder.DropIndex("IX_ScenePlaythroughCharacters_ScenePlaythroughId_SnapshotAssignmentKey", "ScenePlaythroughCharacters");
        migrationBuilder.DropIndex("IX_ScenePlaythroughCharacterSpells_ScenePlaythroughCharacterId_SnapshotSpellKey", "ScenePlaythroughCharacterSpells");
        migrationBuilder.DropIndex("IX_ScenePlaythroughEvents_ScenePlaythroughId_SnapshotEventKey", "ScenePlaythroughEvents");
        migrationBuilder.DropIndex("IX_ScenePlaythroughChests_ScenePlaythroughId_SnapshotChestKey", "ScenePlaythroughChests");
    }

    private static void DropSourceForeignKeys(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey("FK_JourneyPlaythroughs_Journeys_JourneyId", "JourneyPlaythroughs");
        migrationBuilder.DropForeignKey("FK_JourneyPlaythroughCharacters_JourneyCharacters_JourneyCharacterId", "JourneyPlaythroughCharacters");
        migrationBuilder.DropForeignKey("FK_JourneyPlaythroughCharacterSpells_JourneyCharacterSpells_JourneyCharacterSpellId", "JourneyPlaythroughCharacterSpells");
        migrationBuilder.DropForeignKey("FK_JourneyPlaythroughCharacterConsumableItems_ConsumableItems_ConsumableItemId", "JourneyPlaythroughCharacterConsumableItems");
        migrationBuilder.DropForeignKey("FK_JourneyPlaythroughCharacterEquippableItems_EquippableItems_EquippableItemId", "JourneyPlaythroughCharacterEquippableItems");
        migrationBuilder.DropForeignKey("FK_ScenePlaythroughs_Scenes_SceneId", "ScenePlaythroughs");
        migrationBuilder.DropForeignKey("FK_ScenePlaythroughCharacters_SceneCharacters_SceneCharacterId", "ScenePlaythroughCharacters");
        migrationBuilder.DropForeignKey("FK_ScenePlaythroughCharacterSpells_SceneCharacterSpells_SceneCharacterSpellId", "ScenePlaythroughCharacterSpells");
        migrationBuilder.DropForeignKey("FK_ScenePlaythroughCharacterConsumableItems_ConsumableItems_ConsumableItemId", "ScenePlaythroughCharacterConsumableItems");
        migrationBuilder.DropForeignKey("FK_ScenePlaythroughCharacterEquippableItems_EquippableItems_EquippableItemId", "ScenePlaythroughCharacterEquippableItems");
        migrationBuilder.DropForeignKey("FK_ScenePlaythroughEvents_SceneEvents_SceneEventId", "ScenePlaythroughEvents");
        migrationBuilder.DropForeignKey("FK_ScenePlaythroughChests_SceneChests_SceneChestId", "ScenePlaythroughChests");
        migrationBuilder.DropForeignKey("FK_ScenePlaythroughChests_SceneChestLootEntries_SelectedLootEntryId", "ScenePlaythroughChests");
    }

    private static void AddSourceForeignKeys(MigrationBuilder migrationBuilder, ReferentialAction action, ReferentialAction? journeyDelete = null)
    {
        migrationBuilder.AddForeignKey("FK_JourneyPlaythroughs_Journeys_JourneyId", "JourneyPlaythroughs", "JourneyId", "Journeys", principalColumn: "Id", onDelete: journeyDelete ?? action);
        migrationBuilder.AddForeignKey("FK_JourneyPlaythroughCharacters_JourneyCharacters_JourneyCharacterId", "JourneyPlaythroughCharacters", "JourneyCharacterId", "JourneyCharacters", principalColumn: "Id", onDelete: action);
        migrationBuilder.AddForeignKey("FK_JourneyPlaythroughCharacterSpells_JourneyCharacterSpells_JourneyCharacterSpellId", "JourneyPlaythroughCharacterSpells", "JourneyCharacterSpellId", "JourneyCharacterSpells", principalColumn: "Id", onDelete: action);
        migrationBuilder.AddForeignKey("FK_JourneyPlaythroughCharacterConsumableItems_ConsumableItems_ConsumableItemId", "JourneyPlaythroughCharacterConsumableItems", "ConsumableItemId", "ConsumableItems", principalColumn: "Id", onDelete: action);
        migrationBuilder.AddForeignKey("FK_JourneyPlaythroughCharacterEquippableItems_EquippableItems_EquippableItemId", "JourneyPlaythroughCharacterEquippableItems", "EquippableItemId", "EquippableItems", principalColumn: "Id", onDelete: action);
        migrationBuilder.AddForeignKey("FK_ScenePlaythroughs_Scenes_SceneId", "ScenePlaythroughs", "SceneId", "Scenes", principalColumn: "Id", onDelete: action);
        migrationBuilder.AddForeignKey("FK_ScenePlaythroughCharacters_SceneCharacters_SceneCharacterId", "ScenePlaythroughCharacters", "SceneCharacterId", "SceneCharacters", principalColumn: "Id", onDelete: action);
        migrationBuilder.AddForeignKey("FK_ScenePlaythroughCharacterSpells_SceneCharacterSpells_SceneCharacterSpellId", "ScenePlaythroughCharacterSpells", "SceneCharacterSpellId", "SceneCharacterSpells", principalColumn: "Id", onDelete: action);
        migrationBuilder.AddForeignKey("FK_ScenePlaythroughCharacterConsumableItems_ConsumableItems_ConsumableItemId", "ScenePlaythroughCharacterConsumableItems", "ConsumableItemId", "ConsumableItems", principalColumn: "Id", onDelete: action);
        migrationBuilder.AddForeignKey("FK_ScenePlaythroughCharacterEquippableItems_EquippableItems_EquippableItemId", "ScenePlaythroughCharacterEquippableItems", "EquippableItemId", "EquippableItems", principalColumn: "Id", onDelete: action);
        migrationBuilder.AddForeignKey("FK_ScenePlaythroughEvents_SceneEvents_SceneEventId", "ScenePlaythroughEvents", "SceneEventId", "SceneEvents", principalColumn: "Id", onDelete: action);
        migrationBuilder.AddForeignKey("FK_ScenePlaythroughChests_SceneChests_SceneChestId", "ScenePlaythroughChests", "SceneChestId", "SceneChests", principalColumn: "Id", onDelete: action);
        migrationBuilder.AddForeignKey("FK_ScenePlaythroughChests_SceneChestLootEntries_SelectedLootEntryId", "ScenePlaythroughChests", "SelectedLootEntryId", "SceneChestLootEntries", principalColumn: "Id", onDelete: ReferentialAction.NoAction);
    }
}
