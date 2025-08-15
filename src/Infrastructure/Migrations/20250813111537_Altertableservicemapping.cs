using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Altertableservicemapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ServicesId",
                table: "ServiceMappings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ServiceMappings_RoomId_ServiceId",
                table: "ServiceMappings",
                columns: new[] { "RoomId", "ServiceId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceMappings_ServicesId",
                table: "ServiceMappings",
                column: "ServicesId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceMappings_Rooms_RoomId",
                table: "ServiceMappings",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceMappings_Services_ServicesId",
                table: "ServiceMappings",
                column: "ServicesId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceMappings_Rooms_RoomId",
                table: "ServiceMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceMappings_Services_ServicesId",
                table: "ServiceMappings");

            migrationBuilder.DropIndex(
                name: "IX_ServiceMappings_RoomId_ServiceId",
                table: "ServiceMappings");

            migrationBuilder.DropIndex(
                name: "IX_ServiceMappings_ServicesId",
                table: "ServiceMappings");

            migrationBuilder.DropColumn(
                name: "ServicesId",
                table: "ServiceMappings");
        }
    }
}
