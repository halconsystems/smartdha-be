using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.CBMSApplicationDb
{
    /// <inheritdoc />
    public partial class Facilityunitchangesdone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          
            migrationBuilder.AddForeignKey(
                name: "FK_FacilityUnits_Clubs_ClubId",
                table: "FacilityUnits",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacilityUnits_Clubs_ClubId",
                table: "FacilityUnits");

           
        }
    }
}
