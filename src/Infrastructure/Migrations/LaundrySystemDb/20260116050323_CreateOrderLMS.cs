using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.LaundrySystemDb
{
    /// <inheritdoc />
    public partial class CreateOrderLMS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDTSettings_DiscountSetting_SettingId",
                table: "OrderDTSettings");

            migrationBuilder.DropTable(
                name: "DiscountSetting");

            migrationBuilder.DropIndex(
                name: "IX_OrderDTSettings_SettingId",
                table: "OrderDTSettings");

            migrationBuilder.DropColumn(
                name: "SettingId",
                table: "OrderDTSettings");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "OrderDTSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Name",
                table: "OrderDTSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "OrderDTSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ValueType",
                table: "OrderDTSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "OrderDTSettings");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "OrderDTSettings");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "OrderDTSettings");

            migrationBuilder.DropColumn(
                name: "ValueType",
                table: "OrderDTSettings");

            migrationBuilder.AddColumn<Guid>(
                name: "SettingId",
                table: "OrderDTSettings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "DiscountSetting",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsDiscount = table.Column<bool>(type: "bit", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValueType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountSetting", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDTSettings_SettingId",
                table: "OrderDTSettings",
                column: "SettingId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDTSettings_DiscountSetting_SettingId",
                table: "OrderDTSettings",
                column: "SettingId",
                principalTable: "DiscountSetting",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
