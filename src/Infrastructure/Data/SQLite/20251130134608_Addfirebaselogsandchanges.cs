using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Data.SQLite
{
    /// <inheritdoc />
    public partial class Addfirebaselogsandchanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnRouteAt",
                table: "PanicDispatches");

            migrationBuilder.AddColumn<DateTime>(
                name: "LogoutAt",
                table: "UserLoginAudits",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoutAt",
                table: "UserLoginAudits");

            migrationBuilder.AddColumn<DateTime>(
                name: "OnRouteAt",
                table: "PanicDispatches",
                type: "datetime2",
                nullable: true);
        }
    }
}
