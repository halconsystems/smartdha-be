using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.OLHApplicationDb
{
    /// <inheritdoc />
    public partial class DriverInfochangetodriverid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DriverShifts_DriverInfos_DriverInfoId",
                table: "DriverShifts");

            migrationBuilder.DropIndex(
                name: "IX_DriverShifts_DriverInfoId",
                table: "DriverShifts");

            migrationBuilder.DropColumn(
                name: "DriverInfoId",
                table: "DriverShifts");

            migrationBuilder.AddForeignKey(
                name: "FK_DriverShifts_DriverInfos_DriverId",
                table: "DriverShifts",
                column: "DriverId",
                principalTable: "DriverInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DriverShifts_DriverInfos_DriverId",
                table: "DriverShifts");

            migrationBuilder.AddColumn<Guid>(
                name: "DriverInfoId",
                table: "DriverShifts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_DriverShifts_DriverInfoId",
                table: "DriverShifts",
                column: "DriverInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_DriverShifts_DriverInfos_DriverInfoId",
                table: "DriverShifts",
                column: "DriverInfoId",
                principalTable: "DriverInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
