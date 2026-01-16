using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.PMSApplicationDb
{
    /// <inheritdoc />
    public partial class Casefeeadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFeeRequired",
                table: "ServiceProcesses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsNadraVerificationRequired",
                table: "ServiceProcesses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsfeeSubmit",
                table: "ServiceProcesses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "FeeDefinitions",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProcessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeeType = table.Column<int>(type: "int", nullable: false),
                    FixedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AreaUnit = table.Column<int>(type: "int", nullable: true),
                    AllowOverride = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeeDefinitions_ServiceProcesses_ProcessId",
                        column: x => x.ProcessId,
                        principalTable: "ServiceProcesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeeSlabs",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeeDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromArea = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ToArea = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeSlabs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeeSlabs_FeeDefinitions_FeeDefinitionId",
                        column: x => x.FeeDefinitionId,
                        principalTable: "FeeDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CaseFees",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeeDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PropertyArea = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AreaUnit = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FeeSlabId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsOverridden = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseFees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseFees_FeeDefinitions_FeeDefinitionId",
                        column: x => x.FeeDefinitionId,
                        principalTable: "FeeDefinitions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CaseFees_FeeSlabs_FeeSlabId",
                        column: x => x.FeeSlabId,
                        principalTable: "FeeSlabs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CaseFees_PropertyCases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "PropertyCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CaseFees_CaseId",
                table: "CaseFees",
                column: "CaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CaseFees_FeeDefinitionId",
                table: "CaseFees",
                column: "FeeDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseFees_FeeSlabId",
                table: "CaseFees",
                column: "FeeSlabId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeDefinitions_ProcessId",
                table: "FeeDefinitions",
                column: "ProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeSlabs_FeeDefinitionId_FromArea_ToArea",
                table: "FeeSlabs",
                columns: new[] { "FeeDefinitionId", "FromArea", "ToArea" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaseFees");

            migrationBuilder.DropTable(
                name: "FeeSlabs");

            migrationBuilder.DropTable(
                name: "FeeDefinitions");

            migrationBuilder.DropColumn(
                name: "IsFeeRequired",
                table: "ServiceProcesses");

            migrationBuilder.DropColumn(
                name: "IsNadraVerificationRequired",
                table: "ServiceProcesses");

            migrationBuilder.DropColumn(
                name: "IsfeeSubmit",
                table: "ServiceProcesses");
        }
    }
}
