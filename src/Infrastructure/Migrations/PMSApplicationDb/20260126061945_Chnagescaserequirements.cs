using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.PMSApplicationDb
{
    /// <inheritdoc />
    public partial class Chnagescaserequirements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowedExtensions",
                table: "CaseRejectRequirements");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "CaseRejectRequirements");

            migrationBuilder.DropColumn(
                name: "MaxLength",
                table: "CaseRejectRequirements");

            migrationBuilder.DropColumn(
                name: "MinLength",
                table: "CaseRejectRequirements");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "CaseRejectRequirements");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "CaseRejectRequirements");

            migrationBuilder.AddColumn<Guid>(
                name: "PrerequisiteDefinitionId",
                table: "CaseRejectRequirements",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_CaseRejectRequirements_PrerequisiteDefinitionId",
                table: "CaseRejectRequirements",
                column: "PrerequisiteDefinitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CaseRejectRequirements_PrerequisiteDefinitions_PrerequisiteDefinitionId",
                table: "CaseRejectRequirements",
                column: "PrerequisiteDefinitionId",
                principalTable: "PrerequisiteDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaseRejectRequirements_PrerequisiteDefinitions_PrerequisiteDefinitionId",
                table: "CaseRejectRequirements");

            migrationBuilder.DropIndex(
                name: "IX_CaseRejectRequirements_PrerequisiteDefinitionId",
                table: "CaseRejectRequirements");

            migrationBuilder.DropColumn(
                name: "PrerequisiteDefinitionId",
                table: "CaseRejectRequirements");

            migrationBuilder.AddColumn<string>(
                name: "AllowedExtensions",
                table: "CaseRejectRequirements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "CaseRejectRequirements",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MaxLength",
                table: "CaseRejectRequirements",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinLength",
                table: "CaseRejectRequirements",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CaseRejectRequirements",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "CaseRejectRequirements",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
