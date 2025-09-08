using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.OLHApplicationDb
{
    /// <inheritdoc />
    public partial class Newchangesinbowserdbs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OLH_BowserRequests_DriverInfos_DriverInfoId",
                table: "OLH_BowserRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_BowserRequests_OLH_Vehicles_VehicleId",
                table: "OLH_BowserRequests");

            migrationBuilder.DropIndex(
                name: "IX_OLH_BowserRequests_DriverInfoId",
                table: "OLH_BowserRequests");

            migrationBuilder.DropColumn(
                name: "VehicleTypeId",
                table: "OLH_VehicleTypes");

            migrationBuilder.DropColumn(
                name: "VehicleStatusID",
                table: "OLH_VehicleStatuses");

            migrationBuilder.DropColumn(
                name: "VehicleID",
                table: "OLH_Vehicles");

            migrationBuilder.DropColumn(
                name: "VehicleOwnerId",
                table: "OLH_VehicleOwners");

            migrationBuilder.DropColumn(
                name: "ShiftId",
                table: "OLH_Shifts");

            migrationBuilder.DropColumn(
                name: "CapacityID",
                table: "OLH_BowserRequests");

            migrationBuilder.DropColumn(
                name: "DriverId",
                table: "OLH_BowserRequests");

            migrationBuilder.DropColumn(
                name: "DriverInfoId",
                table: "OLH_BowserRequests");

            migrationBuilder.DropColumn(
                name: "PaymentID",
                table: "OLH_BowserRequests");

            migrationBuilder.DropColumn(
                name: "RequestStatusId",
                table: "OLH_BowserRequests");

            migrationBuilder.RenameColumn(
                name: "VehilceType",
                table: "OLH_VehicleTypes",
                newName: "TypeName");

            migrationBuilder.RenameColumn(
                name: "VehicleStatus",
                table: "OLH_VehicleStatuses",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "YearofManufacture",
                table: "OLH_Vehicles",
                newName: "YearOfManufacture");

            migrationBuilder.RenameColumn(
                name: "VehicleStatusID",
                table: "OLH_Vehicles",
                newName: "VehicleStatusId");

            migrationBuilder.RenameColumn(
                name: "ModelID",
                table: "OLH_Vehicles",
                newName: "ModelId");

            migrationBuilder.RenameColumn(
                name: "MakeID",
                table: "OLH_Vehicles",
                newName: "MakeId");

            migrationBuilder.RenameColumn(
                name: "BowserCapacityID",
                table: "OLH_Vehicles",
                newName: "BowserCapacityId");

            migrationBuilder.RenameColumn(
                name: "VehicleOwner",
                table: "OLH_VehicleOwners",
                newName: "OwnerName");

            migrationBuilder.RenameColumn(
                name: "Shift",
                table: "OLH_Shifts",
                newName: "ShiftName");

            migrationBuilder.RenameColumn(
                name: "longitude",
                table: "OLH_BowserRequests",
                newName: "Longitude");

            migrationBuilder.RenameColumn(
                name: "latitude",
                table: "OLH_BowserRequests",
                newName: "Latitude");

            migrationBuilder.RenameColumn(
                name: "VehicleId",
                table: "OLH_BowserRequests",
                newName: "PhaseId");

            migrationBuilder.RenameColumn(
                name: "Phase",
                table: "OLH_BowserRequests",
                newName: "Currency");

            migrationBuilder.RenameColumn(
                name: "PaymentStatusID",
                table: "OLH_BowserRequests",
                newName: "PaymentStatus");

            migrationBuilder.RenameIndex(
                name: "IX_OLH_BowserRequests_VehicleId",
                table: "OLH_BowserRequests",
                newName: "IX_OLH_BowserRequests_PhaseId");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "DriverStatuses",
                newName: "Status");

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "OLH_VehicleStatuses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "OLH_Vehicles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "ModelId",
                table: "OLH_Vehicles",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "MakeId",
                table: "OLH_Vehicles",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "RequestNo",
                table: "OLH_BowserRequests",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PlannedDeliveryDate",
                table: "OLH_BowserRequests",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Ext",
                table: "OLH_BowserRequests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeliveryDate",
                table: "OLH_BowserRequests",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedDriverId",
                table: "OLH_BowserRequests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedVehicleId",
                table: "OLH_BowserRequests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "OLH_BowserRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "OLH_BowserRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "Status",
                table: "OLH_BowserRequests",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "DriverInfos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Capacity",
                table: "BowserCapacitys",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "OLH_BowserAssignmentHistorys",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssignedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcceptedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ArrivedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OLH_BowserAssignmentHistorys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OLH_BowserAssignmentHistorys_DriverInfos_DriverId",
                        column: x => x.DriverId,
                        principalTable: "DriverInfos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OLH_BowserAssignmentHistorys_OLH_BowserRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "OLH_BowserRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OLH_BowserAssignmentHistorys_OLH_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "OLH_Vehicles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OLH_BowserRequestStatus",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderBy = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OLH_BowserRequestStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OLH_BowserRequestStatusHistorys",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromStatus = table.Column<short>(type: "smallint", nullable: false),
                    ToStatus = table.Column<short>(type: "smallint", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_OLH_BowserRequestStatusHistorys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OLH_BowserRequestStatusHistorys_OLH_BowserRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "OLH_BowserRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OLH_DriverShifts",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverInfoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DutyDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OLH_DriverShifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OLH_DriverShifts_DriverInfos_DriverInfoId",
                        column: x => x.DriverInfoId,
                        principalTable: "DriverInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OLH_DriverShifts_OLH_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "OLH_Shifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OLH_DriverShifts_OLH_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "OLH_Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OLH_Payments",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProviderPaymentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProviderIntentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthorizationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuthorizedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VoidedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RefundedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MetaJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OLH_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OLH_Payments_OLH_BowserRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "OLH_BowserRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OLH_Phase",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OLH_Phase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OLH_BowserRequestsNextStatus",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NextStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OLH_BowserRequestsNextStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OLH_BowserRequestsNextStatus_OLH_BowserRequestStatus_NextStatusId",
                        column: x => x.NextStatusId,
                        principalTable: "OLH_BowserRequestStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OLH_BowserRequestsNextStatus_OLH_BowserRequestStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "OLH_BowserRequestStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OLH_Refunds",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RefundedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProviderRefundId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OLH_Refunds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OLH_Refunds_OLH_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "OLH_Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OLH_PhaseCapacities",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BowserCapacityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BaseRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OLH_PhaseCapacities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OLH_PhaseCapacities_BowserCapacitys_BowserCapacityId",
                        column: x => x.BowserCapacityId,
                        principalTable: "BowserCapacitys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OLH_PhaseCapacities_OLH_Phase_PhaseId",
                        column: x => x.PhaseId,
                        principalTable: "OLH_Phase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OLH_Vehicles_BowserCapacityId",
                table: "OLH_Vehicles",
                column: "BowserCapacityId");

            migrationBuilder.CreateIndex(
                name: "IX_OLH_Vehicles_VehicleOwnerId",
                table: "OLH_Vehicles",
                column: "VehicleOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_OLH_Vehicles_VehicleStatusId",
                table: "OLH_Vehicles",
                column: "VehicleStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_OLH_Vehicles_VehicleTypeId",
                table: "OLH_Vehicles",
                column: "VehicleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OLH_BowserRequests_AssignedDriverId",
                table: "OLH_BowserRequests",
                column: "AssignedDriverId");

            migrationBuilder.CreateIndex(
                name: "IX_OLH_BowserRequests_AssignedVehicleId",
                table: "OLH_BowserRequests",
                column: "AssignedVehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_OLH_BowserRequests_RequestNo",
                table: "OLH_BowserRequests",
                column: "RequestNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OLH_BowserAssignmentHistorys_DriverId",
                table: "OLH_BowserAssignmentHistorys",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_OLH_BowserAssignmentHistorys_RequestId",
                table: "OLH_BowserAssignmentHistorys",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_OLH_BowserAssignmentHistorys_VehicleId",
                table: "OLH_BowserAssignmentHistorys",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_OLH_BowserRequestsNextStatus_NextStatusId",
                table: "OLH_BowserRequestsNextStatus",
                column: "NextStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_OLH_BowserRequestsNextStatus_StatusId_NextStatusId",
                table: "OLH_BowserRequestsNextStatus",
                columns: new[] { "StatusId", "NextStatusId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OLH_BowserRequestStatusHistorys_RequestId",
                table: "OLH_BowserRequestStatusHistorys",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_OLH_DriverShifts_DriverId_DutyDate",
                table: "OLH_DriverShifts",
                columns: new[] { "DriverId", "DutyDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OLH_DriverShifts_DriverInfoId",
                table: "OLH_DriverShifts",
                column: "DriverInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_OLH_DriverShifts_ShiftId",
                table: "OLH_DriverShifts",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_OLH_DriverShifts_VehicleId_DutyDate",
                table: "OLH_DriverShifts",
                columns: new[] { "VehicleId", "DutyDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OLH_Payments_RequestId",
                table: "OLH_Payments",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_OLH_PhaseCapacities_BowserCapacityId",
                table: "OLH_PhaseCapacities",
                column: "BowserCapacityId");

            migrationBuilder.CreateIndex(
                name: "IX_OLH_PhaseCapacities_PhaseId_BowserCapacityId_EffectiveFrom_EffectiveTo",
                table: "OLH_PhaseCapacities",
                columns: new[] { "PhaseId", "BowserCapacityId", "EffectiveFrom", "EffectiveTo" });

            migrationBuilder.CreateIndex(
                name: "IX_OLH_Refunds_PaymentId",
                table: "OLH_Refunds",
                column: "PaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_BowserRequests_DriverInfos_AssignedDriverId",
                table: "OLH_BowserRequests",
                column: "AssignedDriverId",
                principalTable: "DriverInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_BowserRequests_OLH_Phase_PhaseId",
                table: "OLH_BowserRequests",
                column: "PhaseId",
                principalTable: "OLH_Phase",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OLH_BowserRequests_DriverInfos_AssignedDriverId",
                table: "OLH_BowserRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_BowserRequests_OLH_Phase_PhaseId",
                table: "OLH_BowserRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_OLH_BowserRequests_OLH_Vehicles_AssignedVehicleId",
                table: "OLH_BowserRequests");

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

            migrationBuilder.DropTable(
                name: "OLH_BowserAssignmentHistorys");

            migrationBuilder.DropTable(
                name: "OLH_BowserRequestsNextStatus");

            migrationBuilder.DropTable(
                name: "OLH_BowserRequestStatusHistorys");

            migrationBuilder.DropTable(
                name: "OLH_DriverShifts");

            migrationBuilder.DropTable(
                name: "OLH_PhaseCapacities");

            migrationBuilder.DropTable(
                name: "OLH_Refunds");

            migrationBuilder.DropTable(
                name: "OLH_BowserRequestStatus");

            migrationBuilder.DropTable(
                name: "OLH_Phase");

            migrationBuilder.DropTable(
                name: "OLH_Payments");

            migrationBuilder.DropIndex(
                name: "IX_OLH_Vehicles_BowserCapacityId",
                table: "OLH_Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_OLH_Vehicles_VehicleOwnerId",
                table: "OLH_Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_OLH_Vehicles_VehicleStatusId",
                table: "OLH_Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_OLH_Vehicles_VehicleTypeId",
                table: "OLH_Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_OLH_BowserRequests_AssignedDriverId",
                table: "OLH_BowserRequests");

            migrationBuilder.DropIndex(
                name: "IX_OLH_BowserRequests_AssignedVehicleId",
                table: "OLH_BowserRequests");

            migrationBuilder.DropIndex(
                name: "IX_OLH_BowserRequests_RequestNo",
                table: "OLH_BowserRequests");

            migrationBuilder.DropColumn(
                name: "AssignedDriverId",
                table: "OLH_BowserRequests");

            migrationBuilder.DropColumn(
                name: "AssignedVehicleId",
                table: "OLH_BowserRequests");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "OLH_BowserRequests");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "OLH_BowserRequests");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "OLH_BowserRequests");

            migrationBuilder.RenameColumn(
                name: "TypeName",
                table: "OLH_VehicleTypes",
                newName: "VehilceType");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "OLH_VehicleStatuses",
                newName: "VehicleStatus");

            migrationBuilder.RenameColumn(
                name: "YearOfManufacture",
                table: "OLH_Vehicles",
                newName: "YearofManufacture");

            migrationBuilder.RenameColumn(
                name: "VehicleStatusId",
                table: "OLH_Vehicles",
                newName: "VehicleStatusID");

            migrationBuilder.RenameColumn(
                name: "ModelId",
                table: "OLH_Vehicles",
                newName: "ModelID");

            migrationBuilder.RenameColumn(
                name: "MakeId",
                table: "OLH_Vehicles",
                newName: "MakeID");

            migrationBuilder.RenameColumn(
                name: "BowserCapacityId",
                table: "OLH_Vehicles",
                newName: "BowserCapacityID");

            migrationBuilder.RenameColumn(
                name: "OwnerName",
                table: "OLH_VehicleOwners",
                newName: "VehicleOwner");

            migrationBuilder.RenameColumn(
                name: "ShiftName",
                table: "OLH_Shifts",
                newName: "Shift");

            migrationBuilder.RenameColumn(
                name: "Longitude",
                table: "OLH_BowserRequests",
                newName: "longitude");

            migrationBuilder.RenameColumn(
                name: "Latitude",
                table: "OLH_BowserRequests",
                newName: "latitude");

            migrationBuilder.RenameColumn(
                name: "PhaseId",
                table: "OLH_BowserRequests",
                newName: "VehicleId");

            migrationBuilder.RenameColumn(
                name: "PaymentStatus",
                table: "OLH_BowserRequests",
                newName: "PaymentStatusID");

            migrationBuilder.RenameColumn(
                name: "Currency",
                table: "OLH_BowserRequests",
                newName: "Phase");

            migrationBuilder.RenameIndex(
                name: "IX_OLH_BowserRequests_PhaseId",
                table: "OLH_BowserRequests",
                newName: "IX_OLH_BowserRequests_VehicleId");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "DriverStatuses",
                newName: "status");

            migrationBuilder.AddColumn<Guid>(
                name: "VehicleTypeId",
                table: "OLH_VehicleTypes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "OLH_VehicleStatuses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VehicleStatusID",
                table: "OLH_VehicleStatuses",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "OLH_Vehicles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ModelID",
                table: "OLH_Vehicles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "MakeID",
                table: "OLH_Vehicles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VehicleID",
                table: "OLH_Vehicles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "VehicleOwnerId",
                table: "OLH_VehicleOwners",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ShiftId",
                table: "OLH_Shifts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "RequestNo",
                table: "OLH_BowserRequests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PlannedDeliveryDate",
                table: "OLH_BowserRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Ext",
                table: "OLH_BowserRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeliveryDate",
                table: "OLH_BowserRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CapacityID",
                table: "OLH_BowserRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "DriverId",
                table: "OLH_BowserRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "DriverInfoId",
                table: "OLH_BowserRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<long>(
                name: "PaymentID",
                table: "OLH_BowserRequests",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<Guid>(
                name: "RequestStatusId",
                table: "OLH_BowserRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "DriverInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Capacity",
                table: "BowserCapacitys",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.CreateIndex(
                name: "IX_OLH_BowserRequests_DriverInfoId",
                table: "OLH_BowserRequests",
                column: "DriverInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_BowserRequests_DriverInfos_DriverInfoId",
                table: "OLH_BowserRequests",
                column: "DriverInfoId",
                principalTable: "DriverInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OLH_BowserRequests_OLH_Vehicles_VehicleId",
                table: "OLH_BowserRequests",
                column: "VehicleId",
                principalTable: "OLH_Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
