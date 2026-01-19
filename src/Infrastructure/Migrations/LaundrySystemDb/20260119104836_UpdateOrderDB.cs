using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.LaundrySystemDb
{
    /// <inheritdoc />
    public partial class UpdateOrderDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDispatches_ShopVehicles_DeliverShopVehiclesId",
                table: "OrderDispatches");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDispatches_ShopVehicles_PickupShopVehiclesId",
                table: "OrderDispatches");

            migrationBuilder.AlterColumn<Guid>(
                name: "PickupShopVehiclesId",
                table: "OrderDispatches",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "DeliverShopVehiclesId",
                table: "OrderDispatches",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDispatches_ShopVehicles_DeliverShopVehiclesId",
                table: "OrderDispatches",
                column: "DeliverShopVehiclesId",
                principalTable: "ShopVehicles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDispatches_ShopVehicles_PickupShopVehiclesId",
                table: "OrderDispatches",
                column: "PickupShopVehiclesId",
                principalTable: "ShopVehicles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDispatches_ShopVehicles_DeliverShopVehiclesId",
                table: "OrderDispatches");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDispatches_ShopVehicles_PickupShopVehiclesId",
                table: "OrderDispatches");

            migrationBuilder.AlterColumn<Guid>(
                name: "PickupShopVehiclesId",
                table: "OrderDispatches",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeliverShopVehiclesId",
                table: "OrderDispatches",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDispatches_ShopVehicles_DeliverShopVehiclesId",
                table: "OrderDispatches",
                column: "DeliverShopVehiclesId",
                principalTable: "ShopVehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDispatches_ShopVehicles_PickupShopVehiclesId",
                table: "OrderDispatches",
                column: "PickupShopVehiclesId",
                principalTable: "ShopVehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
