using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.LaundrySystemDb
{
    /// <inheritdoc />
    public partial class CheckDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AddColumn<Guid>(
                name: "SvVehicleId",
                table: "OrderDispatches",
                type: "uniqueidentifier",
                nullable: true);

           


            migrationBuilder.CreateTable(
                name: "ShopVehicleAssignmentHistories",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DriverId = table.Column<string>(type: "nvarchar(450)", nullable: true),
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
                    table.PrimaryKey("PK_ShopVehicleAssignmentHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopVehicleAssignmentHistories_ShopVehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "ShopVehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });


            migrationBuilder.CreateIndex(
                name: "IX_OrderDispatches_SvVehicleId",
                table: "OrderDispatches",
                column: "SvVehicleId");

           

           

            migrationBuilder.CreateIndex(
                name: "IX_ShopVehicleAssignmentHistories_VehicleId",
                table: "ShopVehicleAssignmentHistories",
                column: "VehicleId");

          
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.DropTable(
                name: "ShopVehicleAssignmentHistories");

          

            migrationBuilder.DropIndex(
                name: "IX_OrderDispatches_SvVehicleId",
                table: "OrderDispatches");

           
        }
    }
}
