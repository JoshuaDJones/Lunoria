using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eldoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSceneObjectives : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentObjectiveIndex",
                table: "ScenePTs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SceneObjectives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SceneId = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SceneObjectives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SceneObjectives_Scenes_SceneId",
                        column: x => x.SceneId,
                        principalTable: "Scenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenePTObjectives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScenePTId = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePTObjectives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenePTObjectives_ScenePTs_ScenePTId",
                        column: x => x.ScenePTId,
                        principalTable: "ScenePTs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SceneObjectivePoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SceneObjectiveId = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Text = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SceneObjectivePoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SceneObjectivePoints_SceneObjectives_SceneObjectiveId",
                        column: x => x.SceneObjectiveId,
                        principalTable: "SceneObjectives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenePTObjectivePoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScenePTObjectiveId = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Text = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenePTObjectivePoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenePTObjectivePoints_ScenePTObjectives_ScenePTObjectiveId",
                        column: x => x.ScenePTObjectiveId,
                        principalTable: "ScenePTObjectives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SceneObjectivePoints_SceneObjectiveId_SortOrder",
                table: "SceneObjectivePoints",
                columns: new[] { "SceneObjectiveId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_SceneObjectives_SceneId_SortOrder",
                table: "SceneObjectives",
                columns: new[] { "SceneId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTObjectivePoints_ScenePTObjectiveId_SortOrder",
                table: "ScenePTObjectivePoints",
                columns: new[] { "ScenePTObjectiveId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_ScenePTObjectives_ScenePTId_SortOrder",
                table: "ScenePTObjectives",
                columns: new[] { "ScenePTId", "SortOrder" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SceneObjectivePoints");

            migrationBuilder.DropTable(
                name: "ScenePTObjectivePoints");

            migrationBuilder.DropTable(
                name: "SceneObjectives");

            migrationBuilder.DropTable(
                name: "ScenePTObjectives");

            migrationBuilder.DropColumn(
                name: "CurrentObjectiveIndex",
                table: "ScenePTs");
        }
    }
}
