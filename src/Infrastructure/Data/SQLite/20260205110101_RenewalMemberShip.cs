using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Data.SQLite
{
    /// <inheritdoc />
    public partial class RenewalMemberShip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AddColumn<bool>(
                name: "IsChild",
                table: "MemberSpouses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "WifeNo",
                table: "MemberSpouses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "ValidUntil",
                table: "MemberRequests",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<Guid>(
                name: "MemberSpouseId",
                table: "MemberChildrens",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SpouseId",
                table: "MemberChildrens",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_MemberChildrens_MemberSpouseId",
                table: "MemberChildrens",
                column: "MemberSpouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberChildrens_MemberSpouses_MemberSpouseId",
                table: "MemberChildrens",
                column: "MemberSpouseId",
                principalTable: "MemberSpouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberChildrens_MemberSpouses_MemberSpouseId",
                table: "MemberChildrens");

            migrationBuilder.DropIndex(
                name: "IX_MemberChildrens_MemberSpouseId",
                table: "MemberChildrens");

            migrationBuilder.DropColumn(
                name: "IsChild",
                table: "MemberSpouses");

            migrationBuilder.DropColumn(
                name: "WifeNo",
                table: "MemberSpouses");

            migrationBuilder.DropColumn(
                name: "ValidUntil",
                table: "MemberRequests");

            migrationBuilder.DropColumn(
                name: "MemberSpouseId",
                table: "MemberChildrens");

            migrationBuilder.DropColumn(
                name: "SpouseId",
                table: "MemberChildrens");

           
        }
    }
}
