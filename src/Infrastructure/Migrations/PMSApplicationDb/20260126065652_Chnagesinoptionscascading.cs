using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.PMSApplicationDb
{
    /// <inheritdoc />
    public partial class Chnagesinoptionscascading : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PrerequisiteDefinitionId1",
                table: "PrerequisiteOptions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteOptions_PrerequisiteDefinitionId1",
                table: "PrerequisiteOptions",
                column: "PrerequisiteDefinitionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_PrerequisiteOptions_PrerequisiteDefinitions_PrerequisiteDefinitionId1",
                table: "PrerequisiteOptions",
                column: "PrerequisiteDefinitionId1",
                principalTable: "PrerequisiteDefinitions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrerequisiteOptions_PrerequisiteDefinitions_PrerequisiteDefinitionId1",
                table: "PrerequisiteOptions");

            migrationBuilder.DropIndex(
                name: "IX_PrerequisiteOptions_PrerequisiteDefinitionId1",
                table: "PrerequisiteOptions");

            migrationBuilder.DropColumn(
                name: "PrerequisiteDefinitionId1",
                table: "PrerequisiteOptions");
        }
    }
}
