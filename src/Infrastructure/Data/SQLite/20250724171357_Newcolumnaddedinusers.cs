using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Data.SQLite
{
    /// <inheritdoc />
    public partial class Newcolumnaddedinusers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserModuleAssignments_Users_UserId1",
                table: "UserModuleAssignments");

            migrationBuilder.DropIndex(
                name: "IX_UserModuleAssignments_UserId1",
                table: "UserModuleAssignments");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserModuleAssignments");

            migrationBuilder.AddColumn<int>(
                name: "AppType",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MobileNo",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RegisteredEmail",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegisteredMobileNo",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationDate",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistrationNo",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserModuleAssignments",
                type: "nvarchar(85)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<int>(
                name: "AppType",
                table: "ApplicationLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserModuleAssignments_UserId",
                table: "UserModuleAssignments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserModuleAssignments_Users_UserId",
                table: "UserModuleAssignments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserModuleAssignments_Users_UserId",
                table: "UserModuleAssignments");

            migrationBuilder.DropIndex(
                name: "IX_UserModuleAssignments_UserId",
                table: "UserModuleAssignments");

            migrationBuilder.DropColumn(
                name: "AppType",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MobileNo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RegisteredEmail",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RegisteredMobileNo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RegistrationDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RegistrationNo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AppType",
                table: "ApplicationLogs");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "UserModuleAssignments",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(85)");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "UserModuleAssignments",
                type: "nvarchar(85)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserModuleAssignments_UserId1",
                table: "UserModuleAssignments",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserModuleAssignments_Users_UserId1",
                table: "UserModuleAssignments",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
