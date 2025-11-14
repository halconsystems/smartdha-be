using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FK_ClubIdinServiceResTypeRoomCat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Services_ClubId",
                table: "Services",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomCategories_ClubId",
                table: "RoomCategories",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_ResidenceTypes_ClubId",
                table: "ResidenceTypes",
                column: "ClubId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResidenceTypes_Clubs_ClubId",
                table: "ResidenceTypes",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomCategories_Clubs_ClubId",
                table: "RoomCategories",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Clubs_ClubId",
                table: "Services",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResidenceTypes_Clubs_ClubId",
                table: "ResidenceTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomCategories_Clubs_ClubId",
                table: "RoomCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_Clubs_ClubId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_ClubId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_RoomCategories_ClubId",
                table: "RoomCategories");

            migrationBuilder.DropIndex(
                name: "IX_ResidenceTypes_ClubId",
                table: "ResidenceTypes");
        }
    }
}
