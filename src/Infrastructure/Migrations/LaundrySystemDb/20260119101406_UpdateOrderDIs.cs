using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.LaundrySystemDb
{
    /// <inheritdoc />
    public partial class UpdateOrderDIs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PickupVehiclesId",
                table: "OrderDispatches",
                newName: "PickupVehicleId");

            migrationBuilder.RenameColumn(
                name: "DeliverVehiclesId",
                table: "OrderDispatches",
                newName: "DeliverVehicleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PickupVehicleId",
                table: "OrderDispatches",
                newName: "PickupVehiclesId");

            migrationBuilder.RenameColumn(
                name: "DeliverVehicleId",
                table: "OrderDispatches",
                newName: "DeliverVehiclesId");
        }
    }
}
