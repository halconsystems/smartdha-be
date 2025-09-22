using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.OLHApplicationDb
{
    /// <inheritdoc />
    public partial class Newchangesphasesremoveolh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OLH_BowserAssignmentHistorys_DriverInfos_DriverId",
                table: "OLH_BowserAssignmentHistorys");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_BowserAssignmentHistorys_OLH_BowserRequests_RequestId",
                table: "OLH_BowserAssignmentHistorys");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_BowserAssignmentHistorys_OLH_Vehicles_VehicleId",
                table: "OLH_BowserAssignmentHistorys");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_BowserDriverShifts_DriverInfos_DriverInfoId",
                table: "OLH_BowserDriverShifts");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_BowserDriverShifts_OLH_Shifts_ShiftId",
                table: "OLH_BowserDriverShifts");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_BowserDriverShifts_OLH_Vehicles_VehicleId",
                table: "OLH_BowserDriverShifts");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_BowserRequests_BowserCapacitys_BowserCapacityId",
                table: "OLH_BowserRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_BowserRequests_DriverInfos_AssignedDriverId",
                table: "OLH_BowserRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_BowserRequests_OLH_Phases_PhaseId",
                table: "OLH_BowserRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_BowserRequests_OLH_Vehicles_AssignedVehicleId",
                table: "OLH_BowserRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_BowserRequestsNextStatus_OLH_BowserRequestStatus_NextStatusId",
                table: "OLH_BowserRequestsNextStatus");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_BowserRequestsNextStatus_OLH_BowserRequestStatus_StatusId",
                table: "OLH_BowserRequestsNextStatus");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_BowserRequestStatusHistorys_OLH_BowserRequests_RequestId",
                table: "OLH_BowserRequestStatusHistorys");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_DriverShifts_DriverInfos_DriverInfoId",
                table: "OLH_DriverShifts");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_DriverShifts_OLH_Shifts_ShiftId",
                table: "OLH_DriverShifts");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_DriverShifts_OLH_Vehicles_VehicleId",
                table: "OLH_DriverShifts");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_Payments_OLH_BowserRequests_RequestId",
                table: "OLH_Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_PhaseCapacities_BowserCapacitys_BowserCapacityId",
                table: "OLH_PhaseCapacities");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_PhaseCapacities_OLH_Phases_PhaseId",
                table: "OLH_PhaseCapacities");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_Refunds_OLH_Payments_PaymentId",
                table: "OLH_Refunds");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_Vehicles_BowserCapacitys_BowserCapacityId",
                table: "OLH_Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_Vehicles_OLH_VehicleOwners_VehicleOwnerId",
                table: "OLH_Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_Vehicles_OLH_VehicleStatuses_VehicleStatusId",
                table: "OLH_Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_Vehicles_OLH_VehicleTypes_VehicleTypeId",
                table: "OLH_Vehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OLH_VehicleTypes",
                table: "OLH_VehicleTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OLH_VehicleStatuses",
                table: "OLH_VehicleStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OLH_Vehicles",
                table: "OLH_Vehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OLH_VehicleOwners",
                table: "OLH_VehicleOwners");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OLH_Shifts",
                table: "OLH_Shifts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OLH_Refunds",
                table: "OLH_Refunds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OLH_Phases",
                table: "OLH_Phases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OLH_PhaseCapacities",
                table: "OLH_PhaseCapacities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OLH_Payments",
                table: "OLH_Payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OLH_DriverShifts",
                table: "OLH_DriverShifts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OLH_BowserRequestStatusHistorys",
                table: "OLH_BowserRequestStatusHistorys");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OLH_BowserRequestsNextStatus",
                table: "OLH_BowserRequestsNextStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OLH_BowserRequests",
                table: "OLH_BowserRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OLH_BowserDriverShifts",
                table: "OLH_BowserDriverShifts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OLH_BowserAssignmentHistorys",
                table: "OLH_BowserAssignmentHistorys");

            migrationBuilder.RenameTable(
                name: "OLH_VehicleTypes",
                newName: "VehicleTypes");

            migrationBuilder.RenameTable(
                name: "OLH_VehicleStatuses",
                newName: "VehicleStatuses");

            migrationBuilder.RenameTable(
                name: "OLH_Vehicles",
                newName: "Vehicles");

            migrationBuilder.RenameTable(
                name: "OLH_VehicleOwners",
                newName: "VehicleOwners");

            migrationBuilder.RenameTable(
                name: "OLH_Shifts",
                newName: "Shifts");

            migrationBuilder.RenameTable(
                name: "OLH_Refunds",
                newName: "Refunds");

            migrationBuilder.RenameTable(
                name: "OLH_Phases",
                newName: "Phases");

            migrationBuilder.RenameTable(
                name: "OLH_PhaseCapacities",
                newName: "PhaseCapacities");

            migrationBuilder.RenameTable(
                name: "OLH_Payments",
                newName: "Payments");

            migrationBuilder.RenameTable(
                name: "OLH_DriverShifts",
                newName: "DriverShifts");

            migrationBuilder.RenameTable(
                name: "OLH_BowserRequestStatusHistorys",
                newName: "BowserRequestStatusHistorys");

            migrationBuilder.RenameTable(
                name: "OLH_BowserRequestsNextStatus",
                newName: "BowserRequestsNextStatus");

            migrationBuilder.RenameTable(
                name: "OLH_BowserRequests",
                newName: "BowserRequests");

            migrationBuilder.RenameTable(
                name: "OLH_BowserDriverShifts",
                newName: "BowserDriverShifts");

            migrationBuilder.RenameTable(
                name: "OLH_BowserAssignmentHistorys",
                newName: "BowserAssignmentHistorys");

            migrationBuilder.RenameIndex(
                name: "IX_OLH_Vehicles_VehicleTypeId",
                table: "Vehicles",
                newName: "IX_Vehicles_VehicleTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_OLH_Vehicles_VehicleStatusId",
                table: "Vehicles",
                newName: "IX_Vehicles_VehicleStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_OLH_Vehicles_VehicleOwnerId",
                table: "Vehicles",
                newName: "IX_Vehicles_VehicleOwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_OLH_Vehicles_BowserCapacityId",
                table: "Vehicles",
                newName: "IX_Vehicles_BowserCapacityId");

            migrationBuilder.RenameIndex(
                name: "IX_OLH_Refunds_PaymentId",
                table: "Refunds",
                newName: "IX_Refunds_PaymentId");

            migrationBuilder.RenameIndex(
                name: "IX_OLH_PhaseCapacities_PhaseId_BowserCapacityId_EffectiveFrom_EffectiveTo",
                table: "PhaseCapacities",
                newName: "IX_PhaseCapacities_PhaseId_BowserCapacityId_EffectiveFrom_EffectiveTo");

            migrationBuilder.RenameIndex(
                name: "IX_OLH_PhaseCapacities_BowserCapacityId",
                table: "PhaseCapacities",
                newName: "IX_PhaseCapacities_BowserCapacityId");

            migrationBuilder.RenameIndex(
                name: "IX_OLH_Payments_RequestId",
                table: "Payments",
                newName: "IX_Payments_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_OLH_DriverShifts_VehicleId_DutyDate",
                table: "DriverShifts",
                newName: "IX_DriverShifts_VehicleId_DutyDate");

            migrationBuilder.RenameIndex(
                name: "IX_OLH_DriverShifts_ShiftId",
                table: "DriverShifts",
                newName: "IX_DriverShifts_ShiftId");

            migrationBuilder.RenameIndex(
                name: "IX_OLH_DriverShifts_DriverInfoId",
                table: "DriverShifts",
                newName: "IX_DriverShifts_DriverInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_OLH_DriverShifts_DriverId_DutyDate",
                table: "DriverShifts",
                newName: "IX_DriverShifts_DriverId_DutyDate");

            migrationBuilder.RenameIndex(
                name: "IX_OLH_BowserRequestStatusHistorys_RequestId",
                table: "BowserRequestStatusHistorys",
                newName: "IX_BowserRequestStatusHistorys_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_OLH_BowserRequestsNextStatus_StatusId_NextStatusId",
                table: "BowserRequestsNextStatus",
                newName: "IX_BowserRequestsNextStatus_StatusId_NextStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_OLH_BowserRequestsNextStatus_NextStatusId",
                table: "BowserRequestsNextStatus",
                newName: "IX_BowserRequestsNextStatus_NextStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_OLH_BowserRequests_RequestNo",
                table: "BowserRequests",
                newName: "IX_BowserRequests_RequestNo");

            migrationBuilder.RenameIndex(
                name: "IX_OLH_BowserRequests_PhaseId",
                table: "BowserRequests",
                newName: "IX_BowserRequests_PhaseId");

            migrationBuilder.RenameIndex(
                name: "IX_OLH_BowserRequests_BowserCapacityId",
                table: "BowserRequests",
                newName: "IX_BowserRequests_BowserCapacityId");

            migrationBuilder.RenameIndex(
                name: "IX_OLH_BowserRequests_AssignedVehicleId",
                table: "BowserRequests",
                newName: "IX_BowserRequests_AssignedVehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_OLH_BowserRequests_AssignedDriverId",
                table: "BowserRequests",
                newName: "IX_BowserRequests_AssignedDriverId");

            migrationBuilder.RenameIndex(
                name: "IX_OLH_BowserDriverShifts_VehicleId",
                table: "BowserDriverShifts",
                newName: "IX_BowserDriverShifts_VehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_OLH_BowserDriverShifts_ShiftId",
                table: "BowserDriverShifts",
                newName: "IX_BowserDriverShifts_ShiftId");

            migrationBuilder.RenameIndex(
                name: "IX_OLH_BowserDriverShifts_DriverInfoId",
                table: "BowserDriverShifts",
                newName: "IX_BowserDriverShifts_DriverInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_OLH_BowserAssignmentHistorys_VehicleId",
                table: "BowserAssignmentHistorys",
                newName: "IX_BowserAssignmentHistorys_VehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_OLH_BowserAssignmentHistorys_RequestId",
                table: "BowserAssignmentHistorys",
                newName: "IX_BowserAssignmentHistorys_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_OLH_BowserAssignmentHistorys_DriverId",
                table: "BowserAssignmentHistorys",
                newName: "IX_BowserAssignmentHistorys_DriverId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleTypes",
                table: "VehicleTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleStatuses",
                table: "VehicleStatuses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vehicles",
                table: "Vehicles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleOwners",
                table: "VehicleOwners",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shifts",
                table: "Shifts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Refunds",
                table: "Refunds",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Phases",
                table: "Phases",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhaseCapacities",
                table: "PhaseCapacities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DriverShifts",
                table: "DriverShifts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BowserRequestStatusHistorys",
                table: "BowserRequestStatusHistorys",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BowserRequestsNextStatus",
                table: "BowserRequestsNextStatus",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BowserRequests",
                table: "BowserRequests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BowserDriverShifts",
                table: "BowserDriverShifts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BowserAssignmentHistorys",
                table: "BowserAssignmentHistorys",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BowserAssignmentHistorys_BowserRequests_RequestId",
                table: "BowserAssignmentHistorys",
                column: "RequestId",
                principalTable: "BowserRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BowserAssignmentHistorys_DriverInfos_DriverId",
                table: "BowserAssignmentHistorys",
                column: "DriverId",
                principalTable: "DriverInfos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BowserAssignmentHistorys_Vehicles_VehicleId",
                table: "BowserAssignmentHistorys",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id");

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
                table: "BowserRequests",
                column: "BowserCapacityId",
                principalTable: "BowserCapacitys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BowserRequests_DriverInfos_AssignedDriverId",
                table: "BowserRequests",
                column: "AssignedDriverId",
                principalTable: "DriverInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BowserRequests_Phases_PhaseId",
                table: "BowserRequests",
                column: "PhaseId",
                principalTable: "Phases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BowserRequests_Vehicles_AssignedVehicleId",
                table: "BowserRequests",
                column: "AssignedVehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BowserRequestsNextStatus_OLH_BowserRequestStatus_NextStatusId",
                table: "BowserRequestsNextStatus",
                column: "NextStatusId",
                principalTable: "OLH_BowserRequestStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BowserRequestsNextStatus_OLH_BowserRequestStatus_StatusId",
                table: "BowserRequestsNextStatus",
                column: "StatusId",
                principalTable: "OLH_BowserRequestStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BowserRequestStatusHistorys_BowserRequests_RequestId",
                table: "BowserRequestStatusHistorys",
                column: "RequestId",
                principalTable: "BowserRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DriverShifts_DriverInfos_DriverInfoId",
                table: "DriverShifts",
                column: "DriverInfoId",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BowserAssignmentHistorys_BowserRequests_RequestId",
                table: "BowserAssignmentHistorys");

            migrationBuilder.DropForeignKey(
                name: "FK_BowserAssignmentHistorys_DriverInfos_DriverId",
                table: "BowserAssignmentHistorys");

            migrationBuilder.DropForeignKey(
                name: "FK_BowserAssignmentHistorys_Vehicles_VehicleId",
                table: "BowserAssignmentHistorys");

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
                table: "BowserRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_BowserRequests_DriverInfos_AssignedDriverId",
                table: "BowserRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_BowserRequests_Phases_PhaseId",
                table: "BowserRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_BowserRequests_Vehicles_AssignedVehicleId",
                table: "BowserRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_BowserRequestsNextStatus_OLH_BowserRequestStatus_NextStatusId",
                table: "BowserRequestsNextStatus");

            migrationBuilder.DropForeignKey(
                name: "FK_BowserRequestsNextStatus_OLH_BowserRequestStatus_StatusId",
                table: "BowserRequestsNextStatus");

            migrationBuilder.DropForeignKey(
                name: "FK_BowserRequestStatusHistorys_BowserRequests_RequestId",
                table: "BowserRequestStatusHistorys");

            migrationBuilder.DropForeignKey(
                name: "FK_DriverShifts_DriverInfos_DriverInfoId",
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

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleTypes",
                table: "VehicleTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleStatuses",
                table: "VehicleStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vehicles",
                table: "Vehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleOwners",
                table: "VehicleOwners");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shifts",
                table: "Shifts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Refunds",
                table: "Refunds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Phases",
                table: "Phases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhaseCapacities",
                table: "PhaseCapacities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DriverShifts",
                table: "DriverShifts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BowserRequestStatusHistorys",
                table: "BowserRequestStatusHistorys");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BowserRequestsNextStatus",
                table: "BowserRequestsNextStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BowserRequests",
                table: "BowserRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BowserDriverShifts",
                table: "BowserDriverShifts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BowserAssignmentHistorys",
                table: "BowserAssignmentHistorys");

            migrationBuilder.RenameTable(
                name: "VehicleTypes",
                newName: "OLH_VehicleTypes");

            migrationBuilder.RenameTable(
                name: "VehicleStatuses",
                newName: "OLH_VehicleStatuses");

            migrationBuilder.RenameTable(
                name: "Vehicles",
                newName: "OLH_Vehicles");

            migrationBuilder.RenameTable(
                name: "VehicleOwners",
                newName: "OLH_VehicleOwners");

            migrationBuilder.RenameTable(
                name: "Shifts",
                newName: "OLH_Shifts");

            migrationBuilder.RenameTable(
                name: "Refunds",
                newName: "OLH_Refunds");

            migrationBuilder.RenameTable(
                name: "Phases",
                newName: "OLH_Phases");

            migrationBuilder.RenameTable(
                name: "PhaseCapacities",
                newName: "OLH_PhaseCapacities");

            migrationBuilder.RenameTable(
                name: "Payments",
                newName: "OLH_Payments");

            migrationBuilder.RenameTable(
                name: "DriverShifts",
                newName: "OLH_DriverShifts");

            migrationBuilder.RenameTable(
                name: "BowserRequestStatusHistorys",
                newName: "OLH_BowserRequestStatusHistorys");

            migrationBuilder.RenameTable(
                name: "BowserRequestsNextStatus",
                newName: "OLH_BowserRequestsNextStatus");

            migrationBuilder.RenameTable(
                name: "BowserRequests",
                newName: "OLH_BowserRequests");

            migrationBuilder.RenameTable(
                name: "BowserDriverShifts",
                newName: "OLH_BowserDriverShifts");

            migrationBuilder.RenameTable(
                name: "BowserAssignmentHistorys",
                newName: "OLH_BowserAssignmentHistorys");

            migrationBuilder.RenameIndex(
                name: "IX_Vehicles_VehicleTypeId",
                table: "OLH_Vehicles",
                newName: "IX_OLH_Vehicles_VehicleTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Vehicles_VehicleStatusId",
                table: "OLH_Vehicles",
                newName: "IX_OLH_Vehicles_VehicleStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Vehicles_VehicleOwnerId",
                table: "OLH_Vehicles",
                newName: "IX_OLH_Vehicles_VehicleOwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Vehicles_BowserCapacityId",
                table: "OLH_Vehicles",
                newName: "IX_OLH_Vehicles_BowserCapacityId");

            migrationBuilder.RenameIndex(
                name: "IX_Refunds_PaymentId",
                table: "OLH_Refunds",
                newName: "IX_OLH_Refunds_PaymentId");

            migrationBuilder.RenameIndex(
                name: "IX_PhaseCapacities_PhaseId_BowserCapacityId_EffectiveFrom_EffectiveTo",
                table: "OLH_PhaseCapacities",
                newName: "IX_OLH_PhaseCapacities_PhaseId_BowserCapacityId_EffectiveFrom_EffectiveTo");

            migrationBuilder.RenameIndex(
                name: "IX_PhaseCapacities_BowserCapacityId",
                table: "OLH_PhaseCapacities",
                newName: "IX_OLH_PhaseCapacities_BowserCapacityId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_RequestId",
                table: "OLH_Payments",
                newName: "IX_OLH_Payments_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_DriverShifts_VehicleId_DutyDate",
                table: "OLH_DriverShifts",
                newName: "IX_OLH_DriverShifts_VehicleId_DutyDate");

            migrationBuilder.RenameIndex(
                name: "IX_DriverShifts_ShiftId",
                table: "OLH_DriverShifts",
                newName: "IX_OLH_DriverShifts_ShiftId");

            migrationBuilder.RenameIndex(
                name: "IX_DriverShifts_DriverInfoId",
                table: "OLH_DriverShifts",
                newName: "IX_OLH_DriverShifts_DriverInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_DriverShifts_DriverId_DutyDate",
                table: "OLH_DriverShifts",
                newName: "IX_OLH_DriverShifts_DriverId_DutyDate");

            migrationBuilder.RenameIndex(
                name: "IX_BowserRequestStatusHistorys_RequestId",
                table: "OLH_BowserRequestStatusHistorys",
                newName: "IX_OLH_BowserRequestStatusHistorys_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_BowserRequestsNextStatus_StatusId_NextStatusId",
                table: "OLH_BowserRequestsNextStatus",
                newName: "IX_OLH_BowserRequestsNextStatus_StatusId_NextStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_BowserRequestsNextStatus_NextStatusId",
                table: "OLH_BowserRequestsNextStatus",
                newName: "IX_OLH_BowserRequestsNextStatus_NextStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_BowserRequests_RequestNo",
                table: "OLH_BowserRequests",
                newName: "IX_OLH_BowserRequests_RequestNo");

            migrationBuilder.RenameIndex(
                name: "IX_BowserRequests_PhaseId",
                table: "OLH_BowserRequests",
                newName: "IX_OLH_BowserRequests_PhaseId");

            migrationBuilder.RenameIndex(
                name: "IX_BowserRequests_BowserCapacityId",
                table: "OLH_BowserRequests",
                newName: "IX_OLH_BowserRequests_BowserCapacityId");

            migrationBuilder.RenameIndex(
                name: "IX_BowserRequests_AssignedVehicleId",
                table: "OLH_BowserRequests",
                newName: "IX_OLH_BowserRequests_AssignedVehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_BowserRequests_AssignedDriverId",
                table: "OLH_BowserRequests",
                newName: "IX_OLH_BowserRequests_AssignedDriverId");

            migrationBuilder.RenameIndex(
                name: "IX_BowserDriverShifts_VehicleId",
                table: "OLH_BowserDriverShifts",
                newName: "IX_OLH_BowserDriverShifts_VehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_BowserDriverShifts_ShiftId",
                table: "OLH_BowserDriverShifts",
                newName: "IX_OLH_BowserDriverShifts_ShiftId");

            migrationBuilder.RenameIndex(
                name: "IX_BowserDriverShifts_DriverInfoId",
                table: "OLH_BowserDriverShifts",
                newName: "IX_OLH_BowserDriverShifts_DriverInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_BowserAssignmentHistorys_VehicleId",
                table: "OLH_BowserAssignmentHistorys",
                newName: "IX_OLH_BowserAssignmentHistorys_VehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_BowserAssignmentHistorys_RequestId",
                table: "OLH_BowserAssignmentHistorys",
                newName: "IX_OLH_BowserAssignmentHistorys_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_BowserAssignmentHistorys_DriverId",
                table: "OLH_BowserAssignmentHistorys",
                newName: "IX_OLH_BowserAssignmentHistorys_DriverId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OLH_VehicleTypes",
                table: "OLH_VehicleTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OLH_VehicleStatuses",
                table: "OLH_VehicleStatuses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OLH_Vehicles",
                table: "OLH_Vehicles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OLH_VehicleOwners",
                table: "OLH_VehicleOwners",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OLH_Shifts",
                table: "OLH_Shifts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OLH_Refunds",
                table: "OLH_Refunds",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OLH_Phases",
                table: "OLH_Phases",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OLH_PhaseCapacities",
                table: "OLH_PhaseCapacities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OLH_Payments",
                table: "OLH_Payments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OLH_DriverShifts",
                table: "OLH_DriverShifts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OLH_BowserRequestStatusHistorys",
                table: "OLH_BowserRequestStatusHistorys",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OLH_BowserRequestsNextStatus",
                table: "OLH_BowserRequestsNextStatus",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OLH_BowserRequests",
                table: "OLH_BowserRequests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OLH_BowserDriverShifts",
                table: "OLH_BowserDriverShifts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OLH_BowserAssignmentHistorys",
                table: "OLH_BowserAssignmentHistorys",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_BowserAssignmentHistorys_DriverInfos_DriverId",
                table: "OLH_BowserAssignmentHistorys",
                column: "DriverId",
                principalTable: "DriverInfos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_BowserAssignmentHistorys_OLH_BowserRequests_RequestId",
                table: "OLH_BowserAssignmentHistorys",
                column: "RequestId",
                principalTable: "OLH_BowserRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_BowserAssignmentHistorys_OLH_Vehicles_VehicleId",
                table: "OLH_BowserAssignmentHistorys",
                column: "VehicleId",
                principalTable: "OLH_Vehicles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_BowserDriverShifts_DriverInfos_DriverInfoId",
                table: "OLH_BowserDriverShifts",
                column: "DriverInfoId",
                principalTable: "DriverInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_BowserDriverShifts_OLH_Shifts_ShiftId",
                table: "OLH_BowserDriverShifts",
                column: "ShiftId",
                principalTable: "OLH_Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_BowserDriverShifts_OLH_Vehicles_VehicleId",
                table: "OLH_BowserDriverShifts",
                column: "VehicleId",
                principalTable: "OLH_Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_BowserRequests_BowserCapacitys_BowserCapacityId",
                table: "OLH_BowserRequests",
                column: "BowserCapacityId",
                principalTable: "BowserCapacitys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_BowserRequests_DriverInfos_AssignedDriverId",
                table: "OLH_BowserRequests",
                column: "AssignedDriverId",
                principalTable: "DriverInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_BowserRequests_OLH_Phases_PhaseId",
                table: "OLH_BowserRequests",
                column: "PhaseId",
                principalTable: "OLH_Phases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_BowserRequests_OLH_Vehicles_AssignedVehicleId",
                table: "OLH_BowserRequests",
                column: "AssignedVehicleId",
                principalTable: "OLH_Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_BowserRequestsNextStatus_OLH_BowserRequestStatus_NextStatusId",
                table: "OLH_BowserRequestsNextStatus",
                column: "NextStatusId",
                principalTable: "OLH_BowserRequestStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_BowserRequestsNextStatus_OLH_BowserRequestStatus_StatusId",
                table: "OLH_BowserRequestsNextStatus",
                column: "StatusId",
                principalTable: "OLH_BowserRequestStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_BowserRequestStatusHistorys_OLH_BowserRequests_RequestId",
                table: "OLH_BowserRequestStatusHistorys",
                column: "RequestId",
                principalTable: "OLH_BowserRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_DriverShifts_DriverInfos_DriverInfoId",
                table: "OLH_DriverShifts",
                column: "DriverInfoId",
                principalTable: "DriverInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_DriverShifts_OLH_Shifts_ShiftId",
                table: "OLH_DriverShifts",
                column: "ShiftId",
                principalTable: "OLH_Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_DriverShifts_OLH_Vehicles_VehicleId",
                table: "OLH_DriverShifts",
                column: "VehicleId",
                principalTable: "OLH_Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_Payments_OLH_BowserRequests_RequestId",
                table: "OLH_Payments",
                column: "RequestId",
                principalTable: "OLH_BowserRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_PhaseCapacities_BowserCapacitys_BowserCapacityId",
                table: "OLH_PhaseCapacities",
                column: "BowserCapacityId",
                principalTable: "BowserCapacitys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_PhaseCapacities_OLH_Phases_PhaseId",
                table: "OLH_PhaseCapacities",
                column: "PhaseId",
                principalTable: "OLH_Phases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_Refunds_OLH_Payments_PaymentId",
                table: "OLH_Refunds",
                column: "PaymentId",
                principalTable: "OLH_Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_Vehicles_BowserCapacitys_BowserCapacityId",
                table: "OLH_Vehicles",
                column: "BowserCapacityId",
                principalTable: "BowserCapacitys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_Vehicles_OLH_VehicleOwners_VehicleOwnerId",
                table: "OLH_Vehicles",
                column: "VehicleOwnerId",
                principalTable: "OLH_VehicleOwners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_Vehicles_OLH_VehicleStatuses_VehicleStatusId",
                table: "OLH_Vehicles",
                column: "VehicleStatusId",
                principalTable: "OLH_VehicleStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_Vehicles_OLH_VehicleTypes_VehicleTypeId",
                table: "OLH_Vehicles",
                column: "VehicleTypeId",
                principalTable: "OLH_VehicleTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
