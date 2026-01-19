using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.LaundrySystemDb
{
    /// <inheritdoc />
    public partial class TablesLMS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConfirmedOrders_Shops_ShopsId",
                table: "ConfirmedOrders");

            migrationBuilder.DropColumn(
                name: "AcknowledgedAt",
                table: "ConfirmedOrders");

            migrationBuilder.DropColumn(
                name: "AmountToCollect",
                table: "ConfirmedOrders");

            migrationBuilder.DropColumn(
                name: "AssignedToUserId",
                table: "ConfirmedOrders");

            migrationBuilder.DropColumn(
                name: "BasketId",
                table: "ConfirmedOrders");

            migrationBuilder.DropColumn(
                name: "CollectedAmount",
                table: "ConfirmedOrders");

            migrationBuilder.DropColumn(
                name: "ConfirmedAt",
                table: "ConfirmedOrders");

            migrationBuilder.DropColumn(
                name: "DeliverAddress",
                table: "ConfirmedOrders");

            migrationBuilder.DropColumn(
                name: "DeliveredToRiderAt",
                table: "ConfirmedOrders");

            migrationBuilder.DropColumn(
                name: "OrderStatus",
                table: "ConfirmedOrders");

            migrationBuilder.DropColumn(
                name: "ParcelPickedParcelFromAddressAt",
                table: "ConfirmedOrders");

            migrationBuilder.DropColumn(
                name: "ParcelReadyAt",
                table: "ConfirmedOrders");

            migrationBuilder.DropColumn(
                name: "PickUpAddress",
                table: "ConfirmedOrders");

            migrationBuilder.DropColumn(
                name: "RiderArrivedOnShopAt",
                table: "ConfirmedOrders");

            migrationBuilder.DropColumn(
                name: "RiderArrivedToAddressAt",
                table: "ConfirmedOrders");

            migrationBuilder.DropColumn(
                name: "RiderArrivedToDeliveredAddressAt",
                table: "ConfirmedOrders");

            migrationBuilder.DropColumn(
                name: "RiderOnTheWayAt",
                table: "ConfirmedOrders");

            migrationBuilder.DropColumn(
                name: "RiderWayFromShopToHomeAt",
                table: "ConfirmedOrders");

            migrationBuilder.DropColumn(
                name: "ShopId",
                table: "ConfirmedOrders");

            migrationBuilder.DropColumn(
                name: "WashnPressProcessAt",
                table: "ConfirmedOrders");

            migrationBuilder.RenameColumn(
                name: "ShopsId",
                table: "ConfirmedOrders",
                newName: "ShopVehiclesId");

            migrationBuilder.RenameIndex(
                name: "IX_ConfirmedOrders_ShopsId",
                table: "ConfirmedOrders",
                newName: "IX_ConfirmedOrders_ShopVehiclesId");

            migrationBuilder.AddColumn<DateTime>(
                name: "AcceptDeliveredAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AcceptPickupAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountToCollect",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "BasketId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CollectedAmount",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliverAddress",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveredToRiderAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DropLatitude",
                table: "Orders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DropLongitude",
                table: "Orders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "OrderStatus",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ParcelDeliveredParcelAddressAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ParcelPickedParcelFromAddressAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ParcelPickedParcelFromShopAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ParcelReadyAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PickUpAddress",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "PickupLatitude",
                table: "Orders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PickupLongitude",
                table: "Orders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "RiderArrivedOnShopAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RiderArrivedToPickDeliveryFromShopAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RiderArrivedToPickupAddressAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ShopId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ShopsId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "WashnPressProcessAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);


            migrationBuilder.CreateTable(
                name: "ShopDrivers",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShopId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShopsId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopDrivers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopDrivers_Shops_ShopsId",
                        column: x => x.ShopsId,
                        principalTable: "Shops",
                        principalColumn: "Id");
                });



            migrationBuilder.CreateTable(
                name: "ShopVehicles",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShopId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShopsId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VehicleType = table.Column<int>(type: "int", nullable: false),
                    MapIconKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DriverUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DriverId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastLatitude = table.Column<double>(type: "float", nullable: true),
                    LastLongitude = table.Column<double>(type: "float", nullable: true),
                    LastLocationAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopVehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopVehicles_Shops_ShopsId",
                        column: x => x.ShopsId,
                        principalTable: "Shops",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderDispatches",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrdersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeliverVehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeliverShopVehiclesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PickupVehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PickupShopVehiclesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PickUpAssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveredAssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PickupAssignedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PickUpByUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PickupDriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeliverAssignedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeliverByUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeliverDriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsPickup = table.Column<bool>(type: "bit", nullable: false),
                    IsDelivered = table.Column<bool>(type: "bit", nullable: false),
                    OrderRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcceptedAtLatitude = table.Column<double>(type: "float", nullable: true),
                    AcceptedAtLongitude = table.Column<double>(type: "float", nullable: true),
                    AcceptedAtAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    DistanceFromOrderKm = table.Column<double>(type: "float", nullable: true),
                    LastLocationUpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDispatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDispatches_Orders_OrdersId",
                        column: x => x.OrdersId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
    name: "FK_OrderDispatches_ShopVehicles_DeliverShopVehiclesId",
    column: x => x.DeliverShopVehiclesId,
    principalTable: "ShopVehicles",
    principalColumn: "Id",
    onDelete: ReferentialAction.NoAction);

                    table.ForeignKey(
                        name: "FK_OrderDispatches_ShopVehicles_PickupShopVehiclesId",
                        column: x => x.PickupShopVehiclesId,
                        principalTable: "ShopVehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);

                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShopsId",
                table: "Orders",
                column: "ShopsId");



            migrationBuilder.CreateIndex(
                name: "IX_OrderDispatches_DeliverByUserId",
                table: "OrderDispatches",
                column: "DeliverByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDispatches_DeliverShopVehiclesId",
                table: "OrderDispatches",
                column: "DeliverShopVehiclesId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDispatches_OrdersId",
                table: "OrderDispatches",
                column: "OrdersId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDispatches_PickUpByUserId",
                table: "OrderDispatches",
                column: "PickUpByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDispatches_PickupShopVehiclesId",
                table: "OrderDispatches",
                column: "PickupShopVehiclesId");


            migrationBuilder.CreateIndex(
                name: "IX_ShopDrivers_DriverUserId",
                table: "ShopDrivers",
                column: "DriverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopDrivers_ShopsId",
                table: "ShopDrivers",
                column: "ShopsId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopVehicles_DriverId",
                table: "ShopVehicles",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopVehicles_ShopsId",
                table: "ShopVehicles",
                column: "ShopsId");


            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Shops_ShopsId",
                table: "Orders",
                column: "ShopsId",
                principalTable: "Shops",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConfirmedOrders_ShopVehicles_ShopVehiclesId",
                table: "ConfirmedOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Shops_ShopsId",
                table: "Orders");



            migrationBuilder.DropTable(
                name: "OrderDispatches");

            migrationBuilder.DropTable(
                name: "ShopDrivers");



            migrationBuilder.DropTable(
                name: "ShopVehicles");



            migrationBuilder.DropIndex(
                name: "IX_Orders_ShopsId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "AcceptDeliveredAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "AcceptPickupAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "AmountToCollect",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BasketId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CollectedAmount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliverAddress",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveredToRiderAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DropLatitude",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DropLongitude",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderStatus",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ParcelDeliveredParcelAddressAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ParcelPickedParcelFromAddressAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ParcelPickedParcelFromShopAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ParcelReadyAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PickUpAddress",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PickupLatitude",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PickupLongitude",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RiderArrivedOnShopAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RiderArrivedToPickDeliveryFromShopAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RiderArrivedToPickupAddressAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShopId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShopsId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "WashnPressProcessAt",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "ShopVehiclesId",
                table: "ConfirmedOrders",
                newName: "ShopsId");

            migrationBuilder.RenameIndex(
                name: "IX_ConfirmedOrders_ShopVehiclesId",
                table: "ConfirmedOrders",
                newName: "IX_ConfirmedOrders_ShopsId");

            migrationBuilder.AddColumn<DateTime>(
                name: "AcknowledgedAt",
                table: "ConfirmedOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountToCollect",
                table: "ConfirmedOrders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "AssignedToUserId",
                table: "ConfirmedOrders",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BasketId",
                table: "ConfirmedOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CollectedAmount",
                table: "ConfirmedOrders",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ConfirmedAt",
                table: "ConfirmedOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliverAddress",
                table: "ConfirmedOrders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveredToRiderAt",
                table: "ConfirmedOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderStatus",
                table: "ConfirmedOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ParcelPickedParcelFromAddressAt",
                table: "ConfirmedOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ParcelReadyAt",
                table: "ConfirmedOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PickUpAddress",
                table: "ConfirmedOrders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RiderArrivedOnShopAt",
                table: "ConfirmedOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RiderArrivedToAddressAt",
                table: "ConfirmedOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RiderArrivedToDeliveredAddressAt",
                table: "ConfirmedOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RiderOnTheWayAt",
                table: "ConfirmedOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RiderWayFromShopToHomeAt",
                table: "ConfirmedOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ShopId",
                table: "ConfirmedOrders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "WashnPressProcessAt",
                table: "ConfirmedOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ConfirmedOrders_Shops_ShopsId",
                table: "ConfirmedOrders",
                column: "ShopsId",
                principalTable: "Shops",
                principalColumn: "Id");
        }
    }
}
