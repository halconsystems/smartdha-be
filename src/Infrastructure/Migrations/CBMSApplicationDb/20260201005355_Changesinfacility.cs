using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.CBMSApplicationDb
{
    /// <inheritdoc />
    public partial class Changesinfacility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookingMode",
                table: "ClubFacilities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FacilityActionType",
                table: "ClubFacilities",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookingMode",
                table: "ClubFacilities");

            migrationBuilder.DropColumn(
                name: "FacilityActionType",
                table: "ClubFacilities");
        }
    }
}
