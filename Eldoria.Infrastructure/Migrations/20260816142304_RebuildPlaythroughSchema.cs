using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eldoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RebuildPlaythroughSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScenePlaythroughParticipants_JourneyPlaythroughCharacters_JourneyPlaythroughCharacterId",
                table: "ScenePlaythroughParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_ScenePlaythroughs_JourneyPlaythroughs_JourneyPlaythroughId",
                table: "ScenePlaythroughs");

            migrationBuilder.DropForeignKey(
                name: "FK_ScenePlaythroughParticipants_ScenePlaythroughCharacters_ScenePlaythroughCharacterId",
                table: "ScenePlaythroughParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_ScenePlaythroughParticipants_ScenePlaythroughs_ScenePlaythroughId",
                table: "ScenePlaythroughParticipants");

            migrationBuilder.DropTable(
                name: "DialogPageSections");

            migrationBuilder.DropTable(
                name: "JourneyPlaythroughCharacterConsumableItems");

            migrationBuilder.DropTable(
                name: "JourneyPlaythroughCharacterEquippableItems");

            migrationBuilder.DropTable(
                name: "JourneyPlaythroughCharacterSpells");

            migrationBuilder.DropTable(
                name: "JourneyPlaythroughEventLogs");

            migrationBuilder.DropTable(
                name: "ScenePlaythroughCharacterConsumableItems");

            migrationBuilder.DropTable(
                name: "ScenePlaythroughCharacterEquippableItems");

            migrationBuilder.DropTable(
                name: "ScenePlaythroughCharacterSpells");

            migrationBuilder.DropTable(
                name: "ScenePlaythroughChests");

            migrationBuilder.DropTable(
                name: "ScenePlaythroughEvents");

            migrationBuilder.DropTable(
                name: "DialogPages");

            migrationBuilder.DropTable(
                name: "JourneyPlaythroughCharacters");

            migrationBuilder.DropTable(
                name: "JourneyPlaythroughs");

            migrationBuilder.DropTable(
                name: "JourneyRevisions");

            migrationBuilder.DropTable(
                name: "ScenePlaythroughCharacters");

            migrationBuilder.DropTable(
                name: "ScenePlaythroughs");

            migrationBuilder.DropTable(
                name: "ScenePlaythroughParticipants");

            migrationBuilder.CreateTable(
                name: "CharacterAddSpellAction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharacterId = table.Column<int>(type: "int", nullable: true),
                    SpellId = table.Column<int>(type: "int", nullable: false),
                    SceneEventActionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterAddSpellAction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterAddSpellAction_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CharacterAddSpellAction_SceneEventActions_SceneEventActionId",
                        column: x => x.SceneEventActionId,
                        principalTable: "SceneEventActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterAddSpellAction_Spells_SpellId",
                        column: x => x.SpellId,
                        principalTable: "Spells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Playthroughs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceJourneyId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playthroughs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Playthroughs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ScenePTDialogPage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNum = table.Column<int>(type: "int", nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SceneDialogId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePTDialogPage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenePTDialogPage_SceneDialogs_SceneDialogId",
                        column: x => x.SceneDialogId,
                        principalTable: "SceneDialogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaythroughCharacters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceCharacterId = table.Column<int>(type: "int", nullable: false),
                    CharacterType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PortraitUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    PortraitFileName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    BaseMaxHp = table.Column<int>(type: "int", nullable: false),
                    BaseMaxMp = table.Column<int>(type: "int", nullable: false),
                    BaseMeleeAttackDamage = table.Column<int>(type: "int", nullable: true),
                    BaseBowAttackDamage = table.Column<int>(type: "int", nullable: true),
                    BaseMovement = table.Column<int>(type: "int", nullable: false),
                    BaseMaxConsumableInventory = table.Column<int>(type: "int", nullable: false),
                    BaseMaxEquippableInventory = table.Column<int>(type: "int", nullable: false),
                    BaseAlternateFormId = table.Column<int>(type: "int", nullable: true),
                    DialogActiveColor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DialogInActiveColor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PlaythroughId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaythroughCharacters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaythroughCharacters_PlaythroughCharacters_BaseAlternateFormId",
                        column: x => x.BaseAlternateFormId,
                        principalTable: "PlaythroughCharacters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PlaythroughCharacters_Playthroughs_PlaythroughId",
                        column: x => x.PlaythroughId,
                        principalTable: "Playthroughs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaythroughConsumableItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceConsumableItemId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    HpEffect = table.Column<int>(type: "int", nullable: false),
                    MpEffect = table.Column<int>(type: "int", nullable: false),
                    PlaythroughId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaythroughConsumableItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaythroughConsumableItems_Playthroughs_PlaythroughId",
                        column: x => x.PlaythroughId,
                        principalTable: "Playthroughs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaythroughEventLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    EventTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlaythroughId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaythroughEventLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaythroughEventLogs_Playthroughs_PlaythroughId",
                        column: x => x.PlaythroughId,
                        principalTable: "Playthroughs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaythroughIntroPages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceIntroPageId = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Config = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreviewPhotoUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    PlaythroughId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaythroughIntroPages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaythroughIntroPages_Playthroughs_PlaythroughId",
                        column: x => x.PlaythroughId,
                        principalTable: "Playthroughs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaythroughSpellTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceSpellTypeId = table.Column<int>(type: "int", nullable: false),
                    TypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PlaythroughId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaythroughSpellTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaythroughSpellTypes_Playthroughs_PlaythroughId",
                        column: x => x.PlaythroughId,
                        principalTable: "Playthroughs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenePTDialogSection",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNum = table.Column<int>(type: "int", nullable: false),
                    ReadingText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsNarrator = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CharacterId = table.Column<int>(type: "int", nullable: true),
                    DialogPageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePTDialogSection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenePTDialogSection_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ScenePTDialogSection_ScenePTDialogPage_DialogPageId",
                        column: x => x.DialogPageId,
                        principalTable: "ScenePTDialogPage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JourneyPTCharacters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceJourneyCharacterId = table.Column<int>(type: "int", nullable: false),
                    InitialMeleeAttackDamage = table.Column<int>(type: "int", nullable: true),
                    InitialBowAttackDamage = table.Column<int>(type: "int", nullable: true),
                    InitialMovement = table.Column<int>(type: "int", nullable: false),
                    InitialMaxConsumableInventory = table.Column<int>(type: "int", nullable: false),
                    InitialMaxEquippableInventory = table.Column<int>(type: "int", nullable: false),
                    InitialMaxHp = table.Column<int>(type: "int", nullable: false),
                    InitialMaxMp = table.Column<int>(type: "int", nullable: false),
                    IsInitiallyActive = table.Column<bool>(type: "bit", nullable: false),
                    MeleeAttackDamage = table.Column<int>(type: "int", nullable: true),
                    BowAttackDamage = table.Column<int>(type: "int", nullable: true),
                    Movement = table.Column<int>(type: "int", nullable: false),
                    MaxConsumableInventory = table.Column<int>(type: "int", nullable: false),
                    MaxEquippableInventory = table.Column<int>(type: "int", nullable: false),
                    CurrentHp = table.Column<int>(type: "int", nullable: false),
                    CurrentMp = table.Column<int>(type: "int", nullable: false),
                    MaxHp = table.Column<int>(type: "int", nullable: false),
                    MaxMp = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDown = table.Column<bool>(type: "bit", nullable: false),
                    IsInAlternateForm = table.Column<bool>(type: "bit", nullable: false),
                    AlternateFormId = table.Column<int>(type: "int", nullable: true),
                    PlaythroughId = table.Column<int>(type: "int", nullable: false),
                    PlaythroughCharacterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JourneyPTCharacters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JourneyPTCharacters_PlaythroughCharacters_AlternateFormId",
                        column: x => x.AlternateFormId,
                        principalTable: "PlaythroughCharacters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JourneyPTCharacters_PlaythroughCharacters_PlaythroughCharacterId",
                        column: x => x.PlaythroughCharacterId,
                        principalTable: "PlaythroughCharacters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JourneyPTCharacters_Playthroughs_PlaythroughId",
                        column: x => x.PlaythroughId,
                        principalTable: "Playthroughs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaythroughEquippableItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceEquippableItemId = table.Column<int>(type: "int", nullable: false),
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
                    SpellDamageModifier = table.Column<int>(type: "int", nullable: true),
                    PlaythroughId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaythroughEquippableItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaythroughEquippableItems_PlaythroughSpellTypes_AffectedSpellTypeId",
                        column: x => x.AffectedSpellTypeId,
                        principalTable: "PlaythroughSpellTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PlaythroughEquippableItems_Playthroughs_PlaythroughId",
                        column: x => x.PlaythroughId,
                        principalTable: "Playthroughs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaythroughSpells",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceSpellId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Range = table.Column<int>(type: "int", nullable: false),
                    IsRadius = table.Column<bool>(type: "bit", nullable: false),
                    MpCost = table.Column<int>(type: "int", nullable: false),
                    DamageEffect = table.Column<int>(type: "int", nullable: true),
                    HealthEffect = table.Column<int>(type: "int", nullable: true),
                    MagicEffect = table.Column<int>(type: "int", nullable: true),
                    PlaythroughSpellTypeId = table.Column<int>(type: "int", nullable: false),
                    PlaythroughId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaythroughSpells", x => x.Id);
                    table.CheckConstraint("CK_PlaythroughSpells_MpCost", "[MpCost] >= 0");
                    table.CheckConstraint("CK_PlaythroughSpells_Range", "[Range] >= 0");
                    table.ForeignKey(
                        name: "FK_PlaythroughSpells_PlaythroughSpellTypes_PlaythroughSpellTypeId",
                        column: x => x.PlaythroughSpellTypeId,
                        principalTable: "PlaythroughSpellTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PlaythroughSpells_Playthroughs_PlaythroughId",
                        column: x => x.PlaythroughId,
                        principalTable: "Playthroughs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JourneyPTCharacterConsumableItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    JourneyPTCharacterId = table.Column<int>(type: "int", nullable: false),
                    PlaythroughConsumableItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JourneyPTCharacterConsumableItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JourneyPTCharacterConsumableItems_JourneyPTCharacters_JourneyPTCharacterId",
                        column: x => x.JourneyPTCharacterId,
                        principalTable: "JourneyPTCharacters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JourneyPTCharacterConsumableItems_PlaythroughConsumableItems_PlaythroughConsumableItemId",
                        column: x => x.PlaythroughConsumableItemId,
                        principalTable: "PlaythroughConsumableItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JourneyPTCharacterEquippableItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsEquipped = table.Column<bool>(type: "bit", nullable: false),
                    JourneyPTCharacterId = table.Column<int>(type: "int", nullable: false),
                    PlaythroughEquippableItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JourneyPTCharacterEquippableItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JourneyPTCharacterEquippableItems_JourneyPTCharacters_JourneyPTCharacterId",
                        column: x => x.JourneyPTCharacterId,
                        principalTable: "JourneyPTCharacters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JourneyPTCharacterEquippableItems_PlaythroughEquippableItems_PlaythroughEquippableItemId",
                        column: x => x.PlaythroughEquippableItemId,
                        principalTable: "PlaythroughEquippableItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JourneyPTCharacterSpells",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceJourneyCharacterSpellId = table.Column<int>(type: "int", nullable: true),
                    JourneyPTCharacterId = table.Column<int>(type: "int", nullable: false),
                    PlaythroughSpellId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JourneyPTCharacterSpells", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JourneyPTCharacterSpells_JourneyPTCharacters_JourneyPTCharacterId",
                        column: x => x.JourneyPTCharacterId,
                        principalTable: "JourneyPTCharacters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JourneyPTCharacterSpells_PlaythroughSpells_PlaythroughSpellId",
                        column: x => x.PlaythroughSpellId,
                        principalTable: "PlaythroughSpells",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PlaythroughCharacterSpells",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceCharacterSpellId = table.Column<int>(type: "int", nullable: false),
                    PlaythroughCharacterId = table.Column<int>(type: "int", nullable: false),
                    PlaythroughSpellId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaythroughCharacterSpells", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaythroughCharacterSpells_PlaythroughCharacters_PlaythroughCharacterId",
                        column: x => x.PlaythroughCharacterId,
                        principalTable: "PlaythroughCharacters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaythroughCharacterSpells_PlaythroughSpells_PlaythroughSpellId",
                        column: x => x.PlaythroughSpellId,
                        principalTable: "PlaythroughSpells",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PlaythroughEquippableItemSpells",
                columns: table => new
                {
                    PlaythroughEquippableItemId = table.Column<int>(type: "int", nullable: false),
                    PlaythroughSpellId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaythroughEquippableItemSpells", x => new { x.PlaythroughEquippableItemId, x.PlaythroughSpellId });
                    table.ForeignKey(
                        name: "FK_PlaythroughEquippableItemSpells_PlaythroughEquippableItems_PlaythroughEquippableItemId",
                        column: x => x.PlaythroughEquippableItemId,
                        principalTable: "PlaythroughEquippableItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaythroughEquippableItemSpells_PlaythroughSpells_PlaythroughSpellId",
                        column: x => x.PlaythroughSpellId,
                        principalTable: "PlaythroughSpells",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PTCharacterAddSpellActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceCharacterAddSpellActionId = table.Column<int>(type: "int", nullable: false),
                    PlaythroughCharacterId = table.Column<int>(type: "int", nullable: true),
                    PlaythroughSpellId = table.Column<int>(type: "int", nullable: false),
                    ScenePTActionEventId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PTCharacterAddSpellActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PTCharacterAddSpellActions_PlaythroughCharacters_PlaythroughCharacterId",
                        column: x => x.PlaythroughCharacterId,
                        principalTable: "PlaythroughCharacters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PTCharacterAddSpellActions_PlaythroughSpells_PlaythroughSpellId",
                        column: x => x.PlaythroughSpellId,
                        principalTable: "PlaythroughSpells",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PTCharacterStatAdjustmentActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceCharacterStatAdjustmentActionId = table.Column<int>(type: "int", nullable: false),
                    CharacterStatType = table.Column<int>(type: "int", nullable: false),
                    AdjustmentOperation = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false),
                    PlaythroughCharacterId = table.Column<int>(type: "int", nullable: true),
                    ScenePTActionEventId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PTCharacterStatAdjustmentActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PTCharacterStatAdjustmentActions_PlaythroughCharacters_PlaythroughCharacterId",
                        column: x => x.PlaythroughCharacterId,
                        principalTable: "PlaythroughCharacters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ScenePTActionEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceSceneEventActionId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    ActionTargetType = table.Column<int>(type: "int", nullable: false),
                    EventActionType = table.Column<int>(type: "int", nullable: false),
                    ScenePTEventId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePTActionEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScenePTCharacterConsumableItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    ScenePTCharacterId = table.Column<int>(type: "int", nullable: false),
                    PlaythroughConsumableItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePTCharacterConsumableItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenePTCharacterConsumableItems_PlaythroughConsumableItems_PlaythroughConsumableItemId",
                        column: x => x.PlaythroughConsumableItemId,
                        principalTable: "PlaythroughConsumableItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ScenePTCharacterEquippableItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsEquipped = table.Column<bool>(type: "bit", nullable: false),
                    ScenePTCharacterId = table.Column<int>(type: "int", nullable: false),
                    PlaythroughEquippableItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePTCharacterEquippableItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenePTCharacterEquippableItems_PlaythroughEquippableItems_PlaythroughEquippableItemId",
                        column: x => x.PlaythroughEquippableItemId,
                        principalTable: "PlaythroughEquippableItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ScenePTCharacters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceSceneCharacterId = table.Column<int>(type: "int", nullable: false),
                    InitialMeleeAttackDamage = table.Column<int>(type: "int", nullable: true),
                    InitialBowAttackDamage = table.Column<int>(type: "int", nullable: true),
                    InitialMovement = table.Column<int>(type: "int", nullable: false),
                    InitialMaxConsumableInventory = table.Column<int>(type: "int", nullable: false),
                    InitialMaxEquippableInventory = table.Column<int>(type: "int", nullable: false),
                    InitialMaxHp = table.Column<int>(type: "int", nullable: false),
                    InitialMaxMp = table.Column<int>(type: "int", nullable: false),
                    IsInitiallyActive = table.Column<bool>(type: "bit", nullable: false),
                    MeleeAttackDamage = table.Column<int>(type: "int", nullable: true),
                    BowAttackDamage = table.Column<int>(type: "int", nullable: true),
                    Movement = table.Column<int>(type: "int", nullable: false),
                    MaxConsumableInventory = table.Column<int>(type: "int", nullable: false),
                    MaxEquippableInventory = table.Column<int>(type: "int", nullable: false),
                    CurrentHp = table.Column<int>(type: "int", nullable: false),
                    CurrentMp = table.Column<int>(type: "int", nullable: false),
                    MaxHp = table.Column<int>(type: "int", nullable: false),
                    MaxMp = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDead = table.Column<bool>(type: "bit", nullable: false),
                    ScenePlaythroughId = table.Column<int>(type: "int", nullable: false),
                    AlternateFormId = table.Column<int>(type: "int", nullable: true),
                    IsInAlternateForm = table.Column<bool>(type: "bit", nullable: false),
                    PlaythroughCharacterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePTCharacters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenePTCharacters_PlaythroughCharacters_AlternateFormId",
                        column: x => x.AlternateFormId,
                        principalTable: "PlaythroughCharacters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScenePTCharacters_PlaythroughCharacters_PlaythroughCharacterId",
                        column: x => x.PlaythroughCharacterId,
                        principalTable: "PlaythroughCharacters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ScenePTCharacterSpells",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceSceneCharacterSpellId = table.Column<int>(type: "int", nullable: true),
                    ScenePTCharacterId = table.Column<int>(type: "int", nullable: false),
                    PlaythroughSpellId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePTCharacterSpells", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenePTCharacterSpells_PlaythroughSpells_PlaythroughSpellId",
                        column: x => x.PlaythroughSpellId,
                        principalTable: "PlaythroughSpells",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScenePTCharacterSpells_ScenePTCharacters_ScenePTCharacterId",
                        column: x => x.ScenePTCharacterId,
                        principalTable: "ScenePTCharacters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenePTChestLootEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceSceneChestLootEntryId = table.Column<int>(type: "int", nullable: false),
                    RollMinimum = table.Column<int>(type: "int", nullable: false),
                    RollMaximum = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    PlaythroughEquippableItemId = table.Column<int>(type: "int", nullable: true),
                    PlaythroughConsumableItemId = table.Column<int>(type: "int", nullable: true),
                    ScenePTChestId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePTChestLootEntries", x => x.Id);
                    table.CheckConstraint("CK_ScenePTChestLootEntries_Item", "([PlaythroughEquippableItemId] IS NOT NULL AND [PlaythroughConsumableItemId] IS NULL) OR ([PlaythroughEquippableItemId] IS NULL AND [PlaythroughConsumableItemId] IS NOT NULL)");
                    table.CheckConstraint("CK_ScenePTChestLootEntries_Quantity", "[Quantity] >= 1");
                    table.CheckConstraint("CK_ScenePTChestLootEntries_RollRange", "[RollMinimum] >= 1 AND [RollMaximum] >= [RollMinimum]");
                    table.ForeignKey(
                        name: "FK_ScenePTChestLootEntries_PlaythroughConsumableItems_PlaythroughConsumableItemId",
                        column: x => x.PlaythroughConsumableItemId,
                        principalTable: "PlaythroughConsumableItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScenePTChestLootEntries_PlaythroughEquippableItems_PlaythroughEquippableItemId",
                        column: x => x.PlaythroughEquippableItemId,
                        principalTable: "PlaythroughEquippableItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ScenePTChests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceSceneChestId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    DieSides = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RolledValue = table.Column<int>(type: "int", nullable: true),
                    OpenedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SelectedLootEntryId = table.Column<int>(type: "int", nullable: true),
                    ScenePlaythroughId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePTChests", x => x.Id);
                    table.CheckConstraint("CK_ScenePTChests_DieSides", "[DieSides] >= 1");
                    table.CheckConstraint("CK_ScenePTChests_RolledValue", "[RolledValue] IS NULL OR [RolledValue] >= 1");
                    table.ForeignKey(
                        name: "FK_ScenePTChests_ScenePTChestLootEntries_SelectedLootEntryId",
                        column: x => x.SelectedLootEntryId,
                        principalTable: "ScenePTChestLootEntries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ScenePTDialogPages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceDialogPageId = table.Column<int>(type: "int", nullable: false),
                    OrderNum = table.Column<int>(type: "int", nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SceneDialogId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePTDialogPages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScenePTDialogSections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceDialogSectionId = table.Column<int>(type: "int", nullable: false),
                    OrderNum = table.Column<int>(type: "int", nullable: false),
                    ReadingText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsNarrator = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CharacterId = table.Column<int>(type: "int", nullable: true),
                    DialogPageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePTDialogSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenePTDialogSections_PlaythroughCharacters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "PlaythroughCharacters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScenePTDialogSections_ScenePTDialogPages_DialogPageId",
                        column: x => x.DialogPageId,
                        principalTable: "ScenePTDialogPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenePTDialogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceSceneDialogId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ScenePTId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePTDialogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScenePTEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceSceneEventId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    ExecutionStatus = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ScenePTId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePTEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScenePTGrids",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceSceneGridId = table.Column<int>(type: "int", nullable: false),
                    Rows = table.Column<int>(type: "int", nullable: false),
                    Columns = table.Column<int>(type: "int", nullable: false),
                    GridColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    BackgroundImageUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    BackgroundFileName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ScenePTId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePTGrids", x => x.Id);
                    table.CheckConstraint("CK_ScenePTGrids_Columns", "[Columns] >= 1 AND [Columns] <= 100");
                    table.CheckConstraint("CK_ScenePTGrids_Rows", "[Rows] >= 1 AND [Rows] <= 100");
                });

            migrationBuilder.CreateTable(
                name: "ScenePTIntroPages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceIntroPageId = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Config = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreviewPhotoUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    ScenePTId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePTIntroPages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScenePTParticipants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrderWithinType = table.Column<int>(type: "int", nullable: true),
                    ParticipantType = table.Column<int>(type: "int", nullable: false),
                    ScenePlaythroughId = table.Column<int>(type: "int", nullable: false),
                    JourneyPlaythroughCharacterId = table.Column<int>(type: "int", nullable: true),
                    ScenePlaythroughCharacterId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePTParticipants", x => x.Id);
                    table.CheckConstraint("CK_ScenePTParticipants_Character", "([JourneyPlaythroughCharacterId] IS NOT NULL AND [ScenePlaythroughCharacterId] IS NULL) OR ([JourneyPlaythroughCharacterId] IS NULL AND [ScenePlaythroughCharacterId] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_ScenePTParticipants_JourneyPTCharacters_JourneyPlaythroughCharacterId",
                        column: x => x.JourneyPlaythroughCharacterId,
                        principalTable: "JourneyPTCharacters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScenePTParticipants_ScenePTCharacters_ScenePlaythroughCharacterId",
                        column: x => x.ScenePlaythroughCharacterId,
                        principalTable: "ScenePTCharacters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ScenePTs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceSceneId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    PhotoUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    GridUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RoundNumber = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CurrentParticipantId = table.Column<int>(type: "int", nullable: true),
                    PlaythroughId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePTs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenePTs_Playthroughs_PlaythroughId",
                        column: x => x.PlaythroughId,
                        principalTable: "Playthroughs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScenePTs_ScenePTParticipants_CurrentParticipantId",
                        column: x => x.CurrentParticipantId,
                        principalTable: "ScenePTParticipants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterAddSpellAction_CharacterId",
                table: "CharacterAddSpellAction",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterAddSpellAction_SceneEventActionId",
                table: "CharacterAddSpellAction",
                column: "SceneEventActionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterAddSpellAction_SpellId",
                table: "CharacterAddSpellAction",
                column: "SpellId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyPTCharacterConsumableItems_JourneyPTCharacterId",
                table: "JourneyPTCharacterConsumableItems",
                column: "JourneyPTCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyPTCharacterConsumableItems_PlaythroughConsumableItemId",
                table: "JourneyPTCharacterConsumableItems",
                column: "PlaythroughConsumableItemId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyPTCharacterEquippableItems_JourneyPTCharacterId",
                table: "JourneyPTCharacterEquippableItems",
                column: "JourneyPTCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyPTCharacterEquippableItems_PlaythroughEquippableItemId",
                table: "JourneyPTCharacterEquippableItems",
                column: "PlaythroughEquippableItemId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyPTCharacters_AlternateFormId",
                table: "JourneyPTCharacters",
                column: "AlternateFormId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyPTCharacters_PlaythroughCharacterId",
                table: "JourneyPTCharacters",
                column: "PlaythroughCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyPTCharacters_PlaythroughId_PlaythroughCharacterId",
                table: "JourneyPTCharacters",
                columns: new[] { "PlaythroughId", "PlaythroughCharacterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JourneyPTCharacters_PlaythroughId_SourceJourneyCharacterId",
                table: "JourneyPTCharacters",
                columns: new[] { "PlaythroughId", "SourceJourneyCharacterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JourneyPTCharacterSpells_JourneyPTCharacterId_PlaythroughSpellId",
                table: "JourneyPTCharacterSpells",
                columns: new[] { "JourneyPTCharacterId", "PlaythroughSpellId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JourneyPTCharacterSpells_PlaythroughSpellId",
                table: "JourneyPTCharacterSpells",
                column: "PlaythroughSpellId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaythroughCharacters_BaseAlternateFormId",
                table: "PlaythroughCharacters",
                column: "BaseAlternateFormId",
                unique: true,
                filter: "[BaseAlternateFormId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PlaythroughCharacters_PlaythroughId_SourceCharacterId",
                table: "PlaythroughCharacters",
                columns: new[] { "PlaythroughId", "SourceCharacterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlaythroughCharacterSpells_PlaythroughCharacterId_PlaythroughSpellId",
                table: "PlaythroughCharacterSpells",
                columns: new[] { "PlaythroughCharacterId", "PlaythroughSpellId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlaythroughCharacterSpells_PlaythroughSpellId",
                table: "PlaythroughCharacterSpells",
                column: "PlaythroughSpellId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaythroughConsumableItems_PlaythroughId_SourceConsumableItemId",
                table: "PlaythroughConsumableItems",
                columns: new[] { "PlaythroughId", "SourceConsumableItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlaythroughEquippableItems_AffectedSpellTypeId",
                table: "PlaythroughEquippableItems",
                column: "AffectedSpellTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaythroughEquippableItems_PlaythroughId_SourceEquippableItemId",
                table: "PlaythroughEquippableItems",
                columns: new[] { "PlaythroughId", "SourceEquippableItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlaythroughEquippableItemSpells_PlaythroughSpellId",
                table: "PlaythroughEquippableItemSpells",
                column: "PlaythroughSpellId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaythroughEventLogs_PlaythroughId_EventTime",
                table: "PlaythroughEventLogs",
                columns: new[] { "PlaythroughId", "EventTime" });

            migrationBuilder.CreateIndex(
                name: "IX_PlaythroughIntroPages_PlaythroughId_SortOrder",
                table: "PlaythroughIntroPages",
                columns: new[] { "PlaythroughId", "SortOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlaythroughIntroPages_PlaythroughId_SourceIntroPageId",
                table: "PlaythroughIntroPages",
                columns: new[] { "PlaythroughId", "SourceIntroPageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Playthroughs_UserId_SourceJourneyId",
                table: "Playthroughs",
                columns: new[] { "UserId", "SourceJourneyId" },
                unique: true,
                filter: "[CompletedAt] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PlaythroughSpells_PlaythroughId_SourceSpellId",
                table: "PlaythroughSpells",
                columns: new[] { "PlaythroughId", "SourceSpellId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlaythroughSpells_PlaythroughSpellTypeId",
                table: "PlaythroughSpells",
                column: "PlaythroughSpellTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaythroughSpellTypes_PlaythroughId_SourceSpellTypeId",
                table: "PlaythroughSpellTypes",
                columns: new[] { "PlaythroughId", "SourceSpellTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PTCharacterAddSpellActions_PlaythroughCharacterId",
                table: "PTCharacterAddSpellActions",
                column: "PlaythroughCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_PTCharacterAddSpellActions_PlaythroughSpellId",
                table: "PTCharacterAddSpellActions",
                column: "PlaythroughSpellId");

            migrationBuilder.CreateIndex(
                name: "IX_PTCharacterAddSpellActions_ScenePTActionEventId",
                table: "PTCharacterAddSpellActions",
                column: "ScenePTActionEventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PTCharacterStatAdjustmentActions_PlaythroughCharacterId",
                table: "PTCharacterStatAdjustmentActions",
                column: "PlaythroughCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_PTCharacterStatAdjustmentActions_ScenePTActionEventId",
                table: "PTCharacterStatAdjustmentActions",
                column: "ScenePTActionEventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTActionEvents_ScenePTEventId_SortOrder",
                table: "ScenePTActionEvents",
                columns: new[] { "ScenePTEventId", "SortOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTActionEvents_ScenePTEventId_SourceSceneEventActionId",
                table: "ScenePTActionEvents",
                columns: new[] { "ScenePTEventId", "SourceSceneEventActionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTCharacterConsumableItems_PlaythroughConsumableItemId",
                table: "ScenePTCharacterConsumableItems",
                column: "PlaythroughConsumableItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTCharacterConsumableItems_ScenePTCharacterId",
                table: "ScenePTCharacterConsumableItems",
                column: "ScenePTCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTCharacterEquippableItems_PlaythroughEquippableItemId",
                table: "ScenePTCharacterEquippableItems",
                column: "PlaythroughEquippableItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTCharacterEquippableItems_ScenePTCharacterId",
                table: "ScenePTCharacterEquippableItems",
                column: "ScenePTCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTCharacters_AlternateFormId",
                table: "ScenePTCharacters",
                column: "AlternateFormId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTCharacters_PlaythroughCharacterId",
                table: "ScenePTCharacters",
                column: "PlaythroughCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTCharacters_ScenePlaythroughId_PlaythroughCharacterId",
                table: "ScenePTCharacters",
                columns: new[] { "ScenePlaythroughId", "PlaythroughCharacterId" });

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTCharacters_ScenePlaythroughId_SourceSceneCharacterId",
                table: "ScenePTCharacters",
                columns: new[] { "ScenePlaythroughId", "SourceSceneCharacterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTCharacterSpells_PlaythroughSpellId",
                table: "ScenePTCharacterSpells",
                column: "PlaythroughSpellId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTCharacterSpells_ScenePTCharacterId_PlaythroughSpellId",
                table: "ScenePTCharacterSpells",
                columns: new[] { "ScenePTCharacterId", "PlaythroughSpellId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTChestLootEntries_PlaythroughConsumableItemId",
                table: "ScenePTChestLootEntries",
                column: "PlaythroughConsumableItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTChestLootEntries_PlaythroughEquippableItemId",
                table: "ScenePTChestLootEntries",
                column: "PlaythroughEquippableItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTChestLootEntries_ScenePTChestId_SourceSceneChestLootEntryId",
                table: "ScenePTChestLootEntries",
                columns: new[] { "ScenePTChestId", "SourceSceneChestLootEntryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTChests_ScenePlaythroughId_SourceSceneChestId",
                table: "ScenePTChests",
                columns: new[] { "ScenePlaythroughId", "SourceSceneChestId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTChests_SelectedLootEntryId",
                table: "ScenePTChests",
                column: "SelectedLootEntryId",
                unique: true,
                filter: "[SelectedLootEntryId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTDialogPage_SceneDialogId_OrderNum",
                table: "ScenePTDialogPage",
                columns: new[] { "SceneDialogId", "OrderNum" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTDialogPages_SceneDialogId_OrderNum",
                table: "ScenePTDialogPages",
                columns: new[] { "SceneDialogId", "OrderNum" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTDialogPages_SceneDialogId_SourceDialogPageId",
                table: "ScenePTDialogPages",
                columns: new[] { "SceneDialogId", "SourceDialogPageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTDialogs_ScenePTId_SourceSceneDialogId",
                table: "ScenePTDialogs",
                columns: new[] { "ScenePTId", "SourceSceneDialogId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTDialogSection_CharacterId",
                table: "ScenePTDialogSection",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTDialogSection_DialogPageId_OrderNum",
                table: "ScenePTDialogSection",
                columns: new[] { "DialogPageId", "OrderNum" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTDialogSections_CharacterId",
                table: "ScenePTDialogSections",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTDialogSections_DialogPageId_OrderNum",
                table: "ScenePTDialogSections",
                columns: new[] { "DialogPageId", "OrderNum" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTDialogSections_DialogPageId_SourceDialogSectionId",
                table: "ScenePTDialogSections",
                columns: new[] { "DialogPageId", "SourceDialogSectionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTEvents_ScenePTId_SortOrder",
                table: "ScenePTEvents",
                columns: new[] { "ScenePTId", "SortOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTEvents_ScenePTId_SourceSceneEventId",
                table: "ScenePTEvents",
                columns: new[] { "ScenePTId", "SourceSceneEventId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTGrids_ScenePTId",
                table: "ScenePTGrids",
                column: "ScenePTId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTIntroPages_ScenePTId_SortOrder",
                table: "ScenePTIntroPages",
                columns: new[] { "ScenePTId", "SortOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTIntroPages_ScenePTId_SourceIntroPageId",
                table: "ScenePTIntroPages",
                columns: new[] { "ScenePTId", "SourceIntroPageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTParticipants_JourneyPlaythroughCharacterId",
                table: "ScenePTParticipants",
                column: "JourneyPlaythroughCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTParticipants_ScenePlaythroughCharacterId",
                table: "ScenePTParticipants",
                column: "ScenePlaythroughCharacterId",
                unique: true,
                filter: "[ScenePlaythroughCharacterId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTParticipants_ScenePlaythroughId_JourneyPlaythroughCharacterId",
                table: "ScenePTParticipants",
                columns: new[] { "ScenePlaythroughId", "JourneyPlaythroughCharacterId" },
                unique: true,
                filter: "[JourneyPlaythroughCharacterId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTParticipants_ScenePlaythroughId_ParticipantType_SortOrderWithinType",
                table: "ScenePTParticipants",
                columns: new[] { "ScenePlaythroughId", "ParticipantType", "SortOrderWithinType" });

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTs_CurrentParticipantId",
                table: "ScenePTs",
                column: "CurrentParticipantId",
                unique: true,
                filter: "[CurrentParticipantId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTs_PlaythroughId_SortOrder",
                table: "ScenePTs",
                columns: new[] { "PlaythroughId", "SortOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTs_PlaythroughId_SourceSceneId",
                table: "ScenePTs",
                columns: new[] { "PlaythroughId", "SourceSceneId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PTCharacterAddSpellActions_ScenePTActionEvents_ScenePTActionEventId",
                table: "PTCharacterAddSpellActions",
                column: "ScenePTActionEventId",
                principalTable: "ScenePTActionEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PTCharacterStatAdjustmentActions_ScenePTActionEvents_ScenePTActionEventId",
                table: "PTCharacterStatAdjustmentActions",
                column: "ScenePTActionEventId",
                principalTable: "ScenePTActionEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScenePTActionEvents_ScenePTEvents_ScenePTEventId",
                table: "ScenePTActionEvents",
                column: "ScenePTEventId",
                principalTable: "ScenePTEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScenePTCharacterConsumableItems_ScenePTCharacters_ScenePTCharacterId",
                table: "ScenePTCharacterConsumableItems",
                column: "ScenePTCharacterId",
                principalTable: "ScenePTCharacters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScenePTCharacterEquippableItems_ScenePTCharacters_ScenePTCharacterId",
                table: "ScenePTCharacterEquippableItems",
                column: "ScenePTCharacterId",
                principalTable: "ScenePTCharacters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScenePTCharacters_ScenePTs_ScenePlaythroughId",
                table: "ScenePTCharacters",
                column: "ScenePlaythroughId",
                principalTable: "ScenePTs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScenePTChestLootEntries_ScenePTChests_ScenePTChestId",
                table: "ScenePTChestLootEntries",
                column: "ScenePTChestId",
                principalTable: "ScenePTChests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScenePTChests_ScenePTs_ScenePlaythroughId",
                table: "ScenePTChests",
                column: "ScenePlaythroughId",
                principalTable: "ScenePTs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScenePTDialogPages_ScenePTDialogs_SceneDialogId",
                table: "ScenePTDialogPages",
                column: "SceneDialogId",
                principalTable: "ScenePTDialogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScenePTDialogs_ScenePTs_ScenePTId",
                table: "ScenePTDialogs",
                column: "ScenePTId",
                principalTable: "ScenePTs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScenePTEvents_ScenePTs_ScenePTId",
                table: "ScenePTEvents",
                column: "ScenePTId",
                principalTable: "ScenePTs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScenePTGrids_ScenePTs_ScenePTId",
                table: "ScenePTGrids",
                column: "ScenePTId",
                principalTable: "ScenePTs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScenePTIntroPages_ScenePTs_ScenePTId",
                table: "ScenePTIntroPages",
                column: "ScenePTId",
                principalTable: "ScenePTs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScenePTParticipants_ScenePTs_ScenePlaythroughId",
                table: "ScenePTParticipants",
                column: "ScenePlaythroughId",
                principalTable: "ScenePTs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScenePTParticipants_JourneyPTCharacters_JourneyPlaythroughCharacterId",
                table: "ScenePTParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_ScenePTChestLootEntries_PlaythroughConsumableItems_PlaythroughConsumableItemId",
                table: "ScenePTChestLootEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_ScenePTChestLootEntries_PlaythroughEquippableItems_PlaythroughEquippableItemId",
                table: "ScenePTChestLootEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_ScenePTCharacters_PlaythroughCharacters_AlternateFormId",
                table: "ScenePTCharacters");

            migrationBuilder.DropForeignKey(
                name: "FK_ScenePTCharacters_PlaythroughCharacters_PlaythroughCharacterId",
                table: "ScenePTCharacters");

            migrationBuilder.DropForeignKey(
                name: "FK_ScenePTs_Playthroughs_PlaythroughId",
                table: "ScenePTs");

            migrationBuilder.DropForeignKey(
                name: "FK_ScenePTParticipants_ScenePTCharacters_ScenePlaythroughCharacterId",
                table: "ScenePTParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_ScenePTChests_ScenePTs_ScenePlaythroughId",
                table: "ScenePTChests");

            migrationBuilder.DropForeignKey(
                name: "FK_ScenePTParticipants_ScenePTs_ScenePlaythroughId",
                table: "ScenePTParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_ScenePTChestLootEntries_ScenePTChests_ScenePTChestId",
                table: "ScenePTChestLootEntries");

            migrationBuilder.DropTable(
                name: "CharacterAddSpellAction");

            migrationBuilder.DropTable(
                name: "JourneyPTCharacterConsumableItems");

            migrationBuilder.DropTable(
                name: "JourneyPTCharacterEquippableItems");

            migrationBuilder.DropTable(
                name: "JourneyPTCharacterSpells");

            migrationBuilder.DropTable(
                name: "PlaythroughCharacterSpells");

            migrationBuilder.DropTable(
                name: "PlaythroughEquippableItemSpells");

            migrationBuilder.DropTable(
                name: "PlaythroughEventLogs");

            migrationBuilder.DropTable(
                name: "PlaythroughIntroPages");

            migrationBuilder.DropTable(
                name: "PTCharacterAddSpellActions");

            migrationBuilder.DropTable(
                name: "PTCharacterStatAdjustmentActions");

            migrationBuilder.DropTable(
                name: "ScenePTCharacterConsumableItems");

            migrationBuilder.DropTable(
                name: "ScenePTCharacterEquippableItems");

            migrationBuilder.DropTable(
                name: "ScenePTCharacterSpells");

            migrationBuilder.DropTable(
                name: "ScenePTDialogSection");

            migrationBuilder.DropTable(
                name: "ScenePTDialogSections");

            migrationBuilder.DropTable(
                name: "ScenePTGrids");

            migrationBuilder.DropTable(
                name: "ScenePTIntroPages");

            migrationBuilder.DropTable(
                name: "ScenePTActionEvents");

            migrationBuilder.DropTable(
                name: "PlaythroughSpells");

            migrationBuilder.DropTable(
                name: "ScenePTDialogPage");

            migrationBuilder.DropTable(
                name: "ScenePTDialogPages");

            migrationBuilder.DropTable(
                name: "ScenePTEvents");

            migrationBuilder.DropTable(
                name: "ScenePTDialogs");

            migrationBuilder.DropTable(
                name: "JourneyPTCharacters");

            migrationBuilder.DropTable(
                name: "PlaythroughConsumableItems");

            migrationBuilder.DropTable(
                name: "PlaythroughEquippableItems");

            migrationBuilder.DropTable(
                name: "PlaythroughSpellTypes");

            migrationBuilder.DropTable(
                name: "PlaythroughCharacters");

            migrationBuilder.DropTable(
                name: "Playthroughs");

            migrationBuilder.DropTable(
                name: "ScenePTCharacters");

            migrationBuilder.DropTable(
                name: "ScenePTs");

            migrationBuilder.DropTable(
                name: "ScenePTParticipants");

            migrationBuilder.DropTable(
                name: "ScenePTChests");

            migrationBuilder.DropTable(
                name: "ScenePTChestLootEntries");

            migrationBuilder.CreateTable(
                name: "DialogPages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SceneDialogId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    OrderNum = table.Column<int>(type: "int", nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DialogPages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DialogPages_SceneDialogs_SceneDialogId",
                        column: x => x.SceneDialogId,
                        principalTable: "SceneDialogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JourneyRevisions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    SourceJourneyId = table.Column<int>(type: "int", nullable: true),
                    ContentHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevisionNumber = table.Column<int>(type: "int", nullable: false),
                    SchemaVersion = table.Column<int>(type: "int", nullable: false),
                    SnapshotJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JourneyRevisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JourneyRevisions_Journeys_SourceJourneyId",
                        column: x => x.SourceJourneyId,
                        principalTable: "Journeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_JourneyRevisions_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DialogPageSections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharacterId = table.Column<int>(type: "int", nullable: true),
                    DialogPageId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsNarrator = table.Column<bool>(type: "bit", nullable: false),
                    OrderNum = table.Column<int>(type: "int", nullable: false),
                    ReadingText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DialogPageSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DialogPageSections_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DialogPageSections_DialogPages_DialogPageId",
                        column: x => x.DialogPageId,
                        principalTable: "DialogPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JourneyPlaythroughs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JourneyId = table.Column<int>(type: "int", nullable: true),
                    JourneyRevisionId = table.Column<int>(type: "int", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SourceJourneyId = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JourneyPlaythroughs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JourneyPlaythroughs_JourneyRevisions_JourneyRevisionId",
                        column: x => x.JourneyRevisionId,
                        principalTable: "JourneyRevisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JourneyPlaythroughs_Journeys_JourneyId",
                        column: x => x.JourneyId,
                        principalTable: "Journeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "JourneyPlaythroughCharacters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlternateFormId = table.Column<int>(type: "int", nullable: true),
                    JourneyCharacterId = table.Column<int>(type: "int", nullable: true),
                    JourneyPlaythroughId = table.Column<int>(type: "int", nullable: false),
                    BowAttackDamage = table.Column<int>(type: "int", nullable: true),
                    CurrentHp = table.Column<int>(type: "int", nullable: false),
                    CurrentMp = table.Column<int>(type: "int", nullable: false),
                    IsDown = table.Column<bool>(type: "bit", nullable: false),
                    IsInAlternateForm = table.Column<bool>(type: "bit", nullable: false),
                    MaxConsumableInventory = table.Column<int>(type: "int", nullable: false),
                    MaxEquippableInventory = table.Column<int>(type: "int", nullable: false),
                    MaxHp = table.Column<int>(type: "int", nullable: false),
                    MaxMp = table.Column<int>(type: "int", nullable: false),
                    MeleeAttackDamage = table.Column<int>(type: "int", nullable: true),
                    Movement = table.Column<int>(type: "int", nullable: false),
                    SnapshotAssignmentKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SnapshotCharacterKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JourneyPlaythroughCharacters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JourneyPlaythroughCharacters_JourneyCharacters_JourneyCharacterId",
                        column: x => x.JourneyCharacterId,
                        principalTable: "JourneyCharacters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_JourneyPlaythroughCharacters_JourneyPlaythroughCharacters_AlternateFormId",
                        column: x => x.AlternateFormId,
                        principalTable: "JourneyPlaythroughCharacters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JourneyPlaythroughCharacters_JourneyPlaythroughs_JourneyPlaythroughId",
                        column: x => x.JourneyPlaythroughId,
                        principalTable: "JourneyPlaythroughs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JourneyPlaythroughEventLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JourneyPlaythroughId = table.Column<int>(type: "int", nullable: false),
                    EventTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JourneyPlaythroughEventLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JourneyPlaythroughEventLogs_JourneyPlaythroughs_JourneyPlaythroughId",
                        column: x => x.JourneyPlaythroughId,
                        principalTable: "JourneyPlaythroughs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JourneyPlaythroughCharacterConsumableItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConsumableItemId = table.Column<int>(type: "int", nullable: true),
                    JourneyPlaythroughCharacterId = table.Column<int>(type: "int", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    SnapshotConsumableKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JourneyPlaythroughCharacterConsumableItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JourneyPlaythroughCharacterConsumableItems_ConsumableItems_ConsumableItemId",
                        column: x => x.ConsumableItemId,
                        principalTable: "ConsumableItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_JourneyPlaythroughCharacterConsumableItems_JourneyPlaythroughCharacters_JourneyPlaythroughCharacterId",
                        column: x => x.JourneyPlaythroughCharacterId,
                        principalTable: "JourneyPlaythroughCharacters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JourneyPlaythroughCharacterEquippableItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquippableItemId = table.Column<int>(type: "int", nullable: true),
                    JourneyPlaythroughCharacterId = table.Column<int>(type: "int", nullable: false),
                    IsEquipped = table.Column<bool>(type: "bit", nullable: false),
                    SnapshotEquipmentKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JourneyPlaythroughCharacterEquippableItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JourneyPlaythroughCharacterEquippableItems_EquippableItems_EquippableItemId",
                        column: x => x.EquippableItemId,
                        principalTable: "EquippableItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_JourneyPlaythroughCharacterEquippableItems_JourneyPlaythroughCharacters_JourneyPlaythroughCharacterId",
                        column: x => x.JourneyPlaythroughCharacterId,
                        principalTable: "JourneyPlaythroughCharacters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JourneyPlaythroughCharacterSpells",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JourneyCharacterSpellId = table.Column<int>(type: "int", nullable: true),
                    JourneyPlaythroughCharacterId = table.Column<int>(type: "int", nullable: false),
                    SnapshotSpellKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JourneyPlaythroughCharacterSpells", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JourneyPlaythroughCharacterSpells_JourneyCharacterSpells_JourneyCharacterSpellId",
                        column: x => x.JourneyCharacterSpellId,
                        principalTable: "JourneyCharacterSpells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_JourneyPlaythroughCharacterSpells_JourneyPlaythroughCharacters_JourneyPlaythroughCharacterId",
                        column: x => x.JourneyPlaythroughCharacterId,
                        principalTable: "JourneyPlaythroughCharacters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenePlaythroughCharacterConsumableItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConsumableItemId = table.Column<int>(type: "int", nullable: true),
                    ScenePlaythroughCharacterId = table.Column<int>(type: "int", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    SnapshotConsumableKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePlaythroughCharacterConsumableItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenePlaythroughCharacterConsumableItems_ConsumableItems_ConsumableItemId",
                        column: x => x.ConsumableItemId,
                        principalTable: "ConsumableItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ScenePlaythroughCharacterEquippableItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquippableItemId = table.Column<int>(type: "int", nullable: true),
                    ScenePlaythroughCharacterId = table.Column<int>(type: "int", nullable: false),
                    IsEquipped = table.Column<bool>(type: "bit", nullable: false),
                    SnapshotEquipmentKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePlaythroughCharacterEquippableItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenePlaythroughCharacterEquippableItems_EquippableItems_EquippableItemId",
                        column: x => x.EquippableItemId,
                        principalTable: "EquippableItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ScenePlaythroughCharacters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlternateFormId = table.Column<int>(type: "int", nullable: true),
                    SceneCharacterId = table.Column<int>(type: "int", nullable: true),
                    ScenePlaythroughId = table.Column<int>(type: "int", nullable: false),
                    BowAttackDamage = table.Column<int>(type: "int", nullable: true),
                    CurrentHp = table.Column<int>(type: "int", nullable: false),
                    CurrentMp = table.Column<int>(type: "int", nullable: false),
                    IsDead = table.Column<bool>(type: "bit", nullable: false),
                    IsInAlternateForm = table.Column<bool>(type: "bit", nullable: false),
                    MaxConsumableInventory = table.Column<int>(type: "int", nullable: false),
                    MaxEquippableInventory = table.Column<int>(type: "int", nullable: false),
                    MaxHp = table.Column<int>(type: "int", nullable: false),
                    MaxMp = table.Column<int>(type: "int", nullable: false),
                    MeleeAttackDamage = table.Column<int>(type: "int", nullable: true),
                    Movement = table.Column<int>(type: "int", nullable: false),
                    SnapshotAssignmentKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SnapshotCharacterKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePlaythroughCharacters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenePlaythroughCharacters_SceneCharacters_SceneCharacterId",
                        column: x => x.SceneCharacterId,
                        principalTable: "SceneCharacters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ScenePlaythroughCharacters_ScenePlaythroughCharacters_AlternateFormId",
                        column: x => x.AlternateFormId,
                        principalTable: "ScenePlaythroughCharacters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ScenePlaythroughCharacterSpells",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SceneCharacterSpellId = table.Column<int>(type: "int", nullable: true),
                    ScenePlaythroughCharacterId = table.Column<int>(type: "int", nullable: false),
                    SnapshotSpellKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePlaythroughCharacterSpells", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenePlaythroughCharacterSpells_SceneCharacterSpells_SceneCharacterSpellId",
                        column: x => x.SceneCharacterSpellId,
                        principalTable: "SceneCharacterSpells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ScenePlaythroughCharacterSpells_ScenePlaythroughCharacters_ScenePlaythroughCharacterId",
                        column: x => x.ScenePlaythroughCharacterId,
                        principalTable: "ScenePlaythroughCharacters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenePlaythroughChests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SceneChestId = table.Column<int>(type: "int", nullable: true),
                    ScenePlaythroughId = table.Column<int>(type: "int", nullable: false),
                    SelectedLootEntryId = table.Column<int>(type: "int", nullable: true),
                    OpenedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RolledValue = table.Column<int>(type: "int", nullable: true),
                    SelectedLootEntrySnapshotKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SnapshotChestKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePlaythroughChests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenePlaythroughChests_SceneChestLootEntries_SelectedLootEntryId",
                        column: x => x.SelectedLootEntryId,
                        principalTable: "SceneChestLootEntries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScenePlaythroughChests_SceneChests_SceneChestId",
                        column: x => x.SceneChestId,
                        principalTable: "SceneChests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ScenePlaythroughEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SceneEventId = table.Column<int>(type: "int", nullable: true),
                    ScenePlaythroughId = table.Column<int>(type: "int", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ExecutionStatus = table.Column<int>(type: "int", nullable: false),
                    SnapshotEventKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePlaythroughEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenePlaythroughEvents_SceneEvents_SceneEventId",
                        column: x => x.SceneEventId,
                        principalTable: "SceneEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ScenePlaythroughParticipants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JourneyPlaythroughCharacterId = table.Column<int>(type: "int", nullable: true),
                    ScenePlaythroughCharacterId = table.Column<int>(type: "int", nullable: true),
                    ScenePlaythroughId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ParticipantType = table.Column<int>(type: "int", nullable: false),
                    SortOrderWithinType = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePlaythroughParticipants", x => x.Id);
                    table.CheckConstraint("CK_ScenePlaythroughParticipants_ActiveOrder", "([IsActive] = 1 AND [SortOrderWithinType] IS NOT NULL) OR ([IsActive] = 0 AND [SortOrderWithinType] IS NULL)");
                    table.CheckConstraint("CK_ScenePlaythroughParticipants_Character", "([JourneyPlaythroughCharacterId] IS NOT NULL AND [ScenePlaythroughCharacterId] IS NULL) OR ([JourneyPlaythroughCharacterId] IS NULL AND [ScenePlaythroughCharacterId] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_ScenePlaythroughParticipants_JourneyPlaythroughCharacters_JourneyPlaythroughCharacterId",
                        column: x => x.JourneyPlaythroughCharacterId,
                        principalTable: "JourneyPlaythroughCharacters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScenePlaythroughParticipants_ScenePlaythroughCharacters_ScenePlaythroughCharacterId",
                        column: x => x.ScenePlaythroughCharacterId,
                        principalTable: "ScenePlaythroughCharacters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ScenePlaythroughs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrentParticipantId = table.Column<int>(type: "int", nullable: true),
                    JourneyPlaythroughId = table.Column<int>(type: "int", nullable: false),
                    SceneId = table.Column<int>(type: "int", nullable: true),
                    EndedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RoundNumber = table.Column<int>(type: "int", nullable: false),
                    SnapshotSceneKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SnapshotSortOrder = table.Column<int>(type: "int", nullable: false),
                    SourceSceneId = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePlaythroughs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenePlaythroughs_JourneyPlaythroughs_JourneyPlaythroughId",
                        column: x => x.JourneyPlaythroughId,
                        principalTable: "JourneyPlaythroughs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScenePlaythroughs_ScenePlaythroughParticipants_CurrentParticipantId",
                        column: x => x.CurrentParticipantId,
                        principalTable: "ScenePlaythroughParticipants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScenePlaythroughs_Scenes_SceneId",
                        column: x => x.SceneId,
                        principalTable: "Scenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DialogPages_SceneDialogId_OrderNum",
                table: "DialogPages",
                columns: new[] { "SceneDialogId", "OrderNum" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DialogPageSections_CharacterId",
                table: "DialogPageSections",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_DialogPageSections_DialogPageId_OrderNum",
                table: "DialogPageSections",
                columns: new[] { "DialogPageId", "OrderNum" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JourneyPlaythroughCharacterConsumableItems_ConsumableItemId",
                table: "JourneyPlaythroughCharacterConsumableItems",
                column: "ConsumableItemId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyPlaythroughCharacterConsumableItems_JourneyPlaythroughCharacterId",
                table: "JourneyPlaythroughCharacterConsumableItems",
                column: "JourneyPlaythroughCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyPlaythroughCharacterEquippableItems_EquippableItemId",
                table: "JourneyPlaythroughCharacterEquippableItems",
                column: "EquippableItemId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyPlaythroughCharacterEquippableItems_JourneyPlaythroughCharacterId",
                table: "JourneyPlaythroughCharacterEquippableItems",
                column: "JourneyPlaythroughCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyPlaythroughCharacters_AlternateFormId",
                table: "JourneyPlaythroughCharacters",
                column: "AlternateFormId",
                unique: true,
                filter: "[AlternateFormId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyPlaythroughCharacters_JourneyCharacterId",
                table: "JourneyPlaythroughCharacters",
                column: "JourneyCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyPlaythroughCharacters_JourneyPlaythroughId_SnapshotAssignmentKey",
                table: "JourneyPlaythroughCharacters",
                columns: new[] { "JourneyPlaythroughId", "SnapshotAssignmentKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JourneyPlaythroughCharacterSpells_JourneyCharacterSpellId",
                table: "JourneyPlaythroughCharacterSpells",
                column: "JourneyCharacterSpellId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyPlaythroughCharacterSpells_JourneyPlaythroughCharacterId_SnapshotSpellKey",
                table: "JourneyPlaythroughCharacterSpells",
                columns: new[] { "JourneyPlaythroughCharacterId", "SnapshotSpellKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JourneyPlaythroughEventLogs_JourneyPlaythroughId_EventTime",
                table: "JourneyPlaythroughEventLogs",
                columns: new[] { "JourneyPlaythroughId", "EventTime" });

            migrationBuilder.CreateIndex(
                name: "IX_JourneyPlaythroughs_JourneyId",
                table: "JourneyPlaythroughs",
                column: "JourneyId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyPlaythroughs_JourneyRevisionId",
                table: "JourneyPlaythroughs",
                column: "JourneyRevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyPlaythroughs_SourceJourneyId",
                table: "JourneyPlaythroughs",
                column: "SourceJourneyId",
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyRevisions_CreatedByUserId_SourceJourneyId_ContentHash",
                table: "JourneyRevisions",
                columns: new[] { "CreatedByUserId", "SourceJourneyId", "ContentHash" },
                unique: true,
                filter: "[SourceJourneyId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyRevisions_CreatedByUserId_SourceJourneyId_RevisionNumber",
                table: "JourneyRevisions",
                columns: new[] { "CreatedByUserId", "SourceJourneyId", "RevisionNumber" },
                unique: true,
                filter: "[SourceJourneyId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyRevisions_SourceJourneyId",
                table: "JourneyRevisions",
                column: "SourceJourneyId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePlaythroughCharacterConsumableItems_ConsumableItemId",
                table: "ScenePlaythroughCharacterConsumableItems",
                column: "ConsumableItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePlaythroughCharacterConsumableItems_ScenePlaythroughCharacterId",
                table: "ScenePlaythroughCharacterConsumableItems",
                column: "ScenePlaythroughCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePlaythroughCharacterEquippableItems_EquippableItemId",
                table: "ScenePlaythroughCharacterEquippableItems",
                column: "EquippableItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePlaythroughCharacterEquippableItems_ScenePlaythroughCharacterId",
                table: "ScenePlaythroughCharacterEquippableItems",
                column: "ScenePlaythroughCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePlaythroughCharacters_AlternateFormId",
                table: "ScenePlaythroughCharacters",
                column: "AlternateFormId",
                unique: true,
                filter: "[AlternateFormId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePlaythroughCharacters_SceneCharacterId",
                table: "ScenePlaythroughCharacters",
                column: "SceneCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePlaythroughCharacters_ScenePlaythroughId_SnapshotAssignmentKey",
                table: "ScenePlaythroughCharacters",
                columns: new[] { "ScenePlaythroughId", "SnapshotAssignmentKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenePlaythroughCharacterSpells_SceneCharacterSpellId",
                table: "ScenePlaythroughCharacterSpells",
                column: "SceneCharacterSpellId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePlaythroughCharacterSpells_ScenePlaythroughCharacterId_SnapshotSpellKey",
                table: "ScenePlaythroughCharacterSpells",
                columns: new[] { "ScenePlaythroughCharacterId", "SnapshotSpellKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenePlaythroughChests_SceneChestId",
                table: "ScenePlaythroughChests",
                column: "SceneChestId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePlaythroughChests_ScenePlaythroughId_SnapshotChestKey",
                table: "ScenePlaythroughChests",
                columns: new[] { "ScenePlaythroughId", "SnapshotChestKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenePlaythroughChests_SelectedLootEntryId",
                table: "ScenePlaythroughChests",
                column: "SelectedLootEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePlaythroughEvents_SceneEventId",
                table: "ScenePlaythroughEvents",
                column: "SceneEventId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePlaythroughEvents_ScenePlaythroughId_SnapshotEventKey",
                table: "ScenePlaythroughEvents",
                columns: new[] { "ScenePlaythroughId", "SnapshotEventKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenePlaythroughParticipants_JourneyPlaythroughCharacterId",
                table: "ScenePlaythroughParticipants",
                column: "JourneyPlaythroughCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePlaythroughParticipants_ScenePlaythroughCharacterId",
                table: "ScenePlaythroughParticipants",
                column: "ScenePlaythroughCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePlaythroughParticipants_ScenePlaythroughId_JourneyPlaythroughCharacterId",
                table: "ScenePlaythroughParticipants",
                columns: new[] { "ScenePlaythroughId", "JourneyPlaythroughCharacterId" },
                unique: true,
                filter: "[JourneyPlaythroughCharacterId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePlaythroughParticipants_ScenePlaythroughId_ParticipantType_SortOrderWithinType",
                table: "ScenePlaythroughParticipants",
                columns: new[] { "ScenePlaythroughId", "ParticipantType", "SortOrderWithinType" },
                unique: true,
                filter: "[SortOrderWithinType] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePlaythroughParticipants_ScenePlaythroughId_ScenePlaythroughCharacterId",
                table: "ScenePlaythroughParticipants",
                columns: new[] { "ScenePlaythroughId", "ScenePlaythroughCharacterId" },
                unique: true,
                filter: "[ScenePlaythroughCharacterId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePlaythroughs_CurrentParticipantId",
                table: "ScenePlaythroughs",
                column: "CurrentParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePlaythroughs_JourneyPlaythroughId_SnapshotSceneKey",
                table: "ScenePlaythroughs",
                columns: new[] { "JourneyPlaythroughId", "SnapshotSceneKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenePlaythroughs_SceneId",
                table: "ScenePlaythroughs",
                column: "SceneId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScenePlaythroughCharacterConsumableItems_ScenePlaythroughCharacters_ScenePlaythroughCharacterId",
                table: "ScenePlaythroughCharacterConsumableItems",
                column: "ScenePlaythroughCharacterId",
                principalTable: "ScenePlaythroughCharacters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScenePlaythroughCharacterEquippableItems_ScenePlaythroughCharacters_ScenePlaythroughCharacterId",
                table: "ScenePlaythroughCharacterEquippableItems",
                column: "ScenePlaythroughCharacterId",
                principalTable: "ScenePlaythroughCharacters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScenePlaythroughCharacters_ScenePlaythroughs_ScenePlaythroughId",
                table: "ScenePlaythroughCharacters",
                column: "ScenePlaythroughId",
                principalTable: "ScenePlaythroughs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScenePlaythroughChests_ScenePlaythroughs_ScenePlaythroughId",
                table: "ScenePlaythroughChests",
                column: "ScenePlaythroughId",
                principalTable: "ScenePlaythroughs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScenePlaythroughEvents_ScenePlaythroughs_ScenePlaythroughId",
                table: "ScenePlaythroughEvents",
                column: "ScenePlaythroughId",
                principalTable: "ScenePlaythroughs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScenePlaythroughParticipants_ScenePlaythroughs_ScenePlaythroughId",
                table: "ScenePlaythroughParticipants",
                column: "ScenePlaythroughId",
                principalTable: "ScenePlaythroughs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
