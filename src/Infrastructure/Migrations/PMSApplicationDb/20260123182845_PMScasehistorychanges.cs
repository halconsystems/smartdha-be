using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.PMSApplicationDb
{
    /// <inheritdoc />
    public partial class PMScasehistorychanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DirectorateId",
                table: "CaseStepHistories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "DirectorateName",
                table: "CaseStepHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "ModuleId",
                table: "CaseStepHistories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "StepName",
                table: "CaseStepHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StepNo",
                table: "CaseStepHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DirectorateId",
                table: "CaseStepHistories");

            migrationBuilder.DropColumn(
                name: "DirectorateName",
                table: "CaseStepHistories");

            migrationBuilder.DropColumn(
                name: "ModuleId",
                table: "CaseStepHistories");

            migrationBuilder.DropColumn(
                name: "StepName",
                table: "CaseStepHistories");

            migrationBuilder.DropColumn(
                name: "StepNo",
                table: "CaseStepHistories");
        }
    }
}
