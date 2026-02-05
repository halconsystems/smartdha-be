using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.CBMSApplicationDb
{
    /// <inheritdoc />
    public partial class Changesdoneclub : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Facilities_ClubCategories_ClubCategoryId",
                table: "Facilities");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceDefinitions_ClubCategories_ClubServiceCategoryId",
                table: "ServiceDefinitions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClubCategories",
                table: "ClubCategories");

            migrationBuilder.RenameTable(
                name: "ClubCategories",
                newName: "ClubServiceCategories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClubServiceCategories",
                table: "ClubServiceCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Facilities_ClubServiceCategories_ClubCategoryId",
                table: "Facilities",
                column: "ClubCategoryId",
                principalTable: "ClubServiceCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceDefinitions_ClubServiceCategories_ClubServiceCategoryId",
                table: "ServiceDefinitions",
                column: "ClubServiceCategoryId",
                principalTable: "ClubServiceCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Facilities_ClubServiceCategories_ClubCategoryId",
                table: "Facilities");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceDefinitions_ClubServiceCategories_ClubServiceCategoryId",
                table: "ServiceDefinitions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClubServiceCategories",
                table: "ClubServiceCategories");

            migrationBuilder.RenameTable(
                name: "ClubServiceCategories",
                newName: "ClubCategories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClubCategories",
                table: "ClubCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Facilities_ClubCategories_ClubCategoryId",
                table: "Facilities",
                column: "ClubCategoryId",
                principalTable: "ClubCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceDefinitions_ClubCategories_ClubServiceCategoryId",
                table: "ServiceDefinitions",
                column: "ClubServiceCategoryId",
                principalTable: "ClubCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
