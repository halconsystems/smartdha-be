using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGroundSlots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromDate",
                table: "GroundSlots");

            migrationBuilder.DropColumn(
                name: "ToDate",
                table: "GroundSlots");

            migrationBuilder.RenameColumn(
                name: "ToDateOnly",
                table: "GroundSlots",
                newName: "SlotDateOnly");

            migrationBuilder.RenameColumn(
                name: "FromDateOnly",
                table: "GroundSlots",
                newName: "SlotDate");


            migrationBuilder.CreateTable(
                name: "ClubFeeCategory",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubFeeCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClubFeeDefinition",
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
                    table.PrimaryKey("PK_ClubFeeDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClubFeeDefinition_ClubProcess_ProcessId",
                        column: x => x.ProcessId,
                        principalTable: "ClubProcess",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClubFeeOption",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeeDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeeCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProcessingDays = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubFeeOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClubFeeOption_ClubFeeCategory_FeeCategoryId",
                        column: x => x.FeeCategoryId,
                        principalTable: "ClubFeeCategory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClubFeeOption_ClubFeeDefinition_FeeDefinitionId",
                        column: x => x.FeeDefinitionId,
                        principalTable: "ClubFeeDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

          

            migrationBuilder.CreateIndex(
                name: "IX_ClubFeeDefinition_ProcessId",
                table: "ClubFeeDefinition",
                column: "ProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_ClubFeeOption_FeeCategoryId",
                table: "ClubFeeOption",
                column: "FeeCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ClubFeeOption_FeeDefinitionId",
                table: "ClubFeeOption",
                column: "FeeDefinitionId");

           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.DropTable(
                name: "ClubFeeOption");

            migrationBuilder.DropTable(
                name: "ClubFeeCategory");

            migrationBuilder.DropTable(
                name: "ClubFeeDefinition");


            migrationBuilder.RenameColumn(
                name: "SlotDateOnly",
                table: "GroundSlots",
                newName: "ToDateOnly");

            migrationBuilder.RenameColumn(
                name: "SlotDate",
                table: "GroundSlots",
                newName: "FromDateOnly");

            migrationBuilder.AddColumn<DateTime>(
                name: "FromDate",
                table: "GroundSlots",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ToDate",
                table: "GroundSlots",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
