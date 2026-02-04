using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Data.SQLite
{
    /// <inheritdoc />
    public partial class MemberShipColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "PassportExpiryDate",
                table: "MemberSpouses",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassportNo",
                table: "MemberSpouses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UploadPassport",
                table: "MemberSpouses",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArmsSvc",
                table: "MemberRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CauseRetirement",
                table: "MemberRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NonPassportCopy",
                table: "MemberRequests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Ranks",
                table: "MemberRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceNo",
                table: "MemberRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Serving",
                table: "MemberRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PassportExpiryDate",
                table: "MemberSpouses");

            migrationBuilder.DropColumn(
                name: "PassportNo",
                table: "MemberSpouses");

            migrationBuilder.DropColumn(
                name: "UploadPassport",
                table: "MemberSpouses");

            migrationBuilder.DropColumn(
                name: "ArmsSvc",
                table: "MemberRequests");

            migrationBuilder.DropColumn(
                name: "CauseRetirement",
                table: "MemberRequests");

            migrationBuilder.DropColumn(
                name: "NonPassportCopy",
                table: "MemberRequests");

            migrationBuilder.DropColumn(
                name: "Ranks",
                table: "MemberRequests");

            migrationBuilder.DropColumn(
                name: "ServiceNo",
                table: "MemberRequests");

            migrationBuilder.DropColumn(
                name: "Serving",
                table: "MemberRequests");
        }
    }
}
