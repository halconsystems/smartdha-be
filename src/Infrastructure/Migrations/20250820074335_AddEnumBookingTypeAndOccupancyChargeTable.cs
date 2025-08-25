using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEnumBookingTypeAndOccupancyChargeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "BookingType",
                table: "RoomCharges",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateOnly>(
                name: "CheckInDateOnly",
                table: "RoomBookings",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "CheckInTimeOnly",
                table: "RoomBookings",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<DateOnly>(
                name: "CheckOutDateOnly",
                table: "RoomBookings",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "CheckOutTimeOnly",
                table: "RoomBookings",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<DateOnly>(
                name: "FromDateOnly",
                table: "ReservationRooms",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "FromTimeOnly",
                table: "ReservationRooms",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<DateOnly>(
                name: "ToDateOnly",
                table: "ReservationRooms",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ToTimeOnly",
                table: "ReservationRooms",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "AccountNo",
                table: "Clubs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountNoAccronym",
                table: "Clubs",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RoomExtraOccupancyCharges",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomChargeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaxOccupancy = table.Column<int>(type: "int", nullable: false),
                    Charges = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomExtraOccupancyCharges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomExtraOccupancyCharges_RoomCharges_RoomChargeID",
                        column: x => x.RoomChargeID,
                        principalTable: "RoomCharges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomCharges_RoomId",
                table: "RoomCharges",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomExtraOccupancyCharges_RoomChargeID",
                table: "RoomExtraOccupancyCharges",
                column: "RoomChargeID");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomCharges_Rooms_RoomId",
                table: "RoomCharges",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomCharges_Rooms_RoomId",
                table: "RoomCharges");

            migrationBuilder.DropTable(
                name: "RoomExtraOccupancyCharges");

            migrationBuilder.DropIndex(
                name: "IX_RoomCharges_RoomId",
                table: "RoomCharges");

            migrationBuilder.DropColumn(
                name: "CheckInDateOnly",
                table: "RoomBookings");

            migrationBuilder.DropColumn(
                name: "CheckInTimeOnly",
                table: "RoomBookings");

            migrationBuilder.DropColumn(
                name: "CheckOutDateOnly",
                table: "RoomBookings");

            migrationBuilder.DropColumn(
                name: "CheckOutTimeOnly",
                table: "RoomBookings");

            migrationBuilder.DropColumn(
                name: "FromDateOnly",
                table: "ReservationRooms");

            migrationBuilder.DropColumn(
                name: "FromTimeOnly",
                table: "ReservationRooms");

            migrationBuilder.DropColumn(
                name: "ToDateOnly",
                table: "ReservationRooms");

            migrationBuilder.DropColumn(
                name: "ToTimeOnly",
                table: "ReservationRooms");

            migrationBuilder.DropColumn(
                name: "AccountNo",
                table: "Clubs");

            migrationBuilder.DropColumn(
                name: "AccountNoAccronym",
                table: "Clubs");

            migrationBuilder.AlterColumn<string>(
                name: "BookingType",
                table: "RoomCharges",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
