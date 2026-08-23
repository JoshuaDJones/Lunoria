using Eldoria.Infrastructure.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eldoria.Infrastructure.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260822170000_AllowDuplicateScenePlaythroughCharacterInstances")]
public sealed class AllowDuplicateScenePlaythroughCharacterInstances : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_ScenePTCharacters_ScenePlaythroughId_SourceSceneCharacterId",
            table: "ScenePTCharacters");

        migrationBuilder.AlterColumn<int>(
            name: "SourceSceneCharacterId",
            table: "ScenePTCharacters",
            type: "int",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.CreateIndex(
            name: "IX_ScenePTCharacters_ScenePlaythroughId_SourceSceneCharacterId",
            table: "ScenePTCharacters",
            columns: new[] { "ScenePlaythroughId", "SourceSceneCharacterId" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_ScenePTCharacters_ScenePlaythroughId_SourceSceneCharacterId",
            table: "ScenePTCharacters");

        migrationBuilder.Sql(
            "UPDATE [ScenePTCharacters] SET [SourceSceneCharacterId] = -[Id] " +
            "WHERE [SourceSceneCharacterId] IS NULL");

        migrationBuilder.AlterColumn<int>(
            name: "SourceSceneCharacterId",
            table: "ScenePTCharacters",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_ScenePTCharacters_ScenePlaythroughId_SourceSceneCharacterId",
            table: "ScenePTCharacters",
            columns: new[] { "ScenePlaythroughId", "SourceSceneCharacterId" },
            unique: true);
    }
}
