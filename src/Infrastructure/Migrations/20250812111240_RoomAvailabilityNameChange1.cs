using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RoomAvailabilityNameChange1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomAvailability_Rooms_RoomId",
                table: "RoomAvailability");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomAvailability",
                table: "RoomAvailability");

            migrationBuilder.RenameTable(
                name: "RoomAvailability",
                newName: "RoomAvailabilities");

            migrationBuilder.RenameIndex(
                name: "IX_RoomAvailability_RoomId_FromDate_ToDate",
                table: "RoomAvailabilities",
                newName: "IX_RoomAvailabilities_RoomId_FromDate_ToDate");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomAvailabilities",
                table: "RoomAvailabilities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomAvailabilities_Rooms_RoomId",
                table: "RoomAvailabilities",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomAvailabilities_Rooms_RoomId",
                table: "RoomAvailabilities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomAvailabilities",
                table: "RoomAvailabilities");

            migrationBuilder.RenameTable(
                name: "RoomAvailabilities",
                newName: "RoomAvailability");

            migrationBuilder.RenameIndex(
                name: "IX_RoomAvailabilities_RoomId_FromDate_ToDate",
                table: "RoomAvailability",
                newName: "IX_RoomAvailability_RoomId_FromDate_ToDate");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomAvailability",
                table: "RoomAvailability",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomAvailability_Rooms_RoomId",
                table: "RoomAvailability",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
