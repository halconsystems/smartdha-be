using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.CBMSApplicationDb
{
    /// <inheritdoc />
    public partial class Changesintables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacilityUnitServices_Facilities_FacilityId",
                table: "FacilityUnitServices");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "FacilityUnitServices");

            migrationBuilder.DropColumn(
                name: "IsComplimentary",
                table: "FacilityUnitServices");

            migrationBuilder.DropColumn(
                name: "IsOptional",
                table: "FacilityUnitServices");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "FacilityUnitServices");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "FacilityUnitServices");

            migrationBuilder.RenameColumn(
                name: "IsQuantityBased",
                table: "FacilityUnitServices",
                newName: "IsEnabled");

            migrationBuilder.RenameColumn(
                name: "FacilityId",
                table: "FacilityUnitServices",
                newName: "FacilityUnitId");

            migrationBuilder.RenameIndex(
                name: "IX_FacilityUnitServices_FacilityId",
                table: "FacilityUnitServices",
                newName: "IX_FacilityUnitServices_FacilityUnitId");

            migrationBuilder.AddColumn<Guid>(
                name: "FacilityServiceId",
                table: "FacilityUnitServices",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "OverridePrice",
                table: "FacilityUnitServices",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "FacilityServices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsQuantityBased",
                table: "FacilityServices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_FacilityUnitServices_FacilityServiceId",
                table: "FacilityUnitServices",
                column: "FacilityServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_FacilityUnitServices_FacilityServices_FacilityServiceId",
                table: "FacilityUnitServices",
                column: "FacilityServiceId",
                principalTable: "FacilityServices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FacilityUnitServices_FacilityUnits_FacilityUnitId",
                table: "FacilityUnitServices",
                column: "FacilityUnitId",
                principalTable: "FacilityUnits",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacilityUnitServices_FacilityServices_FacilityServiceId",
                table: "FacilityUnitServices");

            migrationBuilder.DropForeignKey(
                name: "FK_FacilityUnitServices_FacilityUnits_FacilityUnitId",
                table: "FacilityUnitServices");

            migrationBuilder.DropIndex(
                name: "IX_FacilityUnitServices_FacilityServiceId",
                table: "FacilityUnitServices");

            migrationBuilder.DropColumn(
                name: "FacilityServiceId",
                table: "FacilityUnitServices");

            migrationBuilder.DropColumn(
                name: "OverridePrice",
                table: "FacilityUnitServices");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "FacilityServices");

            migrationBuilder.DropColumn(
                name: "IsQuantityBased",
                table: "FacilityServices");

            migrationBuilder.RenameColumn(
                name: "IsEnabled",
                table: "FacilityUnitServices",
                newName: "IsQuantityBased");

            migrationBuilder.RenameColumn(
                name: "FacilityUnitId",
                table: "FacilityUnitServices",
                newName: "FacilityId");

            migrationBuilder.RenameIndex(
                name: "IX_FacilityUnitServices_FacilityUnitId",
                table: "FacilityUnitServices",
                newName: "IX_FacilityUnitServices_FacilityId");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "FacilityUnitServices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsComplimentary",
                table: "FacilityUnitServices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOptional",
                table: "FacilityUnitServices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "FacilityUnitServices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "FacilityUnitServices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddForeignKey(
                name: "FK_FacilityUnitServices_Facilities_FacilityId",
                table: "FacilityUnitServices",
                column: "FacilityId",
                principalTable: "Facilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
