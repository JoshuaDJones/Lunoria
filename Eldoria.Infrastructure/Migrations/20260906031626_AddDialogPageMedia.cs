using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eldoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDialogPageMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "ScenePTDialogPages",
                newName: "MediaBlobName");

            migrationBuilder.RenameColumn(
                name: "PhotoUrl",
                table: "ScenePTDialogPages",
                newName: "MediaUrl");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "ScenePTDialogPage",
                newName: "MediaBlobName");

            migrationBuilder.RenameColumn(
                name: "PhotoUrl",
                table: "ScenePTDialogPage",
                newName: "MediaUrl");

            migrationBuilder.AddColumn<string>(
                name: "MediaContentType",
                table: "ScenePTDialogPages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PageType",
                table: "ScenePTDialogPages",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "MediaContentType",
                table: "ScenePTDialogPage",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PageType",
                table: "ScenePTDialogPage",
                type: "int",
                nullable: false,
                defaultValue: 1);

            BackfillMedia(migrationBuilder, "ScenePTDialogPages");
            BackfillMedia(migrationBuilder, "ScenePTDialogPage");

            migrationBuilder.AlterColumn<string>(
                name: "MediaBlobName",
                table: "ScenePTDialogPages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MediaContentType",
                table: "ScenePTDialogPages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MediaUrl",
                table: "ScenePTDialogPages",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MediaBlobName",
                table: "ScenePTDialogPage",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MediaContentType",
                table: "ScenePTDialogPage",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MediaUrl",
                table: "ScenePTDialogPage",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MediaContentType",
                table: "ScenePTDialogPages");

            migrationBuilder.DropColumn(
                name: "PageType",
                table: "ScenePTDialogPages");

            migrationBuilder.DropColumn(
                name: "MediaContentType",
                table: "ScenePTDialogPage");

            migrationBuilder.DropColumn(
                name: "PageType",
                table: "ScenePTDialogPage");

            migrationBuilder.AlterColumn<string>(
                name: "MediaBlobName",
                table: "ScenePTDialogPages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "MediaUrl",
                table: "ScenePTDialogPages",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048);

            migrationBuilder.AlterColumn<string>(
                name: "MediaBlobName",
                table: "ScenePTDialogPage",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "MediaUrl",
                table: "ScenePTDialogPage",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048);

            migrationBuilder.RenameColumn(
                name: "MediaBlobName",
                table: "ScenePTDialogPages",
                newName: "FileName");

            migrationBuilder.RenameColumn(
                name: "MediaUrl",
                table: "ScenePTDialogPages",
                newName: "PhotoUrl");

            migrationBuilder.RenameColumn(
                name: "MediaBlobName",
                table: "ScenePTDialogPage",
                newName: "FileName");

            migrationBuilder.RenameColumn(
                name: "MediaUrl",
                table: "ScenePTDialogPage",
                newName: "PhotoUrl");
        }

        private static void BackfillMedia(
            MigrationBuilder migrationBuilder,
            string tableName)
        {
            migrationBuilder.Sql($$"""
                UPDATE [{{tableName}}]
                SET
                    [MediaContentType] = CASE
                        WHEN LOWER(COALESCE(NULLIF([MediaBlobName], N''), [MediaUrl], N'')) LIKE N'%.jpg' THEN N'image/jpeg'
                        WHEN LOWER(COALESCE(NULLIF([MediaBlobName], N''), [MediaUrl], N'')) LIKE N'%.jpeg' THEN N'image/jpeg'
                        WHEN LOWER(COALESCE(NULLIF([MediaBlobName], N''), [MediaUrl], N'')) LIKE N'%.png' THEN N'image/png'
                        WHEN LOWER(COALESCE(NULLIF([MediaBlobName], N''), [MediaUrl], N'')) LIKE N'%.gif' THEN N'image/gif'
                        WHEN LOWER(COALESCE(NULLIF([MediaBlobName], N''), [MediaUrl], N'')) LIKE N'%.webp' THEN N'image/webp'
                        WHEN LOWER(COALESCE(NULLIF([MediaBlobName], N''), [MediaUrl], N'')) LIKE N'%.avif' THEN N'image/avif'
                        WHEN LOWER(COALESCE(NULLIF([MediaBlobName], N''), [MediaUrl], N'')) LIKE N'%.svg' THEN N'image/svg+xml'
                        WHEN LOWER(COALESCE(NULLIF([MediaBlobName], N''), [MediaUrl], N'')) LIKE N'%.bmp' THEN N'image/bmp'
                        WHEN LOWER(COALESCE(NULLIF([MediaBlobName], N''), [MediaUrl], N'')) LIKE N'%.tif' THEN N'image/tiff'
                        WHEN LOWER(COALESCE(NULLIF([MediaBlobName], N''), [MediaUrl], N'')) LIKE N'%.tiff' THEN N'image/tiff'
                        ELSE N'application/octet-stream'
                    END,
                    [MediaUrl] = COALESCE([MediaUrl], N''),
                    [MediaBlobName] = COALESCE([MediaBlobName], N'');
                """);
        }
    }
}
