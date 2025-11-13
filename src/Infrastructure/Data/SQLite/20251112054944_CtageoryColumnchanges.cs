using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DHAFacilitationAPIs.Infrastructure.Data.SQLite
{
    /// <inheritdoc />
    public partial class CtageoryColumnchanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ComplaintCategories",
                keyColumn: "Id",
                keyValue: new Guid("150ba363-45ef-4f19-b03f-57876cc26562"));

            migrationBuilder.DeleteData(
                table: "ComplaintCategories",
                keyColumn: "Id",
                keyValue: new Guid("56214b01-8749-480d-b7a9-1d82a52d2003"));

            migrationBuilder.DeleteData(
                table: "ComplaintCategories",
                keyColumn: "Id",
                keyValue: new Guid("600a3d2a-b165-4b62-a254-d7592004ffc9"));

            migrationBuilder.DeleteData(
                table: "ComplaintCategories",
                keyColumn: "Id",
                keyValue: new Guid("a767a331-729b-4b49-95be-1798f154f855"));

            migrationBuilder.DeleteData(
                table: "ComplaintCategories",
                keyColumn: "Id",
                keyValue: new Guid("c0b52dcd-1688-4ef8-9df3-228b2738ba03"));

            migrationBuilder.DeleteData(
                table: "ComplaintCategories",
                keyColumn: "Id",
                keyValue: new Guid("c2d252f8-def9-423a-88e7-c14ea6eb7263"));

            migrationBuilder.DeleteData(
                table: "ComplaintPriorities",
                keyColumn: "Id",
                keyValue: new Guid("376b5c7d-282f-4a64-83c0-f665c30d8c2e"));

            migrationBuilder.DeleteData(
                table: "ComplaintPriorities",
                keyColumn: "Id",
                keyValue: new Guid("bdee021c-5c6b-419e-9be5-eb0ec3cb8fc7"));

            migrationBuilder.DeleteData(
                table: "ComplaintPriorities",
                keyColumn: "Id",
                keyValue: new Guid("d96ae061-6c92-4ede-947d-94df256394fd"));

            migrationBuilder.DeleteData(
                table: "ComplaintPriorities",
                keyColumn: "Id",
                keyValue: new Guid("f0b28ca3-6fb0-417f-9a68-ea7dea040b4a"));

            migrationBuilder.AddColumn<string>(
                name: "ColorCode",
                table: "ComplaintCategories",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorCode",
                table: "ComplaintCategories");

            migrationBuilder.InsertData(
                table: "ComplaintCategories",
                columns: new[] { "Id", "Code", "Created", "CreatedBy", "Description", "IsActive", "IsDeleted", "LastModified", "LastModifiedBy", "Name" },
                values: new object[,]
                {
                    { new Guid("150ba363-45ef-4f19-b03f-57876cc26562"), "ROAD", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Road / Infrastructure" },
                    { new Guid("56214b01-8749-480d-b7a9-1d82a52d2003"), "SECURITY", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Security" },
                    { new Guid("600a3d2a-b165-4b62-a254-d7592004ffc9"), "MAINT", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Maintenance" },
                    { new Guid("a767a331-729b-4b49-95be-1798f154f855"), "ELECTRIC", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Electricity" },
                    { new Guid("c0b52dcd-1688-4ef8-9df3-228b2738ba03"), "WATER", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Water Supply" },
                    { new Guid("c2d252f8-def9-423a-88e7-c14ea6eb7263"), "OTHER", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Other" }
                });

            migrationBuilder.InsertData(
                table: "ComplaintPriorities",
                columns: new[] { "Id", "Code", "Created", "CreatedBy", "IsActive", "IsDeleted", "LastModified", "LastModifiedBy", "Name", "Weight" },
                values: new object[,]
                {
                    { new Guid("376b5c7d-282f-4a64-83c0-f665c30d8c2e"), "MED", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Medium", 2 },
                    { new Guid("bdee021c-5c6b-419e-9be5-eb0ec3cb8fc7"), "URG", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Urgent", 4 },
                    { new Guid("d96ae061-6c92-4ede-947d-94df256394fd"), "HIGH", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "High", 3 },
                    { new Guid("f0b28ca3-6fb0-417f-9a68-ea7dea040b4a"), "LOW", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Low", 1 }
                });
        }
    }
}
