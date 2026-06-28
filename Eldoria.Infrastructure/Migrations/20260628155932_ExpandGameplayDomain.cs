using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eldoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExpandGameplayDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Characters_AlternateFormId",
                table: "Characters");

            migrationBuilder.DropIndex(
                name: "IX_Characters_AlternateFormId",
                table: "Characters");

            migrationBuilder.RenameColumn(
                name: "IsAlternateForm",
                table: "JourneyCharacters",
                newName: "IsInAlternateForm");

            migrationBuilder.RenameColumn(
                name: "Movement",
                table: "Characters",
                newName: "BaseMovement");

            migrationBuilder.RenameColumn(
                name: "MeleeAttackDamage",
                table: "Characters",
                newName: "BaseMeleeAttackDamage");

            migrationBuilder.RenameColumn(
                name: "MaxMp",
                table: "Characters",
                newName: "BaseMaxMp");

            migrationBuilder.RenameColumn(
                name: "MaxInventory",
                table: "Characters",
                newName: "BaseMaxConsumableInventory");

            migrationBuilder.RenameColumn(
                name: "MaxHp",
                table: "Characters",
                newName: "BaseMaxHp");

            migrationBuilder.RenameColumn(
                name: "BowAttackDamage",
                table: "Characters",
                newName: "BaseBowAttackDamage");

            migrationBuilder.RenameColumn(
                name: "AlternateFormId",
                table: "Characters",
                newName: "BaseAlternateFormId");

            migrationBuilder.AddColumn<int>(
                name: "SpellTypeId",
                table: "Spells",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Spells",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Scenes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AlternateFormId",
                table: "JourneyCharacters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BowAttackDamage",
                table: "JourneyCharacters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxConsumableInventory",
                table: "JourneyCharacters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxEquippableInventory",
                table: "JourneyCharacters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxHp",
                table: "JourneyCharacters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxMp",
                table: "JourneyCharacters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MeleeAttackDamage",
                table: "JourneyCharacters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Movement",
                table: "JourneyCharacters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Items",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Characters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BaseMaxEquippableInventory",
                table: "Characters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Portrait",
                table: "Characters",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PortraitFileName",
                table: "Characters",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "JourneyCharacterSpells",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JourneyCharacterId = table.Column<int>(type: "int", nullable: false),
                    SpellId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JourneyCharacterSpells", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JourneyCharacterSpells_JourneyCharacters_JourneyCharacterId",
                        column: x => x.JourneyCharacterId,
                        principalTable: "JourneyCharacters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JourneyCharacterSpells_Spells_SpellId",
                        column: x => x.SpellId,
                        principalTable: "Spells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JourneyPlaythroughs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    JourneyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JourneyPlaythroughs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JourneyPlaythroughs_Journeys_JourneyId",
                        column: x => x.JourneyId,
                        principalTable: "Journeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpellTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpellTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpellTypes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "SceneProgresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SceneId = table.Column<int>(type: "int", nullable: false),
                    JourneyPlaythroughId = table.Column<int>(type: "int", nullable: false),
                    SceneProgressStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SceneProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SceneProgresses_JourneyPlaythroughs_JourneyPlaythroughId",
                        column: x => x.JourneyPlaythroughId,
                        principalTable: "JourneyPlaythroughs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SceneProgresses_Scenes_SceneId",
                        column: x => x.SceneId,
                        principalTable: "Scenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquippableItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    MeleeAttackDamageModifier = table.Column<int>(type: "int", nullable: false),
                    BowAttackDamageModifier = table.Column<int>(type: "int", nullable: false),
                    MovementModifier = table.Column<int>(type: "int", nullable: false),
                    MaxHpModifier = table.Column<int>(type: "int", nullable: false),
                    MaxMpModifier = table.Column<int>(type: "int", nullable: false),
                    MaxConsumableInventoryModifier = table.Column<int>(type: "int", nullable: false),
                    MaxEquippableInventoryModifier = table.Column<int>(type: "int", nullable: false),
                    MeleeDamageReduction = table.Column<int>(type: "int", nullable: false),
                    BowDamageReduction = table.Column<int>(type: "int", nullable: false),
                    SpellDamageReduction = table.Column<int>(type: "int", nullable: false),
                    AffectedSpellTypeId = table.Column<int>(type: "int", nullable: true),
                    SpellDamageModifier = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquippableItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquippableItems_SpellTypes_AffectedSpellTypeId",
                        column: x => x.AffectedSpellTypeId,
                        principalTable: "SpellTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquippableItems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.Sql(
                """
                DECLARE @DefaultSpellTypeId int;
                INSERT INTO [SpellTypes] ([TypeName], [UserId])
                VALUES (N'Uncategorized', NULL);
                SET @DefaultSpellTypeId = SCOPE_IDENTITY();
                UPDATE [Spells] SET [SpellTypeId] = @DefaultSpellTypeId;
                """);

            migrationBuilder.Sql(
                """
                UPDATE jc
                SET
                    jc.[MaxHp] = c.[BaseMaxHp],
                    jc.[MaxMp] = c.[BaseMaxMp],
                    jc.[MeleeAttackDamage] = c.[BaseMeleeAttackDamage],
                    jc.[BowAttackDamage] = c.[BaseBowAttackDamage],
                    jc.[Movement] = c.[BaseMovement],
                    jc.[MaxConsumableInventory] = c.[BaseMaxConsumableInventory],
                    jc.[MaxEquippableInventory] = c.[BaseMaxEquippableInventory],
                    jc.[AlternateFormId] = c.[BaseAlternateFormId]
                FROM [JourneyCharacters] jc
                INNER JOIN [Characters] c ON c.[Id] = jc.[CharacterId];

                UPDATE [Characters]
                SET [BaseMaxEquippableInventory] = [BaseMaxConsumableInventory];
                """);

            migrationBuilder.CreateTable(
                name: "SceneParticipants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SceneProgressId = table.Column<int>(type: "int", nullable: false),
                    JourneyCharacterId = table.Column<int>(type: "int", nullable: true),
                    SceneCharacterId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SceneParticipants", x => x.Id);
                    table.UniqueConstraint("AK_SceneParticipants_Id_SceneProgressId", x => new { x.Id, x.SceneProgressId });
                    table.CheckConstraint("CK_SceneParticipants_Character", "([JourneyCharacterId] IS NOT NULL AND [SceneCharacterId] IS NULL) OR ([JourneyCharacterId] IS NULL AND [SceneCharacterId] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_SceneParticipants_JourneyCharacters_JourneyCharacterId",
                        column: x => x.JourneyCharacterId,
                        principalTable: "JourneyCharacters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SceneParticipants_SceneCharacters_SceneCharacterId",
                        column: x => x.SceneCharacterId,
                        principalTable: "SceneCharacters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SceneParticipants_SceneProgresses_SceneProgressId",
                        column: x => x.SceneProgressId,
                        principalTable: "SceneProgresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EquippableItemSpells",
                columns: table => new
                {
                    AddedSpellsId = table.Column<int>(type: "int", nullable: false),
                    EquippableItemsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquippableItemSpells", x => new { x.AddedSpellsId, x.EquippableItemsId });
                    table.ForeignKey(
                        name: "FK_EquippableItemSpells_EquippableItems_EquippableItemsId",
                        column: x => x.EquippableItemsId,
                        principalTable: "EquippableItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquippableItemSpells_Spells_AddedSpellsId",
                        column: x => x.AddedSpellsId,
                        principalTable: "Spells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JourneyCharacterEquippableItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsEquipped = table.Column<bool>(type: "bit", nullable: false),
                    JourneyCharacterId = table.Column<int>(type: "int", nullable: false),
                    EquippableItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JourneyCharacterEquippableItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JourneyCharacterEquippableItems_EquippableItems_EquippableItemId",
                        column: x => x.EquippableItemId,
                        principalTable: "EquippableItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JourneyCharacterEquippableItems_JourneyCharacters_JourneyCharacterId",
                        column: x => x.JourneyCharacterId,
                        principalTable: "JourneyCharacters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SceneParticipantTurns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SceneProgressId = table.Column<int>(type: "int", nullable: false),
                    SceneParticipantId = table.Column<int>(type: "int", nullable: false),
                    TurnPosition = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SceneParticipantTurns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SceneParticipantTurns_SceneParticipants_SceneParticipantId_SceneProgressId",
                        columns: x => new { x.SceneParticipantId, x.SceneProgressId },
                        principalTable: "SceneParticipants",
                        principalColumns: new[] { "Id", "SceneProgressId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SceneParticipantTurns_SceneProgresses_SceneProgressId",
                        column: x => x.SceneProgressId,
                        principalTable: "SceneProgresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Spells_SpellTypeId",
                table: "Spells",
                column: "SpellTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Spells_UserId",
                table: "Spells",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyCharacters_AlternateFormId",
                table: "JourneyCharacters",
                column: "AlternateFormId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_UserId",
                table: "Items",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_BaseAlternateFormId",
                table: "Characters",
                column: "BaseAlternateFormId",
                unique: true,
                filter: "[BaseAlternateFormId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_UserId",
                table: "Characters",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EquippableItems_AffectedSpellTypeId",
                table: "EquippableItems",
                column: "AffectedSpellTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EquippableItems_UserId",
                table: "EquippableItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EquippableItemSpells_EquippableItemsId",
                table: "EquippableItemSpells",
                column: "EquippableItemsId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyCharacterEquippableItems_EquippableItemId",
                table: "JourneyCharacterEquippableItems",
                column: "EquippableItemId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyCharacterEquippableItems_JourneyCharacterId",
                table: "JourneyCharacterEquippableItems",
                column: "JourneyCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyCharacterSpells_JourneyCharacterId_SpellId",
                table: "JourneyCharacterSpells",
                columns: new[] { "JourneyCharacterId", "SpellId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JourneyCharacterSpells_SpellId",
                table: "JourneyCharacterSpells",
                column: "SpellId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyPlaythroughs_JourneyId",
                table: "JourneyPlaythroughs",
                column: "JourneyId",
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_SceneParticipants_JourneyCharacterId",
                table: "SceneParticipants",
                column: "JourneyCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_SceneParticipants_SceneCharacterId",
                table: "SceneParticipants",
                column: "SceneCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_SceneParticipants_SceneProgressId_JourneyCharacterId",
                table: "SceneParticipants",
                columns: new[] { "SceneProgressId", "JourneyCharacterId" },
                unique: true,
                filter: "[JourneyCharacterId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SceneParticipants_SceneProgressId_SceneCharacterId",
                table: "SceneParticipants",
                columns: new[] { "SceneProgressId", "SceneCharacterId" },
                unique: true,
                filter: "[SceneCharacterId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SceneParticipantTurns_SceneParticipantId_SceneProgressId",
                table: "SceneParticipantTurns",
                columns: new[] { "SceneParticipantId", "SceneProgressId" });

            migrationBuilder.CreateIndex(
                name: "IX_SceneParticipantTurns_SceneProgressId_TurnPosition",
                table: "SceneParticipantTurns",
                columns: new[] { "SceneProgressId", "TurnPosition" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SceneProgresses_JourneyPlaythroughId_SceneId",
                table: "SceneProgresses",
                columns: new[] { "JourneyPlaythroughId", "SceneId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SceneProgresses_SceneId",
                table: "SceneProgresses",
                column: "SceneId");

            migrationBuilder.CreateIndex(
                name: "IX_SpellTypes_UserId_TypeName",
                table: "SpellTypes",
                columns: new[] { "UserId", "TypeName" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Characters_BaseAlternateFormId",
                table: "Characters",
                column: "BaseAlternateFormId",
                principalTable: "Characters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Users_UserId",
                table: "Characters",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Users_UserId",
                table: "Items",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_JourneyCharacters_Characters_AlternateFormId",
                table: "JourneyCharacters",
                column: "AlternateFormId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Spells_SpellTypes_SpellTypeId",
                table: "Spells",
                column: "SpellTypeId",
                principalTable: "SpellTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Spells_Users_UserId",
                table: "Spells",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Characters_BaseAlternateFormId",
                table: "Characters");

            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Users_UserId",
                table: "Characters");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Users_UserId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_JourneyCharacters_Characters_AlternateFormId",
                table: "JourneyCharacters");

            migrationBuilder.DropForeignKey(
                name: "FK_Spells_SpellTypes_SpellTypeId",
                table: "Spells");

            migrationBuilder.DropForeignKey(
                name: "FK_Spells_Users_UserId",
                table: "Spells");

            migrationBuilder.DropTable(
                name: "EquippableItemSpells");

            migrationBuilder.DropTable(
                name: "JourneyCharacterEquippableItems");

            migrationBuilder.DropTable(
                name: "JourneyCharacterSpells");

            migrationBuilder.DropTable(
                name: "SceneParticipantTurns");

            migrationBuilder.DropTable(
                name: "EquippableItems");

            migrationBuilder.DropTable(
                name: "SceneParticipants");

            migrationBuilder.DropTable(
                name: "SpellTypes");

            migrationBuilder.DropTable(
                name: "SceneProgresses");

            migrationBuilder.DropTable(
                name: "JourneyPlaythroughs");

            migrationBuilder.DropIndex(
                name: "IX_Spells_SpellTypeId",
                table: "Spells");

            migrationBuilder.DropIndex(
                name: "IX_Spells_UserId",
                table: "Spells");

            migrationBuilder.DropIndex(
                name: "IX_JourneyCharacters_AlternateFormId",
                table: "JourneyCharacters");

            migrationBuilder.DropIndex(
                name: "IX_Items_UserId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Characters_BaseAlternateFormId",
                table: "Characters");

            migrationBuilder.DropIndex(
                name: "IX_Characters_UserId",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "SpellTypeId",
                table: "Spells");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Spells");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Scenes");

            migrationBuilder.DropColumn(
                name: "AlternateFormId",
                table: "JourneyCharacters");

            migrationBuilder.DropColumn(
                name: "BowAttackDamage",
                table: "JourneyCharacters");

            migrationBuilder.DropColumn(
                name: "MaxConsumableInventory",
                table: "JourneyCharacters");

            migrationBuilder.DropColumn(
                name: "MaxEquippableInventory",
                table: "JourneyCharacters");

            migrationBuilder.DropColumn(
                name: "MaxHp",
                table: "JourneyCharacters");

            migrationBuilder.DropColumn(
                name: "MaxMp",
                table: "JourneyCharacters");

            migrationBuilder.DropColumn(
                name: "MeleeAttackDamage",
                table: "JourneyCharacters");

            migrationBuilder.DropColumn(
                name: "Movement",
                table: "JourneyCharacters");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "BaseMaxEquippableInventory",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Portrait",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "PortraitFileName",
                table: "Characters");

            migrationBuilder.RenameColumn(
                name: "IsInAlternateForm",
                table: "JourneyCharacters",
                newName: "IsAlternateForm");

            migrationBuilder.RenameColumn(
                name: "BaseMovement",
                table: "Characters",
                newName: "Movement");

            migrationBuilder.RenameColumn(
                name: "BaseMaxMp",
                table: "Characters",
                newName: "MaxMp");

            migrationBuilder.RenameColumn(
                name: "BaseMeleeAttackDamage",
                table: "Characters",
                newName: "MeleeAttackDamage");

            migrationBuilder.RenameColumn(
                name: "BaseMaxConsumableInventory",
                table: "Characters",
                newName: "MaxInventory");

            migrationBuilder.RenameColumn(
                name: "BaseMaxHp",
                table: "Characters",
                newName: "MaxHp");

            migrationBuilder.RenameColumn(
                name: "BaseBowAttackDamage",
                table: "Characters",
                newName: "BowAttackDamage");

            migrationBuilder.RenameColumn(
                name: "BaseAlternateFormId",
                table: "Characters",
                newName: "AlternateFormId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_AlternateFormId",
                table: "Characters",
                column: "AlternateFormId",
                unique: true,
                filter: "[AlternateFormId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Characters_AlternateFormId",
                table: "Characters",
                column: "AlternateFormId",
                principalTable: "Characters",
                principalColumn: "Id");
        }
    }
}
