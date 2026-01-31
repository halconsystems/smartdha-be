using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.CBMSApplicationDb
{
    /// <inheritdoc />
    public partial class Sometablesremove : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FacilityUnitSlots");

            migrationBuilder.DropColumn(
                name: "FromDate",
                table: "BookingSchedules");

            migrationBuilder.DropColumn(
                name: "SlotId",
                table: "BookingSchedules");

            migrationBuilder.DropColumn(
                name: "ToDate",
                table: "BookingSchedules");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Date",
                table: "BookingSchedules",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "EndTime",
                table: "BookingSchedules",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "StartTime",
                table: "BookingSchedules",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "BookingSchedules");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "BookingSchedules");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Date",
                table: "BookingSchedules",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<DateOnly>(
                name: "FromDate",
                table: "BookingSchedules",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SlotId",
                table: "BookingSchedules",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ToDate",
                table: "BookingSchedules",
                type: "date",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FacilityUnitSlots",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    FacilityUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityUnitSlots", x => x.Id);
                });
        }
    }
}
