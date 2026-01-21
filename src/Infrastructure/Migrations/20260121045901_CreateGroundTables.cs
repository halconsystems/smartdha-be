using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateGroundTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroundCategory",
                table: "Rooms",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Directorate",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directorate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroundImages",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageURL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageExtension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroundImages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroundPaymentIpnLogs",
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
                    table.PrimaryKey("PK_GroundPaymentIpnLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Grounds",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountNoAccronym = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    GroundType = table.Column<int>(type: "int", nullable: false),
                    GroundCategory = table.Column<int>(type: "int", nullable: false),
                    ClubId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grounds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Grounds_Clubs_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Clubs",
                        principalColumn: "Id");
                });

           


            migrationBuilder.CreateTable(
                name: "GroundSettings",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SettingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DTCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDiscount = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroundSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroundBookings",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroundsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookingDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckinDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckOutDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubTotal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Taxes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Discount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CollectedAmount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AmountToCollect = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BacketId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    IsConfirm = table.Column<bool>(type: "bit", nullable: false),
                    BookingCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroundBookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroundBookings_Grounds_GroundsId",
                        column: x => x.GroundsId,
                        principalTable: "Grounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GroundSlots",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroundsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SlotName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SlotPrice = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FromDateOnly = table.Column<DateOnly>(type: "date", nullable: false),
                    FromTimeOnly = table.Column<TimeOnly>(type: "time", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDateOnly = table.Column<DateOnly>(type: "date", nullable: false),
                    ToTimeOnly = table.Column<TimeOnly>(type: "time", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroundSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroundSlots_Grounds_GroundsId",
                        column: x => x.GroundsId,
                        principalTable: "Grounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GroundStandtardTimes",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroundsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CheckInTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    CheckOutTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroundStandtardTimes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroundStandtardTimes_Grounds_GroundsId",
                        column: x => x.GroundsId,
                        principalTable: "Grounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            

            migrationBuilder.CreateTable(
                name: "GroundBookingSlots",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroundBookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SlotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroundSlotsId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SlotPrice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroundBookingSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroundBookingSlots_GroundBookings_GroundBookingId",
                        column: x => x.GroundBookingId,
                        principalTable: "GroundBookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GroundBookingSlots_GroundSlots_GroundSlotsId",
                        column: x => x.GroundSlotsId,
                        principalTable: "GroundSlots",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroundBookings_GroundsId",
                table: "GroundBookings",
                column: "GroundsId");

            migrationBuilder.CreateIndex(
                name: "IX_GroundBookingSlots_GroundBookingId",
                table: "GroundBookingSlots",
                column: "GroundBookingId");

            migrationBuilder.CreateIndex(
                name: "IX_GroundBookingSlots_GroundSlotsId",
                table: "GroundBookingSlots",
                column: "GroundSlotsId");

            migrationBuilder.CreateIndex(
                name: "IX_Grounds_ClubId",
                table: "Grounds",
                column: "ClubId");


            migrationBuilder.CreateIndex(
                name: "IX_GroundSlots_GroundsId",
                table: "GroundSlots",
                column: "GroundsId");

            migrationBuilder.CreateIndex(
                name: "IX_GroundStandtardTimes_GroundsId",
                table: "GroundStandtardTimes",
                column: "GroundsId");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(
                name: "GroundBookingSlots");

            migrationBuilder.DropTable(
                name: "GroundImages");

            migrationBuilder.DropTable(
                name: "GroundPaymentIpnLogs");

            migrationBuilder.DropTable(
                name: "GroundSettings");

            migrationBuilder.DropTable(
                name: "GroundStandtardTimes");

            migrationBuilder.DropTable(
                name: "PrerequisiteOptions");

            migrationBuilder.DropTable(
                name: "PropertyCase");

            migrationBuilder.DropTable(
                name: "GroundBookings");

            migrationBuilder.DropTable(
                name: "GroundSlots");

            migrationBuilder.DropTable(
                name: "DiscountSettings");

            migrationBuilder.DropTable(
                name: "Grounds");

            migrationBuilder.DropColumn(
                name: "GroundCategory",
                table: "Rooms");
        }
    }
}
