using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eldoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovePlaythroughSortOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Playthroughs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Playthroughs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
