using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ThirdCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MembershipStartDate",
                table: "UserClubMembership",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "MembershipEndDate",
                table: "UserClubMembership",
                newName: "EndDate");

            migrationBuilder.RenameColumn(
                name: "ServiceName",
                table: "Services",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "RoomTypeId",
                table: "Rooms",
                newName: "ResidenceTypeId");

            migrationBuilder.RenameColumn(
                name: "RoomNo",
                table: "Rooms",
                newName: "No");

            migrationBuilder.RenameColumn(
                name: "RoomName",
                table: "Rooms",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "CategoryName",
                table: "RoomCategories",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ResidenceTypes",
                table: "ResidenceTypes",
                newName: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "UserClubMembership",
                newName: "MembershipStartDate");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "UserClubMembership",
                newName: "MembershipEndDate");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Services",
                newName: "ServiceName");

            migrationBuilder.RenameColumn(
                name: "ResidenceTypeId",
                table: "Rooms",
                newName: "RoomTypeId");

            migrationBuilder.RenameColumn(
                name: "No",
                table: "Rooms",
                newName: "RoomNo");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Rooms",
                newName: "RoomName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "RoomCategories",
                newName: "CategoryName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ResidenceTypes",
                newName: "ResidenceTypes");
        }
    }
}
