using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Data.SQLite
{
    /// <inheritdoc />
    public partial class Panicdispatchchanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptedAtUtc",
                table: "PanicDispatches");

            migrationBuilder.DropColumn(
                name: "ArrivedAtUtc",
                table: "PanicDispatches");

            migrationBuilder.DropColumn(
                name: "AssignedAtUtc",
                table: "PanicDispatches");

            migrationBuilder.DropColumn(
                name: "CancelledAtUtc",
                table: "PanicDispatches");

            migrationBuilder.DropColumn(
                name: "CompletedAtUtc",
                table: "PanicDispatches");

            migrationBuilder.DropColumn(
                name: "OnRouteAtUtc",
                table: "PanicDispatches");

            migrationBuilder.AddColumn<DateTime>(
                name: "AcceptedAt",
                table: "PanicDispatches",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ArrivedAt",
                table: "PanicDispatches",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedAt",
                table: "PanicDispatches",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                table: "PanicDispatches",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "PanicDispatches",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OnRouteAt",
                table: "PanicDispatches",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptedAt",
                table: "PanicDispatches");

            migrationBuilder.DropColumn(
                name: "ArrivedAt",
                table: "PanicDispatches");

            migrationBuilder.DropColumn(
                name: "AssignedAt",
                table: "PanicDispatches");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "PanicDispatches");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "PanicDispatches");

            migrationBuilder.DropColumn(
                name: "OnRouteAt",
                table: "PanicDispatches");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "AcceptedAtUtc",
                table: "PanicDispatches",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ArrivedAtUtc",
                table: "PanicDispatches",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "AssignedAtUtc",
                table: "PanicDispatches",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CancelledAtUtc",
                table: "PanicDispatches",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CompletedAtUtc",
                table: "PanicDispatches",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "OnRouteAtUtc",
                table: "PanicDispatches",
                type: "datetimeoffset",
                nullable: true);
        }
    }
}
