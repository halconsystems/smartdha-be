using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.PMSApplicationDb
{
    /// <inheritdoc />
    public partial class Casefeereceiptchanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OneBillId",
                table: "CaseFeeReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VoucherNo",
                table: "CaseFeeReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CaseFeeReceipts_CaseId",
                table: "CaseFeeReceipts",
                column: "CaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CaseFeeReceipts_PropertyCases_CaseId",
                table: "CaseFeeReceipts",
                column: "CaseId",
                principalTable: "PropertyCases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaseFeeReceipts_PropertyCases_CaseId",
                table: "CaseFeeReceipts");

            migrationBuilder.DropIndex(
                name: "IX_CaseFeeReceipts_CaseId",
                table: "CaseFeeReceipts");

            migrationBuilder.DropColumn(
                name: "OneBillId",
                table: "CaseFeeReceipts");

            migrationBuilder.DropColumn(
                name: "VoucherNo",
                table: "CaseFeeReceipts");
        }
    }
}
