using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Data.SQLite
{
    /// <inheritdoc />
    public partial class AddFieldsToMemberSpouse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CnicFrontImage",
                table: "MemberSpouses",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CnicBackImage",
                table: "MemberSpouses",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cnic",
                table: "MemberSpouses",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CnicExpiry",
                table: "MemberSpouses",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1));
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CnicFrontImage",
                table: "MemberSpouses");

            migrationBuilder.DropColumn(
                name: "CnicBackImage",
                table: "MemberSpouses");

            migrationBuilder.DropColumn(
                name: "Cnic",
                table: "MemberSpouses");

            migrationBuilder.DropColumn(
                name: "CnicExpiry",
                table: "MemberSpouses");
        }

    }
}
