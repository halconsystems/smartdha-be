using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExtraOccupancyImpl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaxOccupancy",
                table: "Rooms",
                newName: "NormalOccupancy");

            migrationBuilder.RenameColumn(
                name: "NoOfOccupancy",
                table: "RoomCharges",
                newName: "ExtraOccupancy");

            migrationBuilder.AlterColumn<int>(
                name: "ExtraOccupancy",
                table: "RoomCharges",
                defaultValue: 0
                );

            migrationBuilder.AddColumn<int>(
                name: "MaxExtraOccupancy",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxExtraOccupancy",
                table: "Rooms");

            migrationBuilder.RenameColumn(
                name: "NormalOccupancy",
                table: "Rooms",
                newName: "MaxOccupancy");

            migrationBuilder.RenameColumn(
                name: "ExtraOccupancy",
                table: "RoomCharges",
                newName: "NoOfOccupancy");
        }
    }
}
