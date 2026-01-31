using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClubTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClubCategories_Clubs_ClubId",
                table: "ClubCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ClubFeeDefinition_ClubProcess_ProcessId",
                table: "ClubFeeDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_ClubProcessPrerequisite_ClubProcess_ProcessId",
                table: "ClubProcessPrerequisite");

            migrationBuilder.DropTable(
                name: "ClubProcess");

            migrationBuilder.DropTable(
                name: "ClubServiceImages");

            migrationBuilder.DropIndex(
                name: "IX_ClubCategories_ClubId",
                table: "ClubCategories");

          

            migrationBuilder.DropColumn(
                name: "ClubId",
                table: "ClubCategories");

           

            migrationBuilder.RenameColumn(
                name: "ProcessId",
                table: "ClubProcessPrerequisite",
                newName: "ClubCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_ClubProcessPrerequisite_ProcessId",
                table: "ClubProcessPrerequisite",
                newName: "IX_ClubProcessPrerequisite_ClubCategoryId");

            migrationBuilder.RenameColumn(
                name: "ProcessId",
                table: "ClubFeeDefinition",
                newName: "ClubCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_ClubFeeDefinition_ProcessId",
                table: "ClubFeeDefinition",
                newName: "IX_ClubFeeDefinition_ClubCategoryId");

            

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Clubs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Clubs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ClubCategories",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ClubCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "ClubCategories",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

           

            migrationBuilder.CreateTable(
                name: "Facilities",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ClubCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FoodType = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: true),
                    IsPriceVisible = table.Column<bool>(type: "bit", nullable: true),
                    Action = table.Column<bool>(type: "bit", nullable: true),
                    ActionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Facilities_ClubCategories_ClubCategoryId",
                        column: x => x.ClubCategoryId,
                        principalTable: "ClubCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClubFacilities",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClubId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FacilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    IsPriceVisible = table.Column<bool>(type: "bit", nullable: false),
                    HasAction = table.Column<bool>(type: "bit", nullable: false),
                    ActionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubFacilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClubFacilities_Clubs_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Clubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClubFacilities_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClubFacilities_ClubId",
                table: "ClubFacilities",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_ClubFacilities_FacilityId",
                table: "ClubFacilities",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_ClubCategoryId",
                table: "Facilities",
                column: "ClubCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClubFeeDefinition_ClubCategories_ClubCategoryId",
                table: "ClubFeeDefinition",
                column: "ClubCategoryId",
                principalTable: "ClubCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClubProcessPrerequisite_ClubCategories_ClubCategoryId",
                table: "ClubProcessPrerequisite",
                column: "ClubCategoryId",
                principalTable: "ClubCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClubFeeDefinition_ClubCategories_ClubCategoryId",
                table: "ClubFeeDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_ClubProcessPrerequisite_ClubCategories_ClubCategoryId",
                table: "ClubProcessPrerequisite");

            migrationBuilder.DropTable(
                name: "ClubFacilities");

            migrationBuilder.DropTable(
                name: "Facilities");



            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Clubs");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Clubs");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ClubCategories");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "ClubCategories");

            migrationBuilder.RenameColumn(
                name: "SubDivision",
                table: "UserProperty",
                newName: "Sector");

            migrationBuilder.RenameColumn(
                name: "ClubCategoryId",
                table: "ClubProcessPrerequisite",
                newName: "ProcessId");

            migrationBuilder.RenameIndex(
                name: "IX_ClubProcessPrerequisite_ClubCategoryId",
                table: "ClubProcessPrerequisite",
                newName: "IX_ClubProcessPrerequisite_ProcessId");

            migrationBuilder.RenameColumn(
                name: "ClubCategoryId",
                table: "ClubFeeDefinition",
                newName: "ProcessId");

            migrationBuilder.RenameIndex(
                name: "IX_ClubFeeDefinition_ClubCategoryId",
                table: "ClubFeeDefinition",
                newName: "IX_ClubFeeDefinition_ProcessId");




            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ClubCategories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<Guid>(
                name: "ClubId",
                table: "ClubCategories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            
            migrationBuilder.CreateTable(
                name: "ClubProcess",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<bool>(type: "bit", nullable: true),
                    ActionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FoodType = table.Column<int>(type: "int", nullable: true),
                    ImageURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Instruction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: true),
                    IsButton = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsFeeAtSubmission = table.Column<bool>(type: "bit", nullable: false),
                    IsFeeRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsInstructionAtStart = table.Column<bool>(type: "bit", nullable: false),
                    IsPriceVisible = table.Column<bool>(type: "bit", nullable: true),
                    IsVoucherPossible = table.Column<bool>(type: "bit", nullable: false),
                    IsfeeSubmit = table.Column<bool>(type: "bit", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Price = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubProcess", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClubProcess_ClubCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ClubCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClubServiceImages",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageExtension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageURL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubServiceImages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClubCategories_ClubId",
                table: "ClubCategories",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_ClubProcess_CategoryId",
                table: "ClubProcess",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClubCategories_Clubs_ClubId",
                table: "ClubCategories",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClubFeeDefinition_ClubProcess_ProcessId",
                table: "ClubFeeDefinition",
                column: "ProcessId",
                principalTable: "ClubProcess",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClubProcessPrerequisite_ClubProcess_ProcessId",
                table: "ClubProcessPrerequisite",
                column: "ProcessId",
                principalTable: "ClubProcess",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
