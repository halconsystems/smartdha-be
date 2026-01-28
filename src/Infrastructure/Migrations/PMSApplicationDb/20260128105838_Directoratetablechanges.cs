using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.PMSApplicationDb
{
    /// <inheritdoc />
    public partial class Directoratetablechanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                table: "Directorates",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_CaseStepHistories_DirectorateId",
                table: "CaseStepHistories",
                column: "DirectorateId");

            migrationBuilder.AddForeignKey(
                name: "FK_CaseStepHistories_Directorates_DirectorateId",
                table: "CaseStepHistories",
                column: "DirectorateId",
                principalTable: "Directorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaseStepHistories_Directorates_DirectorateId",
                table: "CaseStepHistories");

            migrationBuilder.DropIndex(
                name: "IX_CaseStepHistories_DirectorateId",
                table: "CaseStepHistories");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Directorates");
        }
    }
}
