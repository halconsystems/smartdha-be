using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClubTableMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Action",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "ActionName",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "ActionType",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "IsPriceVisible",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Facilities");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "Facilities",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "Facilities",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AddColumn<bool>(
                name: "Action",
                table: "Facilities",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActionName",
                table: "Facilities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActionType",
                table: "Facilities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Facilities",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPriceVisible",
                table: "Facilities",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Price",
                table: "Facilities",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
