using Eldoria.Infrastructure.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eldoria.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260809000000_AllowDuplicateSceneCharacters")]
    public partial class AllowDuplicateSceneCharacters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SceneCharacters_SceneId_CharacterId",
                table: "SceneCharacters");

            migrationBuilder.CreateIndex(
                name: "IX_SceneCharacters_SceneId_CharacterId",
                table: "SceneCharacters",
                columns: new[] { "SceneId", "CharacterId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SceneCharacters_SceneId_CharacterId",
                table: "SceneCharacters");

            migrationBuilder.CreateIndex(
                name: "IX_SceneCharacters_SceneId_CharacterId",
                table: "SceneCharacters",
                columns: new[] { "SceneId", "CharacterId" },
                unique: true);
        }
    }
}
