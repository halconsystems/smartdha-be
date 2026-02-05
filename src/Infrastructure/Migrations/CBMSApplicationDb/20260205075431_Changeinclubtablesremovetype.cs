using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.CBMSApplicationDb
{
    /// <inheritdoc />
    public partial class Changeinclubtablesremovetype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Clubs_ClubId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_ClubFacilities_Clubs_ClubId",
                table: "ClubFacilities");

            migrationBuilder.DropForeignKey(
                name: "FK_FacilityUnits_Clubs_ClubId",
                table: "FacilityUnits");

            migrationBuilder.DropColumn(
                name: "ClubType",
                table: "Clubs");

            migrationBuilder.AddColumn<string>(
                name: "MarchantCode",
                table: "Clubs",
                type: "nvarchar(max)",
                nullable: true);

           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Club_ClubId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_ClubFacilities_Club_ClubId",
                table: "ClubFacilities");

            migrationBuilder.DropForeignKey(
                name: "FK_FacilityUnits_Club_ClubId",
                table: "FacilityUnits");

            migrationBuilder.DropTable(
                name: "Club");

            migrationBuilder.DropColumn(
                name: "MarchantCode",
                table: "Clubs");

            migrationBuilder.AddColumn<int>(
                name: "ClubType",
                table: "Clubs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Clubs_ClubId",
                table: "Bookings",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClubFacilities_Clubs_ClubId",
                table: "ClubFacilities",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FacilityUnits_Clubs_ClubId",
                table: "FacilityUnits",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
