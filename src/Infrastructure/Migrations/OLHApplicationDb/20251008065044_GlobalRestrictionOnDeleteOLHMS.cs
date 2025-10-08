using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.OLHApplicationDb
{
    /// <inheritdoc />
    public partial class GlobalRestrictionOnDeleteOLHMS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BowserAssignmentHistorys_BowserRequests_RequestId",
                table: "BowserAssignmentHistorys");

            migrationBuilder.DropForeignKey(
                name: "FK_BowserCapacityRates_BowserCapacitys_BowserCapacityId",
                table: "BowserCapacityRates");

            migrationBuilder.DropForeignKey(
                name: "FK_BowserDriverShifts_DriverInfos_DriverInfoId",
                table: "BowserDriverShifts");

            migrationBuilder.DropForeignKey(
                name: "FK_BowserDriverShifts_Shifts_ShiftId",
                table: "BowserDriverShifts");

            migrationBuilder.DropForeignKey(
                name: "FK_BowserDriverShifts_Vehicles_VehicleId",
                table: "BowserDriverShifts");

            migrationBuilder.DropForeignKey(
                name: "FK_BowserRequests_BowserCapacitys_BowserCapacityId",
                schema: "dbo",
                table: "BowserRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_BowserRequests_Phases_PhaseId",
                schema: "dbo",
                table: "BowserRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_BowserRequestStatusHistorys_BowserRequests_RequestId",
                table: "BowserRequestStatusHistorys");

            migrationBuilder.DropForeignKey(
                name: "FK_DriverInfos_DriverStatuses_DriverStatusId",
                table: "DriverInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_DriverShifts_DriverInfos_DriverId",
                table: "DriverShifts");

            migrationBuilder.DropForeignKey(
                name: "FK_DriverShifts_Shifts_ShiftId",
                table: "DriverShifts");

            migrationBuilder.DropForeignKey(
                name: "FK_DriverShifts_Vehicles_VehicleId",
                table: "DriverShifts");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_BowserRequests_RequestId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_PhaseCapacities_BowserCapacitys_BowserCapacityId",
                table: "PhaseCapacities");

            migrationBuilder.DropForeignKey(
                name: "FK_PhaseCapacities_Phases_PhaseId",
                table: "PhaseCapacities");

            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_Payments_PaymentId",
                table: "Refunds");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleModels_VehicleMakes_MakeId",
                table: "VehicleModels");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_BowserCapacitys_BowserCapacityId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleOwners_VehicleOwnerId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleStatuses_VehicleStatusId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleTypes_VehicleTypeId",
                table: "Vehicles");

            migrationBuilder.AddForeignKey(
                name: "FK_BowserAssignmentHistorys_BowserRequests_RequestId",
                table: "BowserAssignmentHistorys",
                column: "RequestId",
                principalSchema: "dbo",
                principalTable: "BowserRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BowserCapacityRates_BowserCapacitys_BowserCapacityId",
                table: "BowserCapacityRates",
                column: "BowserCapacityId",
                principalTable: "BowserCapacitys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BowserDriverShifts_DriverInfos_DriverInfoId",
                table: "BowserDriverShifts",
                column: "DriverInfoId",
                principalTable: "DriverInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BowserDriverShifts_Shifts_ShiftId",
                table: "BowserDriverShifts",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BowserDriverShifts_Vehicles_VehicleId",
                table: "BowserDriverShifts",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BowserRequests_BowserCapacitys_BowserCapacityId",
                schema: "dbo",
                table: "BowserRequests",
                column: "BowserCapacityId",
                principalTable: "BowserCapacitys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BowserRequests_Phases_PhaseId",
                schema: "dbo",
                table: "BowserRequests",
                column: "PhaseId",
                principalTable: "Phases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BowserRequestStatusHistorys_BowserRequests_RequestId",
                table: "BowserRequestStatusHistorys",
                column: "RequestId",
                principalSchema: "dbo",
                principalTable: "BowserRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DriverInfos_DriverStatuses_DriverStatusId",
                table: "DriverInfos",
                column: "DriverStatusId",
                principalTable: "DriverStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DriverShifts_DriverInfos_DriverId",
                table: "DriverShifts",
                column: "DriverId",
                principalTable: "DriverInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DriverShifts_Shifts_ShiftId",
                table: "DriverShifts",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DriverShifts_Vehicles_VehicleId",
                table: "DriverShifts",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_BowserRequests_RequestId",
                table: "Payments",
                column: "RequestId",
                principalSchema: "dbo",
                principalTable: "BowserRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhaseCapacities_BowserCapacitys_BowserCapacityId",
                table: "PhaseCapacities",
                column: "BowserCapacityId",
                principalTable: "BowserCapacitys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhaseCapacities_Phases_PhaseId",
                table: "PhaseCapacities",
                column: "PhaseId",
                principalTable: "Phases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Payments_PaymentId",
                table: "Refunds",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleModels_VehicleMakes_MakeId",
                table: "VehicleModels",
                column: "MakeId",
                principalTable: "VehicleMakes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_BowserCapacitys_BowserCapacityId",
                table: "Vehicles",
                column: "BowserCapacityId",
                principalTable: "BowserCapacitys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleOwners_VehicleOwnerId",
                table: "Vehicles",
                column: "VehicleOwnerId",
                principalTable: "VehicleOwners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleStatuses_VehicleStatusId",
                table: "Vehicles",
                column: "VehicleStatusId",
                principalTable: "VehicleStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleTypes_VehicleTypeId",
                table: "Vehicles",
                column: "VehicleTypeId",
                principalTable: "VehicleTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BowserAssignmentHistorys_BowserRequests_RequestId",
                table: "BowserAssignmentHistorys");

            migrationBuilder.DropForeignKey(
                name: "FK_BowserCapacityRates_BowserCapacitys_BowserCapacityId",
                table: "BowserCapacityRates");

            migrationBuilder.DropForeignKey(
                name: "FK_BowserDriverShifts_DriverInfos_DriverInfoId",
                table: "BowserDriverShifts");

            migrationBuilder.DropForeignKey(
                name: "FK_BowserDriverShifts_Shifts_ShiftId",
                table: "BowserDriverShifts");

            migrationBuilder.DropForeignKey(
                name: "FK_BowserDriverShifts_Vehicles_VehicleId",
                table: "BowserDriverShifts");

            migrationBuilder.DropForeignKey(
                name: "FK_BowserRequests_BowserCapacitys_BowserCapacityId",
                schema: "dbo",
                table: "BowserRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_BowserRequests_Phases_PhaseId",
                schema: "dbo",
                table: "BowserRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_BowserRequestStatusHistorys_BowserRequests_RequestId",
                table: "BowserRequestStatusHistorys");

            migrationBuilder.DropForeignKey(
                name: "FK_DriverInfos_DriverStatuses_DriverStatusId",
                table: "DriverInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_DriverShifts_DriverInfos_DriverId",
                table: "DriverShifts");

            migrationBuilder.DropForeignKey(
                name: "FK_DriverShifts_Shifts_ShiftId",
                table: "DriverShifts");

            migrationBuilder.DropForeignKey(
                name: "FK_DriverShifts_Vehicles_VehicleId",
                table: "DriverShifts");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_BowserRequests_RequestId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_PhaseCapacities_BowserCapacitys_BowserCapacityId",
                table: "PhaseCapacities");

            migrationBuilder.DropForeignKey(
                name: "FK_PhaseCapacities_Phases_PhaseId",
                table: "PhaseCapacities");

            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_Payments_PaymentId",
                table: "Refunds");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleModels_VehicleMakes_MakeId",
                table: "VehicleModels");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_BowserCapacitys_BowserCapacityId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleOwners_VehicleOwnerId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleStatuses_VehicleStatusId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleTypes_VehicleTypeId",
                table: "Vehicles");

            migrationBuilder.AddForeignKey(
                name: "FK_BowserAssignmentHistorys_BowserRequests_RequestId",
                table: "BowserAssignmentHistorys",
                column: "RequestId",
                principalSchema: "dbo",
                principalTable: "BowserRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BowserCapacityRates_BowserCapacitys_BowserCapacityId",
                table: "BowserCapacityRates",
                column: "BowserCapacityId",
                principalTable: "BowserCapacitys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BowserDriverShifts_DriverInfos_DriverInfoId",
                table: "BowserDriverShifts",
                column: "DriverInfoId",
                principalTable: "DriverInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BowserDriverShifts_Shifts_ShiftId",
                table: "BowserDriverShifts",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BowserDriverShifts_Vehicles_VehicleId",
                table: "BowserDriverShifts",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BowserRequests_BowserCapacitys_BowserCapacityId",
                schema: "dbo",
                table: "BowserRequests",
                column: "BowserCapacityId",
                principalTable: "BowserCapacitys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BowserRequests_Phases_PhaseId",
                schema: "dbo",
                table: "BowserRequests",
                column: "PhaseId",
                principalTable: "Phases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BowserRequestStatusHistorys_BowserRequests_RequestId",
                table: "BowserRequestStatusHistorys",
                column: "RequestId",
                principalSchema: "dbo",
                principalTable: "BowserRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DriverInfos_DriverStatuses_DriverStatusId",
                table: "DriverInfos",
                column: "DriverStatusId",
                principalTable: "DriverStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DriverShifts_DriverInfos_DriverId",
                table: "DriverShifts",
                column: "DriverId",
                principalTable: "DriverInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DriverShifts_Shifts_ShiftId",
                table: "DriverShifts",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DriverShifts_Vehicles_VehicleId",
                table: "DriverShifts",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_BowserRequests_RequestId",
                table: "Payments",
                column: "RequestId",
                principalSchema: "dbo",
                principalTable: "BowserRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PhaseCapacities_BowserCapacitys_BowserCapacityId",
                table: "PhaseCapacities",
                column: "BowserCapacityId",
                principalTable: "BowserCapacitys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PhaseCapacities_Phases_PhaseId",
                table: "PhaseCapacities",
                column: "PhaseId",
                principalTable: "Phases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Payments_PaymentId",
                table: "Refunds",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleModels_VehicleMakes_MakeId",
                table: "VehicleModels",
                column: "MakeId",
                principalTable: "VehicleMakes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_BowserCapacitys_BowserCapacityId",
                table: "Vehicles",
                column: "BowserCapacityId",
                principalTable: "BowserCapacitys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleOwners_VehicleOwnerId",
                table: "Vehicles",
                column: "VehicleOwnerId",
                principalTable: "VehicleOwners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleStatuses_VehicleStatusId",
                table: "Vehicles",
                column: "VehicleStatusId",
                principalTable: "VehicleStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleTypes_VehicleTypeId",
                table: "Vehicles",
                column: "VehicleTypeId",
                principalTable: "VehicleTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
