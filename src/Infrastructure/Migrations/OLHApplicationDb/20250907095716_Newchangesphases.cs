using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.OLHApplicationDb
{
    /// <inheritdoc />
    public partial class Newchangesphases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OLH_BowserRequests_OLH_Phase_PhaseId",
                table: "OLH_BowserRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_PhaseCapacities_OLH_Phase_PhaseId",
                table: "OLH_PhaseCapacities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OLH_Phase",
                table: "OLH_Phase");

            migrationBuilder.RenameTable(
                name: "OLH_Phase",
                newName: "OLH_Phases");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OLH_Phases",
                table: "OLH_Phases",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_BowserRequests_OLH_Phases_PhaseId",
                table: "OLH_BowserRequests",
                column: "PhaseId",
                principalTable: "OLH_Phases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_PhaseCapacities_OLH_Phases_PhaseId",
                table: "OLH_PhaseCapacities",
                column: "PhaseId",
                principalTable: "OLH_Phases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OLH_BowserRequests_OLH_Phases_PhaseId",
                table: "OLH_BowserRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_PhaseCapacities_OLH_Phases_PhaseId",
                table: "OLH_PhaseCapacities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OLH_Phases",
                table: "OLH_Phases");

            migrationBuilder.RenameTable(
                name: "OLH_Phases",
                newName: "OLH_Phase");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OLH_Phase",
                table: "OLH_Phase",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_BowserRequests_OLH_Phase_PhaseId",
                table: "OLH_BowserRequests",
                column: "PhaseId",
                principalTable: "OLH_Phase",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_PhaseCapacities_OLH_Phase_PhaseId",
                table: "OLH_PhaseCapacities",
                column: "PhaseId",
                principalTable: "OLH_Phase",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
