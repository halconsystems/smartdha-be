using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.PaymentDb
{
    /// <inheritdoc />
    public partial class Changesinpaymentlatefee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountAfterDueDate",
                table: "PayBills",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LateFee",
                table: "PayBills",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountAfterDueDate",
                table: "PayBills");

            migrationBuilder.DropColumn(
                name: "LateFee",
                table: "PayBills");
        }
    }
}
