using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.PMSApplicationDb
{
    /// <inheritdoc />
    public partial class Addednewtables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PerformedBy",
                table: "CaseStepHistories");

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "CaseStepHistories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FromModuleId",
                table: "CaseStepHistories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FromUserId",
                table: "CaseStepHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PerformedByUserId",
                table: "CaseStepHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ToModuleId",
                table: "CaseStepHistories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToUserId",
                table: "CaseStepHistories",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromModuleId",
                table: "CaseStepHistories");

            migrationBuilder.DropColumn(
                name: "FromUserId",
                table: "CaseStepHistories");

            migrationBuilder.DropColumn(
                name: "PerformedByUserId",
                table: "CaseStepHistories");

            migrationBuilder.DropColumn(
                name: "ToModuleId",
                table: "CaseStepHistories");

            migrationBuilder.DropColumn(
                name: "ToUserId",
                table: "CaseStepHistories");

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "CaseStepHistories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PerformedBy",
                table: "CaseStepHistories",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);
        }
    }
}
