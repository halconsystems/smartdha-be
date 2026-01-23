using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.PMSApplicationDb
{
    /// <inheritdoc />
    public partial class AddPropertytablemodification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CurrentModuleId",
                table: "PropertyCases",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DirectorateId",
                table: "PropertyCases",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "PlotNo",
                table: "Properties",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Phase",
                table: "Properties",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Area",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CellNo",
                table: "Properties",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MemberNo",
                table: "Properties",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MemberPk",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropertyPk",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModuleId",
                table: "Directorates",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PropertyCases_DirectorateId",
                table: "PropertyCases",
                column: "DirectorateId");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyCases_Directorates_DirectorateId",
                table: "PropertyCases",
                column: "DirectorateId",
                principalTable: "Directorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyCases_Directorates_DirectorateId",
                table: "PropertyCases");

            migrationBuilder.DropIndex(
                name: "IX_PropertyCases_DirectorateId",
                table: "PropertyCases");

            migrationBuilder.DropColumn(
                name: "CurrentModuleId",
                table: "PropertyCases");

            migrationBuilder.DropColumn(
                name: "DirectorateId",
                table: "PropertyCases");

            migrationBuilder.DropColumn(
                name: "CellNo",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "MemberNo",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "MemberPk",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "PropertyPk",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "ModuleId",
                table: "Directorates");

            migrationBuilder.AlterColumn<string>(
                name: "PlotNo",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Phase",
                table: "Properties",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Area",
                table: "Properties",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
