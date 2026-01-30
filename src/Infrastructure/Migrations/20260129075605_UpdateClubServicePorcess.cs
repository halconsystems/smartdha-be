using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClubServicePorcess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AddColumn<bool>(
                name: "Action",
                table: "ClubProcess",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ActionName",
                table: "ClubProcess",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActionType",
                table: "ClubProcess",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FoodType",
                table: "ClubProcess",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageURL",
                table: "ClubProcess",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "ClubProcess",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPriceVisible",
                table: "ClubProcess",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Price",
                table: "ClubProcess",
                type: "nvarchar(max)",
                nullable: true);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.DropColumn(
                name: "Action",
                table: "ClubProcess");

            migrationBuilder.DropColumn(
                name: "ActionName",
                table: "ClubProcess");

            migrationBuilder.DropColumn(
                name: "ActionType",
                table: "ClubProcess");

            migrationBuilder.DropColumn(
                name: "FoodType",
                table: "ClubProcess");

            migrationBuilder.DropColumn(
                name: "ImageURL",
                table: "ClubProcess");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "ClubProcess");

            migrationBuilder.DropColumn(
                name: "IsPriceVisible",
                table: "ClubProcess");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "ClubProcess");
        }
    }
}
