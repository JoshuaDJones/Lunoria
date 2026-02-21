using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eldoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterDialogSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DialogPageSection_Characters_CharacterId1",
                table: "DialogPageSection");

            migrationBuilder.DropIndex(
                name: "IX_DialogPageSection_CharacterId1",
                table: "DialogPageSection");

            migrationBuilder.DropColumn(
                name: "CharacterId1",
                table: "DialogPageSection");

            migrationBuilder.CreateTable(
                name: "CharacterDialogSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DialogActiveColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DialogUnActiveColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CharacterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterDialogSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterDialogSettings_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterDialogSettings_CharacterId",
                table: "CharacterDialogSettings",
                column: "CharacterId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterDialogSettings");

            migrationBuilder.AddColumn<int>(
                name: "CharacterId1",
                table: "DialogPageSection",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DialogPageSection_CharacterId1",
                table: "DialogPageSection",
                column: "CharacterId1");

            migrationBuilder.AddForeignKey(
                name: "FK_DialogPageSection_Characters_CharacterId1",
                table: "DialogPageSection",
                column: "CharacterId1",
                principalTable: "Characters",
                principalColumn: "Id");
        }
    }
}
