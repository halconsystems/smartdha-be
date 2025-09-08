using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Data.SQLite
{
    /// <inheritdoc />
    public partial class Panicbuttoncdbchanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PanicRequests_EmergencyTypes_EmergencyTypeId",
                table: "PanicRequests");

            migrationBuilder.DropTable(
                name: "PanicRequestActionLogs");

            migrationBuilder.DropIndex(
                name: "IX_PanicRequests_CaseNo",
                table: "PanicRequests");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyTypes_Code",
                table: "EmergencyTypes");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RecordedAt",
                table: "PanicLocationUpdates",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmergencyTypes",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "EmergencyTypes",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValue: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PanicRequests_EmergencyTypes_EmergencyTypeId",
                table: "PanicRequests",
                column: "EmergencyTypeId",
                principalTable: "EmergencyTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PanicRequests_EmergencyTypes_EmergencyTypeId",
                table: "PanicRequests");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RecordedAt",
                table: "PanicLocationUpdates",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmergencyTypes",
                type: "bit",
                nullable: true,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "EmergencyTypes",
                type: "bit",
                nullable: true,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "PanicRequestActionLogs",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PanicRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ActionByUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FromStatus = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ToStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PanicRequestActionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PanicRequestActionLogs_PanicRequests_PanicRequestId",
                        column: x => x.PanicRequestId,
                        principalTable: "PanicRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PanicRequests_CaseNo",
                table: "PanicRequests",
                column: "CaseNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyTypes_Code",
                table: "EmergencyTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PanicRequestActionLogs_PanicRequestId",
                table: "PanicRequestActionLogs",
                column: "PanicRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_PanicRequests_EmergencyTypes_EmergencyTypeId",
                table: "PanicRequests",
                column: "EmergencyTypeId",
                principalTable: "EmergencyTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
