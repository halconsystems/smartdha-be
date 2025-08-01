using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Data.SQLite
{
    /// <inheritdoc />
    public partial class Updatenonmembertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NonMemberVerifications_MembershipPurposes_PurposeId",
                table: "NonMemberVerifications");

            migrationBuilder.DropIndex(
                name: "IX_NonMemberVerifications_PurposeId",
                table: "NonMemberVerifications");

            migrationBuilder.DropColumn(
                name: "PurposeId",
                table: "NonMemberVerifications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PurposeId",
                table: "NonMemberVerifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_NonMemberVerifications_PurposeId",
                table: "NonMemberVerifications",
                column: "PurposeId");

            migrationBuilder.AddForeignKey(
                name: "FK_NonMemberVerifications_MembershipPurposes_PurposeId",
                table: "NonMemberVerifications",
                column: "PurposeId",
                principalTable: "MembershipPurposes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
