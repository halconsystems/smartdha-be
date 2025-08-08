using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RoomRatingCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomRatings",
                table: "RoomRatings");

            migrationBuilder.RenameTable(
                name: "RoomRatings",
                newName: "CBMS_RoomRatings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CBMS_RoomRatings",
                table: "CBMS_RoomRatings",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CBMS_RoomRatings",
                table: "CBMS_RoomRatings");

            migrationBuilder.RenameTable(
                name: "CBMS_RoomRatings",
                newName: "RoomRatings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomRatings",
                table: "RoomRatings",
                column: "Id");
        }
    }
}
