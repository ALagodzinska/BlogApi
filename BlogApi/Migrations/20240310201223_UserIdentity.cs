using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApi.Migrations
{
    /// <inheritdoc />
    public partial class UserIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserIdentityId",
                table: "Posts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UserIdentityId",
                table: "Posts",
                column: "UserIdentityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_UserIdentityId",
                table: "Posts",
                column: "UserIdentityId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_UserIdentityId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_UserIdentityId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "UserIdentityId",
                table: "Posts");
        }
    }
}
