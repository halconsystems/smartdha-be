using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Data.SQLite
{
    /// <inheritdoc />
    public partial class AppRoleModuletablecreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ClubId",
                table: "AppRoles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ClubId",
                table: "AppRolePermissions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppRoleModules",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppRoleModules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppRoleModules_AppRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AppRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppRoleModules_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Club",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Club", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppRolePermissions_ClubId",
                table: "AppRolePermissions",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_AppRoleModules_ModuleId",
                table: "AppRoleModules",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_AppRoleModules_RoleId",
                table: "AppRoleModules",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppRolePermissions_Club_ClubId",
                table: "AppRolePermissions",
                column: "ClubId",
                principalTable: "Club",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppRolePermissions_Club_ClubId",
                table: "AppRolePermissions");

            migrationBuilder.DropTable(
                name: "AppRoleModules");

            migrationBuilder.DropTable(
                name: "Club");

            migrationBuilder.DropIndex(
                name: "IX_AppRolePermissions_ClubId",
                table: "AppRolePermissions");

            migrationBuilder.DropColumn(
                name: "ClubId",
                table: "AppRoles");

            migrationBuilder.DropColumn(
                name: "ClubId",
                table: "AppRolePermissions");
        }
    }
}
