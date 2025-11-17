using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.OLHApplicationDb
{
    /// <inheritdoc />
    public partial class dropIndexDvrdDutyDtOnDvrShift : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DriverShifts_DriverId_DutyDate",
                table: "DriverShifts");

            migrationBuilder.CreateIndex(
                name: "IX_DriverShifts_DriverId",
                table: "DriverShifts",
                column: "DriverId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DriverShifts_DriverId",
                table: "DriverShifts");

            migrationBuilder.CreateIndex(
                name: "IX_DriverShifts_DriverId_DutyDate",
                table: "DriverShifts",
                columns: new[] { "DriverId", "DutyDate" },
                unique: true);
        }
    }
}
