using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.OLHApplicationDb
{
    /// <inheritdoc />
    public partial class dropIndexVehIdDutyDtOnDvrShift : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DriverShifts_VehicleId_DutyDate",
                table: "DriverShifts");

            migrationBuilder.CreateIndex(
                name: "IX_DriverShifts_VehicleId",
                table: "DriverShifts",
                column: "VehicleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DriverShifts_VehicleId",
                table: "DriverShifts");

            migrationBuilder.CreateIndex(
                name: "IX_DriverShifts_VehicleId_DutyDate",
                table: "DriverShifts",
                columns: new[] { "VehicleId", "DutyDate" },
                unique: true);
        }
    }
}
