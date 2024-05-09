using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApi.Migrations
{
    /// <inheritdoc />
    public partial class NewColumnNameForImageFormat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PreviewImageType",
                table: "Posts",
                newName: "PreviewImageFormat");

            migrationBuilder.RenameColumn(
                name: "BackgroundImageType",
                table: "Posts",
                newName: "BackgroundImageFormat");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PreviewImageFormat",
                table: "Posts",
                newName: "PreviewImageType");

            migrationBuilder.RenameColumn(
                name: "BackgroundImageFormat",
                table: "Posts",
                newName: "BackgroundImageType");
        }
    }
}
