using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.PaymentDb
{
    /// <inheritdoc />
    public partial class Changesinpayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BasketId",
                table: "PayTransactions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "PayMerchants",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "SmartPayMerchantId",
                table: "PayMerchants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "SourceSystem",
                table: "PayMerchantRules",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "EntityType",
                table: "PayMerchantRules",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryCode",
                table: "PayMerchantRules",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "PayBills",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SourceSystem",
                table: "PayBills",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_PayTransactions_BasketId",
                table: "PayTransactions",
                column: "BasketId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PayMerchants_Code",
                table: "PayMerchants",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PayMerchantRules_SourceSystem_EntityType_EntityId_CategoryCode_IsActive_Priority",
                table: "PayMerchantRules",
                columns: new[] { "SourceSystem", "EntityType", "EntityId", "CategoryCode", "IsActive", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_PayBills_SourceSystem_SourceVoucherId",
                table: "PayBills",
                columns: new[] { "SourceSystem", "SourceVoucherId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PayBills_UserId_Status_CreatedAt",
                table: "PayBills",
                columns: new[] { "UserId", "Status", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PayTransactions_BasketId",
                table: "PayTransactions");

            migrationBuilder.DropIndex(
                name: "IX_PayMerchants_Code",
                table: "PayMerchants");

            migrationBuilder.DropIndex(
                name: "IX_PayMerchantRules_SourceSystem_EntityType_EntityId_CategoryCode_IsActive_Priority",
                table: "PayMerchantRules");

            migrationBuilder.DropIndex(
                name: "IX_PayBills_SourceSystem_SourceVoucherId",
                table: "PayBills");

            migrationBuilder.DropIndex(
                name: "IX_PayBills_UserId_Status_CreatedAt",
                table: "PayBills");

            migrationBuilder.DropColumn(
                name: "SmartPayMerchantId",
                table: "PayMerchants");

            migrationBuilder.AlterColumn<string>(
                name: "BasketId",
                table: "PayTransactions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "PayMerchants",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "SourceSystem",
                table: "PayMerchantRules",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "EntityType",
                table: "PayMerchantRules",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryCode",
                table: "PayMerchantRules",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "PayBills",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "SourceSystem",
                table: "PayBills",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
