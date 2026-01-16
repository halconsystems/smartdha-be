using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.LaundrySystemDb
{
    /// <inheritdoc />
    public partial class LMSTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfirmedOrders",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrdersId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PickUpAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeliverAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShopId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShopsId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrderStatus = table.Column<int>(type: "int", nullable: false),
                    AssignedToUserId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    ConfirmedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AcknowledgedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RiderOnTheWayAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RiderArrivedToAddressAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ParcelPickedParcelFromAddressAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RiderArrivedOnShopAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WashnPressProcessAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ParcelReadyAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveredToRiderAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RiderWayFromShopToHomeAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RiderArrivedToDeliveredAddressAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AmountToCollect = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CollectedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BasketId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfirmedOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfirmedOrders_Orders_OrdersId",
                        column: x => x.OrdersId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ConfirmedOrders_Shops_ShopsId",
                        column: x => x.ShopsId,
                        principalTable: "Shops",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderPaymentIpnLogs",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ErrCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrMsg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BasketId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValidationHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TransactionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MerchantAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DiscountedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TransactionCurrency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssuerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaskedPan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobileNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsInternational = table.Column<bool>(type: "bit", nullable: true),
                    RecurringTxn = table.Column<bool>(type: "bit", nullable: true),
                    BillNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RdvMessageKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdditionalValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RawPayload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPaymentIpnLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfirmedOrders_OrdersId",
                table: "ConfirmedOrders",
                column: "OrdersId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfirmedOrders_ShopsId",
                table: "ConfirmedOrders",
                column: "ShopsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfirmedOrders");

            migrationBuilder.DropTable(
                name: "OrderPaymentIpnLogs");
        }
    }
}
