using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.PMSApplicationDb
{
    /// <inheritdoc />
    public partial class Changesindocumenttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CaseRejectRequirementId",
                table: "CasePrerequisiteValues",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CaseRejectRequirementId",
                table: "CaseDocuments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CasePrerequisiteValues_CaseRejectRequirementId",
                table: "CasePrerequisiteValues",
                column: "CaseRejectRequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseDocuments_CaseRejectRequirementId",
                table: "CaseDocuments",
                column: "CaseRejectRequirementId");

            migrationBuilder.AddForeignKey(
                name: "FK_CaseDocuments_CaseRejectRequirements_CaseRejectRequirementId",
                table: "CaseDocuments",
                column: "CaseRejectRequirementId",
                principalTable: "CaseRejectRequirements",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CasePrerequisiteValues_CaseRejectRequirements_CaseRejectRequirementId",
                table: "CasePrerequisiteValues",
                column: "CaseRejectRequirementId",
                principalTable: "CaseRejectRequirements",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaseDocuments_CaseRejectRequirements_CaseRejectRequirementId",
                table: "CaseDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_CasePrerequisiteValues_CaseRejectRequirements_CaseRejectRequirementId",
                table: "CasePrerequisiteValues");

            migrationBuilder.DropIndex(
                name: "IX_CasePrerequisiteValues_CaseRejectRequirementId",
                table: "CasePrerequisiteValues");

            migrationBuilder.DropIndex(
                name: "IX_CaseDocuments_CaseRejectRequirementId",
                table: "CaseDocuments");

            migrationBuilder.DropColumn(
                name: "CaseRejectRequirementId",
                table: "CasePrerequisiteValues");

            migrationBuilder.DropColumn(
                name: "CaseRejectRequirementId",
                table: "CaseDocuments");
        }
    }
}
