using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class VisitorPassAndLuggaeEntityAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromDate",
                table: "VisitorPasses");

            migrationBuilder.RenameColumn(
                name: "ValidTill",
                table: "VisitorPasses",
                newName: "ValidTo");

            migrationBuilder.RenameColumn(
                name: "ToDate",
                table: "VisitorPasses",
                newName: "ValidFrom");

            migrationBuilder.RenameColumn(
                name: "QuickPickType",
                table: "VisitorPasses",
                newName: "VisitorPassType");

            migrationBuilder.RenameColumn(
                name: "ValidUpTo",
                table: "Vehicles",
                newName: "ValidTo");

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                table: "Workers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidTo",
                table: "Workers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                table: "Vehicles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                table: "UserFamilies",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidTo",
                table: "UserFamilies",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "LuggagePasses",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CNIC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VehicleLicensePlate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VehicleLicenseNo = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QRCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TagId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LuggagePasses", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LuggagePasses");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                table: "Workers");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                table: "Workers");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                table: "UserFamilies");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                table: "UserFamilies");

            migrationBuilder.RenameColumn(
                name: "VisitorPassType",
                table: "VisitorPasses",
                newName: "QuickPickType");

            migrationBuilder.RenameColumn(
                name: "ValidTo",
                table: "VisitorPasses",
                newName: "ValidTill");

            migrationBuilder.RenameColumn(
                name: "ValidFrom",
                table: "VisitorPasses",
                newName: "ToDate");

            migrationBuilder.RenameColumn(
                name: "ValidTo",
                table: "Vehicles",
                newName: "ValidUpTo");

            migrationBuilder.AddColumn<DateTime>(
                name: "FromDate",
                table: "VisitorPasses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
