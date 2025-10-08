using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Data.SQLite
{
    /// <inheritdoc />
    public partial class RestrictDeleteOnClubAssign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserClubAssignments_Users_UserId",
                table: "UserClubAssignments");

            migrationBuilder.AddForeignKey(
                name: "FK_UserClubAssignments_Users_UserId",
                table: "UserClubAssignments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserClubAssignments_Users_UserId",
                table: "UserClubAssignments");

            migrationBuilder.AddForeignKey(
                name: "FK_UserClubAssignments_Users_UserId",
                table: "UserClubAssignments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
