using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DHAFacilitationAPIs.Infrastructure.Data.SQLite
{
    /// <inheritdoc />
    public partial class Panicbuttonchanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EmergencyTypes",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa99"));

            migrationBuilder.DeleteData(
                table: "EmergencyTypes",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"));

            migrationBuilder.DeleteData(
                table: "EmergencyTypes",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"));

            migrationBuilder.DeleteData(
                table: "EmergencyTypes",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3"));

            migrationBuilder.DeleteData(
                table: "EmergencyTypes",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4"));

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "PanicActionLogs");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "PanicActionLogs");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "PanicActionLogs");

            migrationBuilder.DropColumn(
                name: "PerformedByUserId",
                table: "PanicActionLogs");

            migrationBuilder.RenameColumn(
                name: "ActionType",
                table: "PanicActionLogs",
                newName: "ToStatus");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RecordedAt",
                table: "PanicLocationUpdates",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "Action",
                table: "PanicActionLogs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ActionByUserId",
                table: "PanicActionLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "FromStatus",
                table: "PanicActionLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "PanicActionLogs",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Action",
                table: "PanicActionLogs");

            migrationBuilder.DropColumn(
                name: "ActionByUserId",
                table: "PanicActionLogs");

            migrationBuilder.DropColumn(
                name: "FromStatus",
                table: "PanicActionLogs");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "PanicActionLogs");

            migrationBuilder.RenameColumn(
                name: "ToStatus",
                table: "PanicActionLogs",
                newName: "ActionType");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RecordedAt",
                table: "PanicLocationUpdates",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "PanicActionLogs",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "PanicActionLogs",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "PanicActionLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PerformedByUserId",
                table: "PanicActionLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "EmergencyTypes",
                columns: new[] { "Id", "Code", "Created", "CreatedBy", "Description", "HelplineNumber", "IsActive", "IsDeleted", "LastModified", "LastModifiedBy", "Name" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa99"), 99, new DateTime(2025, 9, 6, 8, 2, 50, 190, DateTimeKind.Utc).AddTicks(177), null, "Other emergency", null, true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Other" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"), 1, new DateTime(2025, 9, 6, 8, 2, 50, 190, DateTimeKind.Utc).AddTicks(166), null, "General rescue", "1122", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Rescue" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"), 2, new DateTime(2025, 9, 6, 8, 2, 50, 190, DateTimeKind.Utc).AddTicks(170), null, "Fire emergency", "16", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Fire Brigade" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3"), 3, new DateTime(2025, 9, 6, 8, 2, 50, 190, DateTimeKind.Utc).AddTicks(172), null, "Ambulance/medical", "115", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Health Emergency" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4"), 4, new DateTime(2025, 9, 6, 8, 2, 50, 190, DateTimeKind.Utc).AddTicks(175), null, "Police helpline", "15", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Police" }
                });
        }
    }
}
