using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.CBMSApplicationDb
{
    /// <inheritdoc />
    public partial class Changesincbms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Facilities_ClubCategoryId",
                table: "Facilities");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "FacilityUnits",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_ClubCategoryId_Code",
                table: "Facilities",
                columns: new[] { "ClubCategoryId", "Code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Facilities_ClubCategoryId_Code",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "FacilityUnits");

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_ClubCategoryId",
                table: "Facilities",
                column: "ClubCategoryId");
        }
    }
}
