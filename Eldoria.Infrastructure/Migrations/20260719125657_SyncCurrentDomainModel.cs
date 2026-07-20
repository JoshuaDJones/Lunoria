using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eldoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncCurrentDomainModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterSpells_Spells_SpellId",
                table: "CharacterSpells");

            migrationBuilder.DropForeignKey(
                name: "FK_DialogPage_SceneDialogs_SceneDialogId",
                table: "DialogPage");

            migrationBuilder.DropForeignKey(
                name: "FK_DialogPageSection_Characters_CharacterId",
                table: "DialogPageSection");

            migrationBuilder.DropForeignKey(
                name: "FK_DialogPageSection_DialogPage_DialogPageId",
                table: "DialogPageSection");

            migrationBuilder.DropForeignKey(
                name: "FK_JourneyCharacters_Characters_AlternateFormId",
                table: "JourneyCharacters");

            migrationBuilder.DropForeignKey(
                name: "FK_JourneyCharacters_Characters_CharacterId",
                table: "JourneyCharacters");

            migrationBuilder.DropForeignKey(
                name: "FK_JourneyCharacterSpells_Spells_SpellId",
                table: "JourneyCharacterSpells");

            migrationBuilder.DropForeignKey(
                name: "FK_Journeys_Users_UserId",
                table: "Journeys");

            migrationBuilder.DropForeignKey(
                name: "FK_SceneCharacters_Characters_CharacterId",
                table: "SceneCharacters");

            migrationBuilder.DropForeignKey(
                name: "FK_Series_Users_UserId",
                table: "Series");

            migrationBuilder.DropTable(
                name: "IntroPage");

            migrationBuilder.DropTable(
                name: "JourneyCharacterEquippableItems");

            migrationBuilder.DropTable(
                name: "JourneyCharacterItems");

            migrationBuilder.DropTable(
                name: "SceneCharacterItems");

            migrationBuilder.DropTable(
                name: "SceneParticipantTurns");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "SceneParticipants");

            migrationBuilder.DropTable(
                name: "SceneProgresses");

            migrationBuilder.DropIndex(
                name: "IX_Spells_UserId",
                table: "Spells");

            migrationBuilder.DropIndex(
                name: "IX_Series_UserId",
                table: "Series");

            migrationBuilder.DropIndex(
                name: "IX_Scenes_JourneyId",
                table: "Scenes");

            migrationBuilder.DropIndex(
                name: "IX_SceneCharacters_SceneId",
                table: "SceneCharacters");

            migrationBuilder.DropIndex(
                name: "IX_Journeys_SeriesId",
                table: "Journeys");

            migrationBuilder.DropIndex(
                name: "IX_JourneyCharacters_JourneyId",
                table: "JourneyCharacters");

            migrationBuilder.DropIndex(
                name: "IX_EquippableItems_UserId",
                table: "EquippableItems");

            migrationBuilder.DropIndex(
                name: "IX_CharacterSpells_CharacterId",
                table: "CharacterSpells");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DialogPageSection",
                table: "DialogPageSection");

            migrationBuilder.DropIndex(
                name: "IX_DialogPageSection_DialogPageId",
                table: "DialogPageSection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DialogPage",
                table: "DialogPage");

            migrationBuilder.DropIndex(
                name: "IX_DialogPage_SceneDialogId",
                table: "DialogPage");

            migrationBuilder.DropColumn(
                name: "IsAlternateForm",
                table: "SceneCharacters");

            migrationBuilder.DropColumn(
                name: "CurrentHp",
                table: "JourneyCharacters");

            migrationBuilder.DropColumn(
                name: "CurrentMp",
                table: "JourneyCharacters");

            migrationBuilder.DropColumn(
                name: "IsDown",
                table: "JourneyCharacters");

            migrationBuilder.DropColumn(
                name: "IsEnemy",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "IsNPC",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "IsPlayer",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Portrait",
                table: "Characters");

            migrationBuilder.RenameTable(
                name: "DialogPageSection",
                newName: "DialogPageSections");

            migrationBuilder.RenameTable(
                name: "DialogPage",
                newName: "DialogPages");

            migrationBuilder.RenameColumn(
                name: "UpdateDate",
                table: "Users",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "Users",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdateDate",
                table: "Spells",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "Spells",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdateDate",
                table: "Series",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "Series",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdateDate",
                table: "Scenes",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "Scenes",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdateDate",
                table: "SceneDialogs",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "SceneDialogs",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "IsDown",
                table: "SceneCharacters",
                newName: "IsInitiallyActive");

            migrationBuilder.RenameColumn(
                name: "CurrentMp",
                table: "SceneCharacters",
                newName: "Movement");

            migrationBuilder.RenameColumn(
                name: "CurrentHp",
                table: "SceneCharacters",
                newName: "MaxMp");

            migrationBuilder.RenameColumn(
                name: "UpdateDate",
                table: "Journeys",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "Journeys",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "IsInAlternateForm",
                table: "JourneyCharacters",
                newName: "IsInitiallyActive");

            migrationBuilder.RenameColumn(
                name: "UpdateDate",
                table: "EquippableItems",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "EquippableItems",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdateDate",
                table: "Characters",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "Characters",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "DialogUnActiveColor",
                table: "CharacterDialogSettings",
                newName: "DialogInActiveColor");

            migrationBuilder.RenameColumn(
                name: "UpdateDate",
                table: "DialogPageSections",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "DialogPageSections",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_DialogPageSection_CharacterId",
                table: "DialogPageSections",
                newName: "IX_DialogPageSections_CharacterId");

            migrationBuilder.RenameColumn(
                name: "UpdateDate",
                table: "DialogPages",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "DialogPages",
                newName: "CreatedAt");

            migrationBuilder.AlterColumn<string>(
                name: "PhotoUrl",
                table: "Scenes",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048);

            migrationBuilder.AlterColumn<string>(
                name: "GridUrl",
                table: "Scenes",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "Scenes",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Scenes",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "SceneDialogs",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "AlternateFormId",
                table: "SceneCharacters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BowAttackDamage",
                table: "SceneCharacters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxConsumableInventory",
                table: "SceneCharacters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxEquippableInventory",
                table: "SceneCharacters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxHp",
                table: "SceneCharacters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MeleeAttackDamage",
                table: "SceneCharacters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Journeys",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "SpellDamageModifier",
                table: "EquippableItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "PortraitFileName",
                table: "Characters",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AddColumn<int>(
                name: "CharacterType",
                table: "Characters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PortraitUrl",
                table: "Characters",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhotoUrl",
                table: "DialogPages",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "DialogPages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DialogPageSections",
                table: "DialogPageSections",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DialogPages",
                table: "DialogPages",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ConsumableItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    HpEffect = table.Column<int>(type: "int", nullable: false),
                    MpEffect = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumableItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsumableItems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JourneyIntroPages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Config = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreviewPhotoUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    JourneyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JourneyIntroPages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JourneyIntroPages_Journeys_JourneyId",
                        column: x => x.JourneyId,
                        principalTable: "Journeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JourneyPlaythroughCharacters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MeleeAttackDamage = table.Column<int>(type: "int", nullable: true),
                    BowAttackDamage = table.Column<int>(type: "int", nullable: true),
                    Movement = table.Column<int>(type: "int", nullable: false),
                    MaxConsumableInventory = table.Column<int>(type: "int", nullable: false),
                    MaxEquippableInventory = table.Column<int>(type: "int", nullable: false),
                    CurrentHp = table.Column<int>(type: "int", nullable: false),
                    CurrentMp = table.Column<int>(type: "int", nullable: false),
                    MaxHp = table.Column<int>(type: "int", nullable: false),
                    MaxMp = table.Column<int>(type: "int", nullable: false),
                    IsDown = table.Column<bool>(type: "bit", nullable: false),
                    JourneyPlaythroughId = table.Column<int>(type: "int", nullable: false),
                    AlternateFormId = table.Column<int>(type: "int", nullable: true),
                    IsInAlternateForm = table.Column<bool>(type: "bit", nullable: false),
                    JourneyCharacterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JourneyPlaythroughCharacters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JourneyPlaythroughCharacters_JourneyCharacters_JourneyCharacterId",
                        column: x => x.JourneyCharacterId,
                        principalTable: "JourneyCharacters",
                        principalColumn: "Id");
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
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    EventTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    JourneyPlaythroughId = table.Column<int>(type: "int", nullable: false)
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
                name: "SceneCharacterSpells",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SceneCharacterId = table.Column<int>(type: "int", nullable: false),
                    SpellId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SceneCharacterSpells", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SceneCharacterSpells_SceneCharacters_SceneCharacterId",
                        column: x => x.SceneCharacterId,
                        principalTable: "SceneCharacters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SceneCharacterSpells_Spells_SpellId",
                        column: x => x.SpellId,
                        principalTable: "Spells",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SceneChests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    DieSides = table.Column<int>(type: "int", nullable: false),
                    SceneId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SceneChests", x => x.Id);
                    table.CheckConstraint("CK_SceneChests_DieSides", "[DieSides] >= 1");
                    table.ForeignKey(
                        name: "FK_SceneChests_Scenes_SceneId",
                        column: x => x.SceneId,
                        principalTable: "Scenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SceneEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    SceneId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SceneEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SceneEvents_Scenes_SceneId",
                        column: x => x.SceneId,
                        principalTable: "Scenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SceneIntroPages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Config = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreviewPhotoUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    SceneId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SceneIntroPages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SceneIntroPages_Scenes_SceneId",
                        column: x => x.SceneId,
                        principalTable: "Scenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JourneyPlaythroughCharacterConsumableItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    ConsumableItemId = table.Column<int>(type: "int", nullable: false),
                    JourneyPlaythroughCharacterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JourneyPlaythroughCharacterConsumableItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JourneyPlaythroughCharacterConsumableItems_ConsumableItems_ConsumableItemId",
                        column: x => x.ConsumableItemId,
                        principalTable: "ConsumableItems",
                        principalColumn: "Id");
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
                    EquippableItemId = table.Column<int>(type: "int", nullable: false),
                    JourneyPlaythroughCharacterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JourneyPlaythroughCharacterEquippableItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JourneyPlaythroughCharacterEquippableItems_EquippableItems_EquippableItemId",
                        column: x => x.EquippableItemId,
                        principalTable: "EquippableItems",
                        principalColumn: "Id");
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
                    JourneyPlaythroughCharacterId = table.Column<int>(type: "int", nullable: false),
                    JourneyCharacterSpellId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JourneyPlaythroughCharacterSpells", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JourneyPlaythroughCharacterSpells_JourneyCharacterSpells_JourneyCharacterSpellId",
                        column: x => x.JourneyCharacterSpellId,
                        principalTable: "JourneyCharacterSpells",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JourneyPlaythroughCharacterSpells_JourneyPlaythroughCharacters_JourneyPlaythroughCharacterId",
                        column: x => x.JourneyPlaythroughCharacterId,
                        principalTable: "JourneyPlaythroughCharacters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SceneChestLootEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RollMinimum = table.Column<int>(type: "int", nullable: false),
                    RollMaximum = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    EquippableItemId = table.Column<int>(type: "int", nullable: true),
                    ConsumableItemId = table.Column<int>(type: "int", nullable: true),
                    SceneChestId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SceneChestLootEntries", x => x.Id);
                    table.CheckConstraint("CK_SceneChestLootEntries_Item", "([EquippableItemId] IS NOT NULL AND [ConsumableItemId] IS NULL) OR ([EquippableItemId] IS NULL AND [ConsumableItemId] IS NOT NULL)");
                    table.CheckConstraint("CK_SceneChestLootEntries_Quantity", "[Quantity] >= 1");
                    table.CheckConstraint("CK_SceneChestLootEntries_RollRange", "[RollMinimum] >= 1 AND [RollMaximum] >= [RollMinimum]");
                    table.ForeignKey(
                        name: "FK_SceneChestLootEntries_ConsumableItems_ConsumableItemId",
                        column: x => x.ConsumableItemId,
                        principalTable: "ConsumableItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SceneChestLootEntries_EquippableItems_EquippableItemId",
                        column: x => x.EquippableItemId,
                        principalTable: "EquippableItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SceneChestLootEntries_SceneChests_SceneChestId",
                        column: x => x.SceneChestId,
                        principalTable: "SceneChests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SceneEventActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    ActionTargetType = table.Column<int>(type: "int", nullable: false),
                    EventActionType = table.Column<int>(type: "int", nullable: false),
                    SceneEventId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SceneEventActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SceneEventActions_SceneEvents_SceneEventId",
                        column: x => x.SceneEventId,
                        principalTable: "SceneEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterStatAdjustmentActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharacterStatType = table.Column<int>(type: "int", nullable: false),
                    AdjustmentOperation = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false),
                    CharacterId = table.Column<int>(type: "int", nullable: true),
                    SceneEventActionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterStatAdjustmentActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterStatAdjustmentActions_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CharacterStatAdjustmentActions_SceneEventActions_SceneEventActionId",
                        column: x => x.SceneEventActionId,
                        principalTable: "SceneEventActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenePlaythroughCharacterConsumableItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    ConsumableItemId = table.Column<int>(type: "int", nullable: false),
                    ScenePlaythroughCharacterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePlaythroughCharacterConsumableItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenePlaythroughCharacterConsumableItems_ConsumableItems_ConsumableItemId",
                        column: x => x.ConsumableItemId,
                        principalTable: "ConsumableItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ScenePlaythroughCharacterEquippableItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquippableItemId = table.Column<int>(type: "int", nullable: false),
                    ScenePlaythroughCharacterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePlaythroughCharacterEquippableItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenePlaythroughCharacterEquippableItems_EquippableItems_EquippableItemId",
                        column: x => x.EquippableItemId,
                        principalTable: "EquippableItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ScenePlaythroughCharacters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MeleeAttackDamage = table.Column<int>(type: "int", nullable: true),
                    BowAttackDamage = table.Column<int>(type: "int", nullable: true),
                    Movement = table.Column<int>(type: "int", nullable: false),
                    MaxConsumableInventory = table.Column<int>(type: "int", nullable: false),
                    MaxEquippableInventory = table.Column<int>(type: "int", nullable: false),
                    CurrentHp = table.Column<int>(type: "int", nullable: false),
                    CurrentMp = table.Column<int>(type: "int", nullable: false),
                    MaxHp = table.Column<int>(type: "int", nullable: false),
                    MaxMp = table.Column<int>(type: "int", nullable: false),
                    IsDead = table.Column<bool>(type: "bit", nullable: false),
                    ScenePlaythroughId = table.Column<int>(type: "int", nullable: false),
                    AlternateFormId = table.Column<int>(type: "int", nullable: true),
                    IsInAlternateForm = table.Column<bool>(type: "bit", nullable: false),
                    SceneCharacterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePlaythroughCharacters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenePlaythroughCharacters_SceneCharacters_SceneCharacterId",
                        column: x => x.SceneCharacterId,
                        principalTable: "SceneCharacters",
                        principalColumn: "Id");
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
                    ScenePlaythroughCharacterId = table.Column<int>(type: "int", nullable: false),
                    SceneCharacterSpellId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePlaythroughCharacterSpells", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenePlaythroughCharacterSpells_SceneCharacterSpells_SceneCharacterSpellId",
                        column: x => x.SceneCharacterSpellId,
                        principalTable: "SceneCharacterSpells",
                        principalColumn: "Id");
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
                    Status = table.Column<int>(type: "int", nullable: false),
                    RolledValue = table.Column<int>(type: "int", nullable: true),
                    OpenedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SelectedLootEntryId = table.Column<int>(type: "int", nullable: true),
                    ScenePlaythroughId = table.Column<int>(type: "int", nullable: false),
                    SceneChestId = table.Column<int>(type: "int", nullable: false)
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
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ScenePlaythroughEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExecutionStatus = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ScenePlaythroughId = table.Column<int>(type: "int", nullable: false),
                    SceneEventId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePlaythroughEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenePlaythroughEvents_SceneEvents_SceneEventId",
                        column: x => x.SceneEventId,
                        principalTable: "SceneEvents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ScenePlaythroughParticipants",
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
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CurrentParticipantId = table.Column<int>(type: "int", nullable: true),
                    RoundNumber = table.Column<int>(type: "int", nullable: false),
                    SceneId = table.Column<int>(type: "int", nullable: false),
                    JourneyPlaythroughId = table.Column<int>(type: "int", nullable: false)
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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Spells_UserId_Name",
                table: "Spells",
                columns: new[] { "UserId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Series_UserId_Name",
                table: "Series",
                columns: new[] { "UserId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Scenes_JourneyId_SortOrder",
                table: "Scenes",
                columns: new[] { "JourneyId", "SortOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SceneCharacters_AlternateFormId",
                table: "SceneCharacters",
                column: "AlternateFormId");

            migrationBuilder.CreateIndex(
                name: "IX_SceneCharacters_SceneId_CharacterId",
                table: "SceneCharacters",
                columns: new[] { "SceneId", "CharacterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Journeys_SeriesId_SortOrder",
                table: "Journeys",
                columns: new[] { "SeriesId", "SortOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JourneyCharacters_JourneyId_CharacterId",
                table: "JourneyCharacters",
                columns: new[] { "JourneyId", "CharacterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquippableItems_UserId_Name",
                table: "EquippableItems",
                columns: new[] { "UserId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterSpells_CharacterId_SpellId",
                table: "CharacterSpells",
                columns: new[] { "CharacterId", "SpellId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DialogPageSections_DialogPageId_OrderNum",
                table: "DialogPageSections",
                columns: new[] { "DialogPageId", "OrderNum" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DialogPages_SceneDialogId_OrderNum",
                table: "DialogPages",
                columns: new[] { "SceneDialogId", "OrderNum" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterStatAdjustmentActions_CharacterId",
                table: "CharacterStatAdjustmentActions",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterStatAdjustmentActions_SceneEventActionId",
                table: "CharacterStatAdjustmentActions",
                column: "SceneEventActionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConsumableItems_UserId_Name",
                table: "ConsumableItems",
                columns: new[] { "UserId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JourneyIntroPages_JourneyId_SortOrder",
                table: "JourneyIntroPages",
                columns: new[] { "JourneyId", "SortOrder" },
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
                name: "IX_JourneyPlaythroughCharacters_JourneyPlaythroughId_JourneyCharacterId",
                table: "JourneyPlaythroughCharacters",
                columns: new[] { "JourneyPlaythroughId", "JourneyCharacterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JourneyPlaythroughCharacterSpells_JourneyCharacterSpellId",
                table: "JourneyPlaythroughCharacterSpells",
                column: "JourneyCharacterSpellId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyPlaythroughCharacterSpells_JourneyPlaythroughCharacterId_JourneyCharacterSpellId",
                table: "JourneyPlaythroughCharacterSpells",
                columns: new[] { "JourneyPlaythroughCharacterId", "JourneyCharacterSpellId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JourneyPlaythroughEventLogs_JourneyPlaythroughId_EventTime",
                table: "JourneyPlaythroughEventLogs",
                columns: new[] { "JourneyPlaythroughId", "EventTime" });

            migrationBuilder.CreateIndex(
                name: "IX_SceneCharacterSpells_SceneCharacterId_SpellId",
                table: "SceneCharacterSpells",
                columns: new[] { "SceneCharacterId", "SpellId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SceneCharacterSpells_SpellId",
                table: "SceneCharacterSpells",
                column: "SpellId");

            migrationBuilder.CreateIndex(
                name: "IX_SceneChestLootEntries_ConsumableItemId",
                table: "SceneChestLootEntries",
                column: "ConsumableItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SceneChestLootEntries_EquippableItemId",
                table: "SceneChestLootEntries",
                column: "EquippableItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SceneChestLootEntries_SceneChestId",
                table: "SceneChestLootEntries",
                column: "SceneChestId");

            migrationBuilder.CreateIndex(
                name: "IX_SceneChests_SceneId_Name",
                table: "SceneChests",
                columns: new[] { "SceneId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SceneEventActions_SceneEventId_SortOrder",
                table: "SceneEventActions",
                columns: new[] { "SceneEventId", "SortOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SceneEvents_SceneId_SortOrder",
                table: "SceneEvents",
                columns: new[] { "SceneId", "SortOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SceneIntroPages_SceneId_SortOrder",
                table: "SceneIntroPages",
                columns: new[] { "SceneId", "SortOrder" },
                unique: true);

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
                name: "IX_ScenePlaythroughCharacters_ScenePlaythroughId_SceneCharacterId",
                table: "ScenePlaythroughCharacters",
                columns: new[] { "ScenePlaythroughId", "SceneCharacterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenePlaythroughCharacterSpells_SceneCharacterSpellId",
                table: "ScenePlaythroughCharacterSpells",
                column: "SceneCharacterSpellId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePlaythroughCharacterSpells_ScenePlaythroughCharacterId_SceneCharacterSpellId",
                table: "ScenePlaythroughCharacterSpells",
                columns: new[] { "ScenePlaythroughCharacterId", "SceneCharacterSpellId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenePlaythroughChests_SceneChestId",
                table: "ScenePlaythroughChests",
                column: "SceneChestId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenePlaythroughChests_ScenePlaythroughId_SceneChestId",
                table: "ScenePlaythroughChests",
                columns: new[] { "ScenePlaythroughId", "SceneChestId" },
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
                name: "IX_ScenePlaythroughEvents_ScenePlaythroughId_SceneEventId",
                table: "ScenePlaythroughEvents",
                columns: new[] { "ScenePlaythroughId", "SceneEventId" },
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
                name: "IX_ScenePlaythroughs_JourneyPlaythroughId_SceneId",
                table: "ScenePlaythroughs",
                columns: new[] { "JourneyPlaythroughId", "SceneId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenePlaythroughs_SceneId",
                table: "ScenePlaythroughs",
                column: "SceneId");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterSpells_Spells_SpellId",
                table: "CharacterSpells",
                column: "SpellId",
                principalTable: "Spells",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DialogPages_SceneDialogs_SceneDialogId",
                table: "DialogPages",
                column: "SceneDialogId",
                principalTable: "SceneDialogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DialogPageSections_Characters_CharacterId",
                table: "DialogPageSections",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_DialogPageSections_DialogPages_DialogPageId",
                table: "DialogPageSections",
                column: "DialogPageId",
                principalTable: "DialogPages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JourneyCharacters_Characters_AlternateFormId",
                table: "JourneyCharacters",
                column: "AlternateFormId",
                principalTable: "Characters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JourneyCharacters_Characters_CharacterId",
                table: "JourneyCharacters",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JourneyCharacterSpells_Spells_SpellId",
                table: "JourneyCharacterSpells",
                column: "SpellId",
                principalTable: "Spells",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Journeys_Users_UserId",
                table: "Journeys",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SceneCharacters_Characters_AlternateFormId",
                table: "SceneCharacters",
                column: "AlternateFormId",
                principalTable: "Characters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SceneCharacters_Characters_CharacterId",
                table: "SceneCharacters",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Series_Users_UserId",
                table: "Series",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterSpells_Spells_SpellId",
                table: "CharacterSpells");

            migrationBuilder.DropForeignKey(
                name: "FK_DialogPages_SceneDialogs_SceneDialogId",
                table: "DialogPages");

            migrationBuilder.DropForeignKey(
                name: "FK_DialogPageSections_Characters_CharacterId",
                table: "DialogPageSections");

            migrationBuilder.DropForeignKey(
                name: "FK_DialogPageSections_DialogPages_DialogPageId",
                table: "DialogPageSections");

            migrationBuilder.DropForeignKey(
                name: "FK_JourneyCharacters_Characters_AlternateFormId",
                table: "JourneyCharacters");

            migrationBuilder.DropForeignKey(
                name: "FK_JourneyCharacters_Characters_CharacterId",
                table: "JourneyCharacters");

            migrationBuilder.DropForeignKey(
                name: "FK_JourneyCharacterSpells_Spells_SpellId",
                table: "JourneyCharacterSpells");

            migrationBuilder.DropForeignKey(
                name: "FK_Journeys_Users_UserId",
                table: "Journeys");

            migrationBuilder.DropForeignKey(
                name: "FK_SceneCharacters_Characters_AlternateFormId",
                table: "SceneCharacters");

            migrationBuilder.DropForeignKey(
                name: "FK_SceneCharacters_Characters_CharacterId",
                table: "SceneCharacters");

            migrationBuilder.DropForeignKey(
                name: "FK_Series_Users_UserId",
                table: "Series");

            migrationBuilder.DropForeignKey(
                name: "FK_ScenePlaythroughParticipants_JourneyPlaythroughCharacters_JourneyPlaythroughCharacterId",
                table: "ScenePlaythroughParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_ScenePlaythroughParticipants_ScenePlaythroughCharacters_ScenePlaythroughCharacterId",
                table: "ScenePlaythroughParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_ScenePlaythroughParticipants_ScenePlaythroughs_ScenePlaythroughId",
                table: "ScenePlaythroughParticipants");

            migrationBuilder.DropTable(
                name: "CharacterStatAdjustmentActions");

            migrationBuilder.DropTable(
                name: "JourneyIntroPages");

            migrationBuilder.DropTable(
                name: "JourneyPlaythroughCharacterConsumableItems");

            migrationBuilder.DropTable(
                name: "JourneyPlaythroughCharacterEquippableItems");

            migrationBuilder.DropTable(
                name: "JourneyPlaythroughCharacterSpells");

            migrationBuilder.DropTable(
                name: "JourneyPlaythroughEventLogs");

            migrationBuilder.DropTable(
                name: "SceneIntroPages");

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
                name: "SceneEventActions");

            migrationBuilder.DropTable(
                name: "SceneCharacterSpells");

            migrationBuilder.DropTable(
                name: "SceneChestLootEntries");

            migrationBuilder.DropTable(
                name: "SceneEvents");

            migrationBuilder.DropTable(
                name: "ConsumableItems");

            migrationBuilder.DropTable(
                name: "SceneChests");

            migrationBuilder.DropTable(
                name: "JourneyPlaythroughCharacters");

            migrationBuilder.DropTable(
                name: "ScenePlaythroughCharacters");

            migrationBuilder.DropTable(
                name: "ScenePlaythroughs");

            migrationBuilder.DropTable(
                name: "ScenePlaythroughParticipants");

            migrationBuilder.DropIndex(
                name: "IX_Spells_UserId_Name",
                table: "Spells");

            migrationBuilder.DropIndex(
                name: "IX_Series_UserId_Name",
                table: "Series");

            migrationBuilder.DropIndex(
                name: "IX_Scenes_JourneyId_SortOrder",
                table: "Scenes");

            migrationBuilder.DropIndex(
                name: "IX_SceneCharacters_AlternateFormId",
                table: "SceneCharacters");

            migrationBuilder.DropIndex(
                name: "IX_SceneCharacters_SceneId_CharacterId",
                table: "SceneCharacters");

            migrationBuilder.DropIndex(
                name: "IX_Journeys_SeriesId_SortOrder",
                table: "Journeys");

            migrationBuilder.DropIndex(
                name: "IX_JourneyCharacters_JourneyId_CharacterId",
                table: "JourneyCharacters");

            migrationBuilder.DropIndex(
                name: "IX_EquippableItems_UserId_Name",
                table: "EquippableItems");

            migrationBuilder.DropIndex(
                name: "IX_CharacterSpells_CharacterId_SpellId",
                table: "CharacterSpells");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DialogPageSections",
                table: "DialogPageSections");

            migrationBuilder.DropIndex(
                name: "IX_DialogPageSections_DialogPageId_OrderNum",
                table: "DialogPageSections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DialogPages",
                table: "DialogPages");

            migrationBuilder.DropIndex(
                name: "IX_DialogPages_SceneDialogId_OrderNum",
                table: "DialogPages");

            migrationBuilder.DropColumn(
                name: "AlternateFormId",
                table: "SceneCharacters");

            migrationBuilder.DropColumn(
                name: "BowAttackDamage",
                table: "SceneCharacters");

            migrationBuilder.DropColumn(
                name: "MaxConsumableInventory",
                table: "SceneCharacters");

            migrationBuilder.DropColumn(
                name: "MaxEquippableInventory",
                table: "SceneCharacters");

            migrationBuilder.DropColumn(
                name: "MaxHp",
                table: "SceneCharacters");

            migrationBuilder.DropColumn(
                name: "MeleeAttackDamage",
                table: "SceneCharacters");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Journeys");

            migrationBuilder.DropColumn(
                name: "CharacterType",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "PortraitUrl",
                table: "Characters");

            migrationBuilder.RenameTable(
                name: "DialogPageSections",
                newName: "DialogPageSection");

            migrationBuilder.RenameTable(
                name: "DialogPages",
                newName: "DialogPage");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Users",
                newName: "UpdateDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Users",
                newName: "CreateDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Spells",
                newName: "UpdateDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Spells",
                newName: "CreateDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Series",
                newName: "UpdateDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Series",
                newName: "CreateDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Scenes",
                newName: "UpdateDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Scenes",
                newName: "CreateDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "SceneDialogs",
                newName: "UpdateDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "SceneDialogs",
                newName: "CreateDate");

            migrationBuilder.RenameColumn(
                name: "Movement",
                table: "SceneCharacters",
                newName: "CurrentMp");

            migrationBuilder.RenameColumn(
                name: "MaxMp",
                table: "SceneCharacters",
                newName: "CurrentHp");

            migrationBuilder.RenameColumn(
                name: "IsInitiallyActive",
                table: "SceneCharacters",
                newName: "IsDown");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Journeys",
                newName: "UpdateDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Journeys",
                newName: "CreateDate");

            migrationBuilder.RenameColumn(
                name: "IsInitiallyActive",
                table: "JourneyCharacters",
                newName: "IsInAlternateForm");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "EquippableItems",
                newName: "UpdateDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "EquippableItems",
                newName: "CreateDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Characters",
                newName: "UpdateDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Characters",
                newName: "CreateDate");

            migrationBuilder.RenameColumn(
                name: "DialogInActiveColor",
                table: "CharacterDialogSettings",
                newName: "DialogUnActiveColor");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "DialogPageSection",
                newName: "UpdateDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "DialogPageSection",
                newName: "CreateDate");

            migrationBuilder.RenameIndex(
                name: "IX_DialogPageSections_CharacterId",
                table: "DialogPageSection",
                newName: "IX_DialogPageSection_CharacterId");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "DialogPage",
                newName: "UpdateDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "DialogPage",
                newName: "CreateDate");

            migrationBuilder.AlterColumn<string>(
                name: "PhotoUrl",
                table: "Scenes",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GridUrl",
                table: "Scenes",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "Scenes",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Scenes",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "SceneDialogs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AddColumn<bool>(
                name: "IsAlternateForm",
                table: "SceneCharacters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CurrentHp",
                table: "JourneyCharacters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrentMp",
                table: "JourneyCharacters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDown",
                table: "JourneyCharacters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "SpellDamageModifier",
                table: "EquippableItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PortraitFileName",
                table: "Characters",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnemy",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsNPC",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPlayer",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Portrait",
                table: "Characters",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "PhotoUrl",
                table: "DialogPage",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "DialogPage",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DialogPageSection",
                table: "DialogPageSection",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DialogPage",
                table: "DialogPage",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "IntroPage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JourneyId = table.Column<int>(type: "int", nullable: false),
                    Config = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    PreviewPhotoUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntroPage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IntroPage_Journeys_JourneyId",
                        column: x => x.JourneyId,
                        principalTable: "Journeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    HpEffect = table.Column<int>(type: "int", nullable: false),
                    MpEffect = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JourneyCharacterEquippableItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquippableItemId = table.Column<int>(type: "int", nullable: false),
                    JourneyCharacterId = table.Column<int>(type: "int", nullable: false),
                    IsEquipped = table.Column<bool>(type: "bit", nullable: false)
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
                name: "SceneProgresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JourneyPlaythroughId = table.Column<int>(type: "int", nullable: false),
                    SceneId = table.Column<int>(type: "int", nullable: false),
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
                name: "JourneyCharacterItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    JourneyCharacterId = table.Column<int>(type: "int", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JourneyCharacterItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JourneyCharacterItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JourneyCharacterItems_JourneyCharacters_JourneyCharacterId",
                        column: x => x.JourneyCharacterId,
                        principalTable: "JourneyCharacters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SceneCharacterItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    SceneCharacterId = table.Column<int>(type: "int", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SceneCharacterItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SceneCharacterItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SceneCharacterItems_SceneCharacters_SceneCharacterId",
                        column: x => x.SceneCharacterId,
                        principalTable: "SceneCharacters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SceneParticipants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JourneyCharacterId = table.Column<int>(type: "int", nullable: true),
                    SceneCharacterId = table.Column<int>(type: "int", nullable: true),
                    SceneProgressId = table.Column<int>(type: "int", nullable: false)
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
                name: "SceneParticipantTurns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SceneParticipantId = table.Column<int>(type: "int", nullable: false),
                    SceneProgressId = table.Column<int>(type: "int", nullable: false),
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
                name: "IX_Spells_UserId",
                table: "Spells",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Series_UserId",
                table: "Series",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Scenes_JourneyId",
                table: "Scenes",
                column: "JourneyId");

            migrationBuilder.CreateIndex(
                name: "IX_SceneCharacters_SceneId",
                table: "SceneCharacters",
                column: "SceneId");

            migrationBuilder.CreateIndex(
                name: "IX_Journeys_SeriesId",
                table: "Journeys",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyCharacters_JourneyId",
                table: "JourneyCharacters",
                column: "JourneyId");

            migrationBuilder.CreateIndex(
                name: "IX_EquippableItems_UserId",
                table: "EquippableItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterSpells_CharacterId",
                table: "CharacterSpells",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_DialogPageSection_DialogPageId",
                table: "DialogPageSection",
                column: "DialogPageId");

            migrationBuilder.CreateIndex(
                name: "IX_DialogPage_SceneDialogId",
                table: "DialogPage",
                column: "SceneDialogId");

            migrationBuilder.CreateIndex(
                name: "IX_IntroPage_JourneyId_Order",
                table: "IntroPage",
                columns: new[] { "JourneyId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_UserId",
                table: "Items",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyCharacterEquippableItems_EquippableItemId",
                table: "JourneyCharacterEquippableItems",
                column: "EquippableItemId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyCharacterEquippableItems_JourneyCharacterId",
                table: "JourneyCharacterEquippableItems",
                column: "JourneyCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyCharacterItems_ItemId",
                table: "JourneyCharacterItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyCharacterItems_JourneyCharacterId",
                table: "JourneyCharacterItems",
                column: "JourneyCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_SceneCharacterItems_ItemId",
                table: "SceneCharacterItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SceneCharacterItems_SceneCharacterId",
                table: "SceneCharacterItems",
                column: "SceneCharacterId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterSpells_Spells_SpellId",
                table: "CharacterSpells",
                column: "SpellId",
                principalTable: "Spells",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DialogPage_SceneDialogs_SceneDialogId",
                table: "DialogPage",
                column: "SceneDialogId",
                principalTable: "SceneDialogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DialogPageSection_Characters_CharacterId",
                table: "DialogPageSection",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_DialogPageSection_DialogPage_DialogPageId",
                table: "DialogPageSection",
                column: "DialogPageId",
                principalTable: "DialogPage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JourneyCharacters_Characters_AlternateFormId",
                table: "JourneyCharacters",
                column: "AlternateFormId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JourneyCharacters_Characters_CharacterId",
                table: "JourneyCharacters",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JourneyCharacterSpells_Spells_SpellId",
                table: "JourneyCharacterSpells",
                column: "SpellId",
                principalTable: "Spells",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Journeys_Users_UserId",
                table: "Journeys",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SceneCharacters_Characters_CharacterId",
                table: "SceneCharacters",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Series_Users_UserId",
                table: "Series",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
