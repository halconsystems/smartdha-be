using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGroundStots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroundSlots_Grounds_GroundsId",
                table: "GroundSlots");

            migrationBuilder.AlterColumn<Guid>(
                name: "GroundsId",
                table: "GroundSlots",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_GroundSlots_Grounds_GroundsId",
                table: "GroundSlots",
                column: "GroundsId",
                principalTable: "Grounds",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroundSlots_Grounds_GroundsId",
                table: "GroundSlots");

            migrationBuilder.AlterColumn<Guid>(
                name: "GroundsId",
                table: "GroundSlots",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GroundSlots_Grounds_GroundsId",
                table: "GroundSlots",
                column: "GroundsId",
                principalTable: "Grounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
