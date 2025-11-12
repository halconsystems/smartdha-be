using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DHAFacilitationAPIs.Infrastructure.Data.SQLite
{
    /// <inheritdoc />
    public partial class AddComplaintModuleChanegs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComplaintAttachments_Complaints_ComplaintId1",
                table: "ComplaintAttachments");

            migrationBuilder.DropIndex(
                name: "IX_ComplaintAttachments_ComplaintId1",
                table: "ComplaintAttachments");

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
                keyValue: new Guid("c2d252f8-def9-423a-88e7-c14ea6eb7263"));

            migrationBuilder.DeleteData(
                table: "ComplaintCategories",
                keyColumn: "Id",
                keyValue: new Guid("f4bc1909-4a6c-4557-a543-0f088004ff45"));

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

            migrationBuilder.DropColumn(
                name: "ComplaintId1",
                table: "ComplaintAttachments");

            migrationBuilder.AlterColumn<Guid>(
                name: "ComplaintId",
                table: "ComplaintAttachments",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "ComplaintCategories",
                columns: new[] { "Id", "Code", "Created", "CreatedBy", "Description", "IsActive", "IsDeleted", "LastModified", "LastModifiedBy", "Name" },
                values: new object[,]
                {
                    { new Guid("56cca0d1-8570-41cf-a49a-32243d7056aa"), "ROAD", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Road / Infrastructure" },
                    { new Guid("b8a19827-2734-4080-a773-2fc8fb773515"), "OTHER", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Other" },
                    { new Guid("c0b52dcd-1688-4ef8-9df3-228b2738ba03"), "WATER", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Water Supply" },
                    { new Guid("c82e2d7b-4d16-4657-aabf-03dd117c28fb"), "ELECTRIC", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Electricity" },
                    { new Guid("df30b37b-78e5-44bc-a5d1-aa630a74b285"), "SECURITY", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Security" },
                    { new Guid("e5844ea0-f0e1-4d52-87ec-8fcda461aaff"), "MAINT", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Maintenance" }
                });

            migrationBuilder.InsertData(
                table: "ComplaintPriorities",
                columns: new[] { "Id", "Code", "Created", "CreatedBy", "IsActive", "IsDeleted", "LastModified", "LastModifiedBy", "Name", "Weight" },
                values: new object[,]
                {
                    { new Guid("7bf66a00-1a9c-4002-81c1-7fe0e1a426c9"), "MED", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Medium", 2 },
                    { new Guid("9c09c2cc-4544-4690-80c0-01d230653491"), "URG", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Urgent", 4 },
                    { new Guid("f4934f1c-685f-4d49-a8e4-beaa59d74ac0"), "HIGH", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "High", 3 },
                    { new Guid("f5b114c9-e375-473b-a150-d2de529d1385"), "LOW", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Low", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComplaintAttachments_ComplaintId",
                table: "ComplaintAttachments",
                column: "ComplaintId");

            migrationBuilder.AddForeignKey(
                name: "FK_ComplaintAttachments_Complaints_ComplaintId",
                table: "ComplaintAttachments",
                column: "ComplaintId",
                principalTable: "Complaints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComplaintAttachments_Complaints_ComplaintId",
                table: "ComplaintAttachments");

            migrationBuilder.DropIndex(
                name: "IX_ComplaintAttachments_ComplaintId",
                table: "ComplaintAttachments");

            migrationBuilder.DeleteData(
                table: "ComplaintCategories",
                keyColumn: "Id",
                keyValue: new Guid("56cca0d1-8570-41cf-a49a-32243d7056aa"));

            migrationBuilder.DeleteData(
                table: "ComplaintCategories",
                keyColumn: "Id",
                keyValue: new Guid("b8a19827-2734-4080-a773-2fc8fb773515"));

            migrationBuilder.DeleteData(
                table: "ComplaintCategories",
                keyColumn: "Id",
                keyValue: new Guid("c0b52dcd-1688-4ef8-9df3-228b2738ba03"));

            migrationBuilder.DeleteData(
                table: "ComplaintCategories",
                keyColumn: "Id",
                keyValue: new Guid("c82e2d7b-4d16-4657-aabf-03dd117c28fb"));

            migrationBuilder.DeleteData(
                table: "ComplaintCategories",
                keyColumn: "Id",
                keyValue: new Guid("df30b37b-78e5-44bc-a5d1-aa630a74b285"));

            migrationBuilder.DeleteData(
                table: "ComplaintCategories",
                keyColumn: "Id",
                keyValue: new Guid("e5844ea0-f0e1-4d52-87ec-8fcda461aaff"));

            migrationBuilder.DeleteData(
                table: "ComplaintPriorities",
                keyColumn: "Id",
                keyValue: new Guid("7bf66a00-1a9c-4002-81c1-7fe0e1a426c9"));

            migrationBuilder.DeleteData(
                table: "ComplaintPriorities",
                keyColumn: "Id",
                keyValue: new Guid("9c09c2cc-4544-4690-80c0-01d230653491"));

            migrationBuilder.DeleteData(
                table: "ComplaintPriorities",
                keyColumn: "Id",
                keyValue: new Guid("f4934f1c-685f-4d49-a8e4-beaa59d74ac0"));

            migrationBuilder.DeleteData(
                table: "ComplaintPriorities",
                keyColumn: "Id",
                keyValue: new Guid("f5b114c9-e375-473b-a150-d2de529d1385"));

            migrationBuilder.AlterColumn<string>(
                name: "ComplaintId",
                table: "ComplaintAttachments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ComplaintId1",
                table: "ComplaintAttachments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.InsertData(
                table: "ComplaintCategories",
                columns: new[] { "Id", "Code", "Created", "CreatedBy", "Description", "IsActive", "IsDeleted", "LastModified", "LastModifiedBy", "Name" },
                values: new object[,]
                {
                    { new Guid("150ba363-45ef-4f19-b03f-57876cc26562"), "ROAD", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Road / Infrastructure" },
                    { new Guid("56214b01-8749-480d-b7a9-1d82a52d2003"), "SECURITY", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Security" },
                    { new Guid("600a3d2a-b165-4b62-a254-d7592004ffc9"), "MAINT", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Maintenance" },
                    { new Guid("a767a331-729b-4b49-95be-1798f154f855"), "ELECTRIC", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Electricity" },
                    { new Guid("c2d252f8-def9-423a-88e7-c14ea6eb7263"), "OTHER", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Other" },
                    { new Guid("f4bc1909-4a6c-4557-a543-0f088004ff45"), "WATER", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Water Supply" }
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

            migrationBuilder.CreateIndex(
                name: "IX_ComplaintAttachments_ComplaintId1",
                table: "ComplaintAttachments",
                column: "ComplaintId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ComplaintAttachments_Complaints_ComplaintId1",
                table: "ComplaintAttachments",
                column: "ComplaintId1",
                principalTable: "Complaints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
