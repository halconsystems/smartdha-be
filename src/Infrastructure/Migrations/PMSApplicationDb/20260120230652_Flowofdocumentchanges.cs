using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.PMSApplicationDb
{
    /// <inheritdoc />
    public partial class Flowofdocumentchanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromModuleId",
                table: "CaseStepHistories");

            migrationBuilder.DropColumn(
                name: "NextStepId",
                table: "CaseStepHistories");

            migrationBuilder.DropColumn(
                name: "NextStepNo",
                table: "CaseStepHistories");

            migrationBuilder.DropColumn(
                name: "ToModuleId",
                table: "CaseStepHistories");

            migrationBuilder.AddColumn<string>(
                name: "CurrentAssignedUserId",
                table: "PropertyCases",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentAssignedUserId",
                table: "PropertyCases");

            migrationBuilder.AddColumn<Guid>(
                name: "FromModuleId",
                table: "CaseStepHistories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NextStepId",
                table: "CaseStepHistories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NextStepNo",
                table: "CaseStepHistories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ToModuleId",
                table: "CaseStepHistories",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
