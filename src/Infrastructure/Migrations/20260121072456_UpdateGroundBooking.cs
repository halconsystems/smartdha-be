using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGroundBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroundBookings_Grounds_GroundsId",
                table: "GroundBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_GroundBookingSlots_GroundBookings_GroundBookingId",
                table: "GroundBookingSlots");

            migrationBuilder.AlterColumn<Guid>(
                name: "GroundBookingId",
                table: "GroundBookingSlots",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "GroundsId",
                table: "GroundBookings",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_GroundBookings_Grounds_GroundsId",
                table: "GroundBookings",
                column: "GroundsId",
                principalTable: "Grounds",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GroundBookingSlots_GroundBookings_GroundBookingId",
                table: "GroundBookingSlots",
                column: "GroundBookingId",
                principalTable: "GroundBookings",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroundBookings_Grounds_GroundsId",
                table: "GroundBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_GroundBookingSlots_GroundBookings_GroundBookingId",
                table: "GroundBookingSlots");

            migrationBuilder.AlterColumn<Guid>(
                name: "GroundBookingId",
                table: "GroundBookingSlots",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "GroundsId",
                table: "GroundBookings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GroundBookings_Grounds_GroundsId",
                table: "GroundBookings",
                column: "GroundsId",
                principalTable: "Grounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GroundBookingSlots_GroundBookings_GroundBookingId",
                table: "GroundBookingSlots",
                column: "GroundBookingId",
                principalTable: "GroundBookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
