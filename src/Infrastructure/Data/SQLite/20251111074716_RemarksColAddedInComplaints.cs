using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DHAFacilitationAPIs.Infrastructure.Data.SQLite
{
    /// <inheritdoc />
    public partial class RemarksColAddedInComplaints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ComplaintCategories",
                keyColumn: "Id",
                keyValue: new Guid("05ec0df1-7142-4549-8040-9b3042d90ab9"));

            migrationBuilder.DeleteData(
                table: "ComplaintCategories",
                keyColumn: "Id",
                keyValue: new Guid("2ce0ae7b-1a10-42ad-823d-0c94e3e99804"));

            migrationBuilder.DeleteData(
                table: "ComplaintCategories",
                keyColumn: "Id",
                keyValue: new Guid("73d55319-c4b8-4cb0-935e-59abc2e8e608"));

            migrationBuilder.DeleteData(
                table: "ComplaintCategories",
                keyColumn: "Id",
                keyValue: new Guid("a81c3402-ead5-42d8-b961-97ec77bed9c5"));

            migrationBuilder.DeleteData(
                table: "ComplaintCategories",
                keyColumn: "Id",
                keyValue: new Guid("d21db04f-328d-4236-8aa6-b121088cdc96"));

            migrationBuilder.DeleteData(
                table: "ComplaintCategories",
                keyColumn: "Id",
                keyValue: new Guid("e99b2a7a-bf14-4441-b6cc-ef138ee382a6"));

            migrationBuilder.DeleteData(
                table: "ComplaintPriorities",
                keyColumn: "Id",
                keyValue: new Guid("107eed1b-99be-4816-b92d-360b77a4a4c6"));

            migrationBuilder.DeleteData(
                table: "ComplaintPriorities",
                keyColumn: "Id",
                keyValue: new Guid("284ce9f9-a0e7-483f-9671-04fea3f9d6e0"));

            migrationBuilder.DeleteData(
                table: "ComplaintPriorities",
                keyColumn: "Id",
                keyValue: new Guid("e4bfb355-1fd9-42c3-ab26-eae15e1d6778"));

            migrationBuilder.DeleteData(
                table: "ComplaintPriorities",
                keyColumn: "Id",
                keyValue: new Guid("fb7fc901-0c33-4f5d-ab2b-fe9b09b1d522"));

            migrationBuilder.AddColumn<string>(
                name: "AdminRemarks",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "ComplaintCategories",
                columns: new[] { "Id", "Code", "Created", "CreatedBy", "Description", "IsActive", "IsDeleted", "LastModified", "LastModifiedBy", "Name" },
                values: new object[,]
                {
                    { new Guid("1c508462-f2f0-4e5a-a88a-0a2d3f2eb1b8"), "WATER", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Water Supply" },
                    { new Guid("1d769d16-6c21-4db7-b3eb-4c321e2bc69a"), "MAINT", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Maintenance" },
                    { new Guid("2955554f-25d6-4267-b710-f751617c88a0"), "ROAD", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Road / Infrastructure" },
                    { new Guid("43cbb35e-0744-4ed7-a133-3516865883e5"), "ELECTRIC", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Electricity" },
                    { new Guid("981e9f52-42f9-44d4-aade-71c6a108eff7"), "OTHER", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Other" },
                    { new Guid("c149efdd-b3b3-4b72-946b-ee4defc640b5"), "SECURITY", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Security" }
                });

            migrationBuilder.InsertData(
                table: "ComplaintPriorities",
                columns: new[] { "Id", "Code", "Created", "CreatedBy", "IsActive", "IsDeleted", "LastModified", "LastModifiedBy", "Name", "Weight" },
                values: new object[,]
                {
                    { new Guid("4d3816f2-34ad-4c75-b0d0-d4716d1f128a"), "HIGH", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "High", 3 },
                    { new Guid("9ecbc224-3f00-4001-b6fb-9f5acfc36166"), "URG", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Urgent", 4 },
                    { new Guid("a955a3fb-c292-4ec0-872b-4eb6b43d34d7"), "LOW", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Low", 1 },
                    { new Guid("c9f0b605-61e0-4eba-b775-8da932d49cee"), "MED", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Medium", 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ComplaintCategories",
                keyColumn: "Id",
                keyValue: new Guid("1c508462-f2f0-4e5a-a88a-0a2d3f2eb1b8"));

            migrationBuilder.DeleteData(
                table: "ComplaintCategories",
                keyColumn: "Id",
                keyValue: new Guid("1d769d16-6c21-4db7-b3eb-4c321e2bc69a"));

            migrationBuilder.DeleteData(
                table: "ComplaintCategories",
                keyColumn: "Id",
                keyValue: new Guid("2955554f-25d6-4267-b710-f751617c88a0"));

            migrationBuilder.DeleteData(
                table: "ComplaintCategories",
                keyColumn: "Id",
                keyValue: new Guid("43cbb35e-0744-4ed7-a133-3516865883e5"));

            migrationBuilder.DeleteData(
                table: "ComplaintCategories",
                keyColumn: "Id",
                keyValue: new Guid("981e9f52-42f9-44d4-aade-71c6a108eff7"));

            migrationBuilder.DeleteData(
                table: "ComplaintCategories",
                keyColumn: "Id",
                keyValue: new Guid("c149efdd-b3b3-4b72-946b-ee4defc640b5"));

            migrationBuilder.DeleteData(
                table: "ComplaintPriorities",
                keyColumn: "Id",
                keyValue: new Guid("4d3816f2-34ad-4c75-b0d0-d4716d1f128a"));

            migrationBuilder.DeleteData(
                table: "ComplaintPriorities",
                keyColumn: "Id",
                keyValue: new Guid("9ecbc224-3f00-4001-b6fb-9f5acfc36166"));

            migrationBuilder.DeleteData(
                table: "ComplaintPriorities",
                keyColumn: "Id",
                keyValue: new Guid("a955a3fb-c292-4ec0-872b-4eb6b43d34d7"));

            migrationBuilder.DeleteData(
                table: "ComplaintPriorities",
                keyColumn: "Id",
                keyValue: new Guid("c9f0b605-61e0-4eba-b775-8da932d49cee"));

            migrationBuilder.DropColumn(
                name: "AdminRemarks",
                table: "Complaints");

            migrationBuilder.InsertData(
                table: "ComplaintCategories",
                columns: new[] { "Id", "Code", "Created", "CreatedBy", "Description", "IsActive", "IsDeleted", "LastModified", "LastModifiedBy", "Name" },
                values: new object[,]
                {
                    { new Guid("05ec0df1-7142-4549-8040-9b3042d90ab9"), "SECURITY", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Security" },
                    { new Guid("2ce0ae7b-1a10-42ad-823d-0c94e3e99804"), "MAINT", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Maintenance" },
                    { new Guid("73d55319-c4b8-4cb0-935e-59abc2e8e608"), "OTHER", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Other" },
                    { new Guid("a81c3402-ead5-42d8-b961-97ec77bed9c5"), "ROAD", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Road / Infrastructure" },
                    { new Guid("d21db04f-328d-4236-8aa6-b121088cdc96"), "ELECTRIC", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Electricity" },
                    { new Guid("e99b2a7a-bf14-4441-b6cc-ef138ee382a6"), "WATER", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Water Supply" }
                });

            migrationBuilder.InsertData(
                table: "ComplaintPriorities",
                columns: new[] { "Id", "Code", "Created", "CreatedBy", "IsActive", "IsDeleted", "LastModified", "LastModifiedBy", "Name", "Weight" },
                values: new object[,]
                {
                    { new Guid("107eed1b-99be-4816-b92d-360b77a4a4c6"), "MED", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Medium", 2 },
                    { new Guid("284ce9f9-a0e7-483f-9671-04fea3f9d6e0"), "HIGH", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "High", 3 },
                    { new Guid("e4bfb355-1fd9-42c3-ab26-eae15e1d6778"), "URG", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Urgent", 4 },
                    { new Guid("fb7fc901-0c33-4f5d-ab2b-fe9b09b1d522"), "LOW", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Low", 1 }
                });
        }
    }
}
