using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.PaymentDb
{
    /// <inheritdoc />
    public partial class Changesinmybills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PayBills_UserId_Status_CreatedAt",
                table: "PayBills");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "PayBills");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "PayBills");

            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "PayBills");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "PayBills",
                newName: "LastPaymentDate");

            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "PayBills",
                newName: "OutstandingAmount");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "PayBills",
                newName: "PaymentStatus");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "PayBills",
                newName: "BillGeneratedOn");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "PayBills",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "MerchantCode",
                table: "PayBills",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<decimal>(
                name: "BillAmount",
                table: "PayBills",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "PayBills",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastAuthNo",
                table: "PayBills",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentBillId",
                table: "PayBills",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillAmount",
                table: "PayBills");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "PayBills");

            migrationBuilder.DropColumn(
                name: "LastAuthNo",
                table: "PayBills");

            migrationBuilder.DropColumn(
                name: "PaymentBillId",
                table: "PayBills");

            migrationBuilder.RenameColumn(
                name: "PaymentStatus",
                table: "PayBills",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "OutstandingAmount",
                table: "PayBills",
                newName: "TotalAmount");

            migrationBuilder.RenameColumn(
                name: "LastPaymentDate",
                table: "PayBills",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "BillGeneratedOn",
                table: "PayBills",
                newName: "CreatedAt");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "PayBills",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MerchantCode",
                table: "PayBills",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "PayBills",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "EntityId",
                table: "PayBills",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntityType",
                table: "PayBills",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_PayBills_UserId_Status_CreatedAt",
                table: "PayBills",
                columns: new[] { "UserId", "Status", "CreatedAt" });
        }
    }
}
