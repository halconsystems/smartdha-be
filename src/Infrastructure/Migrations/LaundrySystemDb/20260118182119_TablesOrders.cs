using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.LaundrySystemDb
{
    /// <inheritdoc />
    public partial class TablesOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropIndex(
                name: "IX_ShopVehicles_DriverId",
                table: "ShopVehicles");

            migrationBuilder.DropIndex(
                name: "IX_ShopDrivers_DriverUserId",
                table: "ShopDrivers");

            migrationBuilder.DropIndex(
                name: "IX_OrderDispatches_DeliverByUserId",
                table: "OrderDispatches");

            migrationBuilder.DropIndex(
                name: "IX_OrderDispatches_PickUpByUserId",
                table: "OrderDispatches");

            migrationBuilder.DropColumn(
                name: "DriverId",
                table: "ShopVehicles");

            migrationBuilder.DropColumn(
                name: "DriverUserId",
                table: "ShopDrivers");

            migrationBuilder.DropColumn(
                name: "DeliverByUserId",
                table: "OrderDispatches");

            migrationBuilder.DropColumn(
                name: "PickUpByUserId",
                table: "OrderDispatches");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DriverId",
                table: "ShopVehicles",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DriverUserId",
                table: "ShopDrivers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliverByUserId",
                table: "OrderDispatches",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PickUpByUserId",
                table: "OrderDispatches",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShopVehicles_DriverId",
                table: "ShopVehicles",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopDrivers_DriverUserId",
                table: "ShopDrivers",
                column: "DriverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDispatches_DeliverByUserId",
                table: "OrderDispatches",
                column: "DeliverByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDispatches_PickUpByUserId",
                table: "OrderDispatches",
                column: "PickUpByUserId");
        }
    }
}
