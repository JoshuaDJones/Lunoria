using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eldoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class sceneDialogUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SceneDialogs_Characters_CharacterId",
                table: "SceneDialogs");

            migrationBuilder.DropIndex(
                name: "IX_SceneDialogs_CharacterId",
                table: "SceneDialogs");

            migrationBuilder.DropColumn(
                name: "CharacterId",
                table: "SceneDialogs");

            migrationBuilder.DropColumn(
                name: "Dialog",
                table: "SceneDialogs");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "SceneDialogs");

            migrationBuilder.DropColumn(
                name: "OrderNum",
                table: "SceneDialogs");

            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "SceneDialogs");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "SceneDialogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "DialogPage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNum = table.Column<int>(type: "int", nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SceneDialogId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DialogPage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DialogPage_SceneDialogs_SceneDialogId",
                        column: x => x.SceneDialogId,
                        principalTable: "SceneDialogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DialogPageSection",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNum = table.Column<int>(type: "int", nullable: false),
                    ReadingText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsNarrator = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CharacterId = table.Column<int>(type: "int", nullable: true),
                    DialogPageId = table.Column<int>(type: "int", nullable: false),
                    CharacterId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DialogPageSection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DialogPageSection_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DialogPageSection_Characters_CharacterId1",
                        column: x => x.CharacterId1,
                        principalTable: "Characters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DialogPageSection_DialogPage_DialogPageId",
                        column: x => x.DialogPageId,
                        principalTable: "DialogPage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DialogPage_SceneDialogId",
                table: "DialogPage",
                column: "SceneDialogId");

            migrationBuilder.CreateIndex(
                name: "IX_DialogPageSection_CharacterId",
                table: "DialogPageSection",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_DialogPageSection_CharacterId1",
                table: "DialogPageSection",
                column: "CharacterId1");

            migrationBuilder.CreateIndex(
                name: "IX_DialogPageSection_DialogPageId",
                table: "DialogPageSection",
                column: "DialogPageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DialogPageSection");

            migrationBuilder.DropTable(
                name: "DialogPage");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "SceneDialogs");

            migrationBuilder.AddColumn<int>(
                name: "CharacterId",
                table: "SceneDialogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Dialog",
                table: "SceneDialogs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "SceneDialogs",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderNum",
                table: "SceneDialogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "SceneDialogs",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SceneDialogs_CharacterId",
                table: "SceneDialogs",
                column: "CharacterId");

            migrationBuilder.AddForeignKey(
                name: "FK_SceneDialogs_Characters_CharacterId",
                table: "SceneDialogs",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
