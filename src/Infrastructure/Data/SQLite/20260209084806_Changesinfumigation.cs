using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Data.SQLite
{
    /// <inheritdoc />
    public partial class Changesinfumigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          

            migrationBuilder.AddColumn<Guid>(
                name: "FemServiceId",
                table: "TankerSizes",
                type: "uniqueidentifier",
                nullable: true);

           
            migrationBuilder.CreateIndex(
                name: "IX_TankerSizes_FemServiceId",
                table: "TankerSizes",
                column: "FemServiceId");

           
            migrationBuilder.AddForeignKey(
                name: "FK_TankerSizes_FemServices_FemServiceId",
                table: "TankerSizes",
                column: "FemServiceId",
                principalTable: "FemServices",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TankerSizes_FemServices_FemServiceId",
                table: "TankerSizes");


            migrationBuilder.DropIndex(
                name: "IX_TankerSizes_FemServiceId",
                table: "TankerSizes");


            migrationBuilder.DropColumn(
                name: "FemServiceId",
                table: "TankerSizes");
        }
    }
}
