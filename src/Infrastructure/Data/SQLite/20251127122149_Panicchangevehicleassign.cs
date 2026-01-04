using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Data.SQLite
{
    /// <inheritdoc />
    public partial class Panicchangevehicleassign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DriverId",
                table: "SvVehicles",
                type: "nvarchar(85)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SvVehicleAssignmentHistories",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DriverId = table.Column<string>(type: "nvarchar(85)", nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UnassignedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SvVehicleAssignmentHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SvVehicleAssignmentHistories_SvVehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "SvVehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SvVehicleAssignmentHistories_Users_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SvVehicles_DriverId",
                table: "SvVehicles",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_SvVehicleAssignmentHistories_DriverId",
                table: "SvVehicleAssignmentHistories",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_SvVehicleAssignmentHistories_VehicleId",
                table: "SvVehicleAssignmentHistories",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_SvVehicles_Users_DriverId",
                table: "SvVehicles",
                column: "DriverId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SvVehicles_Users_DriverId",
                table: "SvVehicles");

            migrationBuilder.DropTable(
                name: "SvVehicleAssignmentHistories");

            migrationBuilder.DropIndex(
                name: "IX_SvVehicles_DriverId",
                table: "SvVehicles");

            migrationBuilder.DropColumn(
                name: "DriverId",
                table: "SvVehicles");
        }
    }
}
