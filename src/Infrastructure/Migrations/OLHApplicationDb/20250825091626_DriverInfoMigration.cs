using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.OLHApplicationDb
{
    /// <inheritdoc />
    public partial class DriverInfoMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OLH_BowserDriverShifts_OLH_DriverInfo_DriverInfoId",
                table: "OLH_BowserDriverShifts");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_BowserRequests_OLH_DriverInfo_DriverInfoId",
                table: "OLH_BowserRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_DriverInfo_DriverStatuses_DriverStatusId",
                table: "OLH_DriverInfo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OLH_DriverInfo",
                table: "OLH_DriverInfo");

            migrationBuilder.RenameTable(
                name: "OLH_DriverInfo",
                newName: "DriverInfos");

            migrationBuilder.RenameIndex(
                name: "IX_OLH_DriverInfo_DriverStatusId",
                table: "DriverInfos",
                newName: "IX_DriverInfos_DriverStatusId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DriverInfos",
                table: "DriverInfos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DriverInfos_DriverStatuses_DriverStatusId",
                table: "DriverInfos",
                column: "DriverStatusId",
                principalTable: "DriverStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_BowserDriverShifts_DriverInfos_DriverInfoId",
                table: "OLH_BowserDriverShifts",
                column: "DriverInfoId",
                principalTable: "DriverInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_BowserRequests_DriverInfos_DriverInfoId",
                table: "OLH_BowserRequests",
                column: "DriverInfoId",
                principalTable: "DriverInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DriverInfos_DriverStatuses_DriverStatusId",
                table: "DriverInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_BowserDriverShifts_DriverInfos_DriverInfoId",
                table: "OLH_BowserDriverShifts");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_BowserRequests_DriverInfos_DriverInfoId",
                table: "OLH_BowserRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DriverInfos",
                table: "DriverInfos");

            migrationBuilder.RenameTable(
                name: "DriverInfos",
                newName: "OLH_DriverInfo");

            migrationBuilder.RenameIndex(
                name: "IX_DriverInfos_DriverStatusId",
                table: "OLH_DriverInfo",
                newName: "IX_OLH_DriverInfo_DriverStatusId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OLH_DriverInfo",
                table: "OLH_DriverInfo",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_BowserDriverShifts_OLH_DriverInfo_DriverInfoId",
                table: "OLH_BowserDriverShifts",
                column: "DriverInfoId",
                principalTable: "OLH_DriverInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_BowserRequests_OLH_DriverInfo_DriverInfoId",
                table: "OLH_BowserRequests",
                column: "DriverInfoId",
                principalTable: "OLH_DriverInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_DriverInfo_DriverStatuses_DriverStatusId",
                table: "OLH_DriverInfo",
                column: "DriverStatusId",
                principalTable: "DriverStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
