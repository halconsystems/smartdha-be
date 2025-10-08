using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GlobalRestrictionOnDeleteOLMRS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClubBookingStandardTimes_Clubs_ClubId",
                table: "ClubBookingStandardTimes");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentIntents_Reservations_ReservationId",
                table: "PaymentIntents");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_PaymentIntents_PaymentIntentId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_RefundPolicies_Clubs_ClubId",
                table: "RefundPolicies");

            migrationBuilder.DropForeignKey(
                name: "FK_RefundRequests_Reservations_ReservationId",
                table: "RefundRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationRooms_Reservations_ReservationId",
                table: "ReservationRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationRooms_Rooms_RoomId",
                table: "ReservationRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Clubs_ClubId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomAvailabilities_Rooms_RoomId",
                table: "RoomAvailabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomBookings_Clubs_ClubId",
                table: "RoomBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomBookings_Reservations_ReservationId",
                table: "RoomBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomBookings_Rooms_RoomId",
                table: "RoomBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomCharges_Rooms_RoomId",
                table: "RoomCharges");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceMappings_Rooms_RoomId",
                table: "ServiceMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceMappings_Services_ServiceId",
                table: "ServiceMappings");

            migrationBuilder.AddForeignKey(
                name: "FK_ClubBookingStandardTimes_Clubs_ClubId",
                table: "ClubBookingStandardTimes",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentIntents_Reservations_ReservationId",
                table: "PaymentIntents",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_PaymentIntents_PaymentIntentId",
                table: "Payments",
                column: "PaymentIntentId",
                principalTable: "PaymentIntents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RefundPolicies_Clubs_ClubId",
                table: "RefundPolicies",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RefundRequests_Reservations_ReservationId",
                table: "RefundRequests",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationRooms_Reservations_ReservationId",
                table: "ReservationRooms",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationRooms_Rooms_RoomId",
                table: "ReservationRooms",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Clubs_ClubId",
                table: "Reservations",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomAvailabilities_Rooms_RoomId",
                table: "RoomAvailabilities",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomBookings_Clubs_ClubId",
                table: "RoomBookings",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomBookings_Reservations_ReservationId",
                table: "RoomBookings",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomBookings_Rooms_RoomId",
                table: "RoomBookings",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomCharges_Rooms_RoomId",
                table: "RoomCharges",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceMappings_Rooms_RoomId",
                table: "ServiceMappings",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceMappings_Services_ServiceId",
                table: "ServiceMappings",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClubBookingStandardTimes_Clubs_ClubId",
                table: "ClubBookingStandardTimes");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentIntents_Reservations_ReservationId",
                table: "PaymentIntents");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_PaymentIntents_PaymentIntentId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_RefundPolicies_Clubs_ClubId",
                table: "RefundPolicies");

            migrationBuilder.DropForeignKey(
                name: "FK_RefundRequests_Reservations_ReservationId",
                table: "RefundRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationRooms_Reservations_ReservationId",
                table: "ReservationRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationRooms_Rooms_RoomId",
                table: "ReservationRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Clubs_ClubId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomAvailabilities_Rooms_RoomId",
                table: "RoomAvailabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomBookings_Clubs_ClubId",
                table: "RoomBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomBookings_Reservations_ReservationId",
                table: "RoomBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomBookings_Rooms_RoomId",
                table: "RoomBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomCharges_Rooms_RoomId",
                table: "RoomCharges");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceMappings_Rooms_RoomId",
                table: "ServiceMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceMappings_Services_ServiceId",
                table: "ServiceMappings");

            migrationBuilder.AddForeignKey(
                name: "FK_ClubBookingStandardTimes_Clubs_ClubId",
                table: "ClubBookingStandardTimes",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentIntents_Reservations_ReservationId",
                table: "PaymentIntents",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_PaymentIntents_PaymentIntentId",
                table: "Payments",
                column: "PaymentIntentId",
                principalTable: "PaymentIntents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RefundPolicies_Clubs_ClubId",
                table: "RefundPolicies",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RefundRequests_Reservations_ReservationId",
                table: "RefundRequests",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationRooms_Reservations_ReservationId",
                table: "ReservationRooms",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationRooms_Rooms_RoomId",
                table: "ReservationRooms",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Clubs_ClubId",
                table: "Reservations",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomAvailabilities_Rooms_RoomId",
                table: "RoomAvailabilities",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomBookings_Clubs_ClubId",
                table: "RoomBookings",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomBookings_Reservations_ReservationId",
                table: "RoomBookings",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomBookings_Rooms_RoomId",
                table: "RoomBookings",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomCharges_Rooms_RoomId",
                table: "RoomCharges",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceMappings_Rooms_RoomId",
                table: "ServiceMappings",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceMappings_Services_ServiceId",
                table: "ServiceMappings",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
