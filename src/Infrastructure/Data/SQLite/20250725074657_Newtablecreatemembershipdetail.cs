using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Data.SQLite
{
    /// <inheritdoc />
    public partial class Newtablecreatemembershipdetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UserModuleAssignments",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UserModuleAssignments",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "SubModules",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "SubModules",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "RolePermissions",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "RolePermissions",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "RoleAssignments",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "RoleAssignments",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Modules",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Modules",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Membershipdetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MembershipId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MembershipName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Membershipdetails", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Membershipdetails");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UserModuleAssignments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UserModuleAssignments");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "SubModules");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "SubModules");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "RoleAssignments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "RoleAssignments");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Modules");
        }
    }
}
