using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Data.SQLite
{
    /// <inheritdoc />
    public partial class Disptachcolumnadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AcceptedAtAddress",
                table: "PanicDispatches",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "AcceptedAtLatitude",
                table: "PanicDispatches",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "AcceptedAtLongitude",
                table: "PanicDispatches",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DistanceFromPanicKm",
                table: "PanicDispatches",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLocationUpdateAt",
                table: "PanicDispatches",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptedAtAddress",
                table: "PanicDispatches");

            migrationBuilder.DropColumn(
                name: "AcceptedAtLatitude",
                table: "PanicDispatches");

            migrationBuilder.DropColumn(
                name: "AcceptedAtLongitude",
                table: "PanicDispatches");

            migrationBuilder.DropColumn(
                name: "DistanceFromPanicKm",
                table: "PanicDispatches");

            migrationBuilder.DropColumn(
                name: "LastLocationUpdateAt",
                table: "PanicDispatches");
        }
    }
}
