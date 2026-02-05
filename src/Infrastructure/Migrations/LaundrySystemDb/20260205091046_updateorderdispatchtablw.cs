using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.LaundrySystemDb
{
    /// <inheritdoc />
    public partial class UpdateOrderDisptachTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
          

           

            migrationBuilder.DropIndex(
                name: "IX_OrderDispatches_SvVehicleId",
                table: "OrderDispatches");

            migrationBuilder.DropColumn(
                name: "LastLocationUpdateAt",
                table: "OrderDispatches");

            migrationBuilder.DropColumn(
                name: "SvVehicleId",
                table: "OrderDispatches");

           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLocationUpdateAt",
                table: "OrderDispatches",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SvVehicleId",
                table: "OrderDispatches",
                type: "uniqueidentifier",
                nullable: true);

           

            migrationBuilder.CreateIndex(
                name: "IX_OrderDispatches_SvVehicleId",
                table: "OrderDispatches",
                column: "SvVehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_SvVehicle_DriverId",
                table: "SvVehicle",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_SvVehicle_SvPointId",
                table: "SvVehicle",
                column: "SvPointId");

            

            migrationBuilder.AddForeignKey(
                name: "FK_ShopVehicleAssignmentHistories_SvVehicle_VehicleId",
                table: "ShopVehicleAssignmentHistories",
                column: "VehicleId",
                principalTable: "SvVehicle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
