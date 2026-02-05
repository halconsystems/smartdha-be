using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.CBMSApplicationDb
{
    /// <inheritdoc />
    public partial class Changesinservicestable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacilityUnitServices_FacilityServices_FacilityServiceId",
                table: "FacilityUnitServices");

            migrationBuilder.DropTable(
                name: "FacilityServices");

            migrationBuilder.DropColumn(
                name: "OverridePrice",
                table: "FacilityUnitServices");

            migrationBuilder.RenameColumn(
                name: "FacilityServiceId",
                table: "FacilityUnitServices",
                newName: "ServiceDefinitionId");

            migrationBuilder.RenameIndex(
                name: "IX_FacilityUnitServices_FacilityServiceId",
                table: "FacilityUnitServices",
                newName: "IX_FacilityUnitServices_ServiceDefinitionId");

            migrationBuilder.AddColumn<bool>(
                name: "IsComplimentary",
                table: "FacilityUnitServices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "FacilityUnitServices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "ServiceDefinitions",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClubServiceCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsQuantityBased = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceDefinitions_ClubCategories_ClubServiceCategoryId",
                        column: x => x.ClubServiceCategoryId,
                        principalTable: "ClubCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDefinitions_ClubServiceCategoryId",
                table: "ServiceDefinitions",
                column: "ClubServiceCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_FacilityUnitServices_ServiceDefinitions_ServiceDefinitionId",
                table: "FacilityUnitServices",
                column: "ServiceDefinitionId",
                principalTable: "ServiceDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacilityUnitServices_ServiceDefinitions_ServiceDefinitionId",
                table: "FacilityUnitServices");

            migrationBuilder.DropTable(
                name: "ServiceDefinitions");

            migrationBuilder.DropColumn(
                name: "IsComplimentary",
                table: "FacilityUnitServices");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "FacilityUnitServices");

            migrationBuilder.RenameColumn(
                name: "ServiceDefinitionId",
                table: "FacilityUnitServices",
                newName: "FacilityServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_FacilityUnitServices_ServiceDefinitionId",
                table: "FacilityUnitServices",
                newName: "IX_FacilityUnitServices_FacilityServiceId");

            migrationBuilder.AddColumn<decimal>(
                name: "OverridePrice",
                table: "FacilityUnitServices",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FacilityServices",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FacilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    IsComplimentary = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsQuantityBased = table.Column<bool>(type: "bit", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacilityServices_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FacilityServices_FacilityId",
                table: "FacilityServices",
                column: "FacilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_FacilityUnitServices_FacilityServices_FacilityServiceId",
                table: "FacilityUnitServices",
                column: "FacilityServiceId",
                principalTable: "FacilityServices",
                principalColumn: "Id");
        }
    }
}
