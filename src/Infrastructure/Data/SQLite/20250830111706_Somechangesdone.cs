using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Data.SQLite
{
    /// <inheritdoc />
    public partial class Somechangesdone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserModuleAssignments_Modules_ModuleId",
                table: "UserModuleAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserModuleAssignments_Users_UserId",
                table: "UserModuleAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissionAssignments_AppPermissions_PermissionId",
                table: "UserPermissionAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissionAssignments_Users_UserId",
                table: "UserPermissionAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSubModuleAssignments_SubModules_SubModuleId",
                table: "UserSubModuleAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSubModuleAssignments_Users_UserId",
                table: "UserSubModuleAssignments");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "UserModuleAssignments",
                type: "nvarchar(85)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModuleId1",
                table: "UserModuleAssignments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserModuleAssignments_ApplicationUserId",
                table: "UserModuleAssignments",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserModuleAssignments_ModuleId1",
                table: "UserModuleAssignments",
                column: "ModuleId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserModuleAssignments_Modules_ModuleId",
                table: "UserModuleAssignments",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserModuleAssignments_Modules_ModuleId1",
                table: "UserModuleAssignments",
                column: "ModuleId1",
                principalTable: "Modules",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserModuleAssignments_Users_ApplicationUserId",
                table: "UserModuleAssignments",
                column: "ApplicationUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserModuleAssignments_Users_UserId",
                table: "UserModuleAssignments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissionAssignments_AppPermissions_PermissionId",
                table: "UserPermissionAssignments",
                column: "PermissionId",
                principalTable: "AppPermissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissionAssignments_Users_UserId",
                table: "UserPermissionAssignments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubModuleAssignments_SubModules_SubModuleId",
                table: "UserSubModuleAssignments",
                column: "SubModuleId",
                principalTable: "SubModules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubModuleAssignments_Users_UserId",
                table: "UserSubModuleAssignments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserModuleAssignments_Modules_ModuleId",
                table: "UserModuleAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserModuleAssignments_Modules_ModuleId1",
                table: "UserModuleAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserModuleAssignments_Users_ApplicationUserId",
                table: "UserModuleAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserModuleAssignments_Users_UserId",
                table: "UserModuleAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissionAssignments_AppPermissions_PermissionId",
                table: "UserPermissionAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissionAssignments_Users_UserId",
                table: "UserPermissionAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSubModuleAssignments_SubModules_SubModuleId",
                table: "UserSubModuleAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSubModuleAssignments_Users_UserId",
                table: "UserSubModuleAssignments");

            migrationBuilder.DropIndex(
                name: "IX_UserModuleAssignments_ApplicationUserId",
                table: "UserModuleAssignments");

            migrationBuilder.DropIndex(
                name: "IX_UserModuleAssignments_ModuleId1",
                table: "UserModuleAssignments");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "UserModuleAssignments");

            migrationBuilder.DropColumn(
                name: "ModuleId1",
                table: "UserModuleAssignments");

            migrationBuilder.AddForeignKey(
                name: "FK_UserModuleAssignments_Modules_ModuleId",
                table: "UserModuleAssignments",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserModuleAssignments_Users_UserId",
                table: "UserModuleAssignments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissionAssignments_AppPermissions_PermissionId",
                table: "UserPermissionAssignments",
                column: "PermissionId",
                principalTable: "AppPermissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissionAssignments_Users_UserId",
                table: "UserPermissionAssignments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubModuleAssignments_SubModules_SubModuleId",
                table: "UserSubModuleAssignments",
                column: "SubModuleId",
                principalTable: "SubModules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubModuleAssignments_Users_UserId",
                table: "UserSubModuleAssignments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
