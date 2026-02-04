using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Data.SQLite
{
    /// <inheritdoc />
    public partial class Clubmbershiptchnagesuserid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ClubMemberships",
                type: "nvarchar(85)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClubMemberships_UserId",
                table: "ClubMemberships",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClubMemberships_Users_UserId",
                table: "ClubMemberships",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClubMemberships_Users_UserId",
                table: "ClubMemberships");

            migrationBuilder.DropIndex(
                name: "IX_ClubMemberships_UserId",
                table: "ClubMemberships");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ClubMemberships");
        }
    }
}
