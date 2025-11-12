using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DHAFacilitationAPIs.Infrastructure.Data.SQLite
{
    /// <inheritdoc />
    public partial class UpdateComplaintTablesFixed2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ////migrationBuilder.DropForeignKey(
            ////    name: "FK_ComplaintAttachments_Complaints_ComplaintId1",
            ////    table: "ComplaintAttachments");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_ComplaintComment_Complaints_ComplaintId1",
            //    table: "ComplaintComment");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_ComplaintHistories_Complaints_ComplaintId1",
            //    table: "ComplaintHistories");

            //migrationBuilder.DropIndex(
            //    name: "IX_ComplaintHistories_ComplaintId1",
            //    table: "ComplaintHistories");

            //migrationBuilder.DropIndex(
            //    name: "IX_ComplaintComment_ComplaintId1",
            //    table: "ComplaintComment");

            ////migrationBuilder.DropIndex(
            ////    name: "IX_ComplaintAttachments_ComplaintId1",
            ////    table: "ComplaintAttachments");

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

            //migrationBuilder.DropColumn(
            //    name: "ComplaintId1",
            //    table: "ComplaintHistories");

            //migrationBuilder.DropColumn(
            //    name: "ComplaintId1",
            //    table: "ComplaintComment");

            ////migrationBuilder.DropColumn(
            ////    name: "ComplaintId1",
            ////    table: "ComplaintAttachments");

            migrationBuilder.DropColumn(
                name: "ComplaintId",
                table: "ComplaintHistories");

            migrationBuilder.AddColumn<Guid>(
                name: "ComplaintId",
                table: "ComplaintHistories",
                type: "uniqueidentifier",
                nullable: false);

            migrationBuilder.DropForeignKey(
                name: "FK_ComplaintComment_Complaints_ComplaintId",
                table: "ComplaintComment");

            migrationBuilder.DropColumn(
               name: "ComplaintId",
               table: "ComplaintComment");

            migrationBuilder.AddColumn<Guid>(
                name: "ComplaintId",
                table: "ComplaintComment",
                type: "uniqueidentifier",
                nullable: false);

            //migrationBuilder.AlterColumn<Guid>(
            //    name: "ComplaintId",
            //    table: "ComplaintHistories",
            //    type: "uniqueidentifier",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            //migrationBuilder.AlterColumn<Guid>(
            //    name: "ComplaintId",
            //    table: "ComplaintComment",
            //    type: "uniqueidentifier",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "ComplaintId",
                table: "ComplaintAttachments",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            //migrationBuilder.CreateTable(
            //    name: "PanicResponders",
            //    columns: table => new
            //    {
            //        Ser = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //        CNIC = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
            //        PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
            //        Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //        Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
            //        EmergencyTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        Created = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: true),
            //        IsActive = table.Column<bool>(type: "bit", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_PanicResponders", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_PanicResponders_EmergencyTypes_EmergencyTypeId",
            //            column: x => x.EmergencyTypeId,
            //            principalTable: "EmergencyTypes",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.InsertData(
            //    table: "ComplaintCategories",
            //    columns: new[] { "Id", "Code", "Created", "CreatedBy", "Description", "IsActive", "IsDeleted", "LastModified", "LastModifiedBy", "Name" },
            //    values: new object[,]
            //    {
            //        { new Guid("05ec0df1-7142-4549-8040-9b3042d90ab9"), "SECURITY", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Security" },
            //        { new Guid("2ce0ae7b-1a10-42ad-823d-0c94e3e99804"), "MAINT", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Maintenance" },
            //        { new Guid("73d55319-c4b8-4cb0-935e-59abc2e8e608"), "OTHER", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Other" },
            //        { new Guid("a81c3402-ead5-42d8-b961-97ec77bed9c5"), "ROAD", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Road / Infrastructure" },
            //        { new Guid("d21db04f-328d-4236-8aa6-b121088cdc96"), "ELECTRIC", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Electricity" },
            //        { new Guid("e99b2a7a-bf14-4441-b6cc-ef138ee382a6"), "WATER", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Water Supply" }
            //    });

            //migrationBuilder.InsertData(
            //    table: "ComplaintPriorities",
            //    columns: new[] { "Id", "Code", "Created", "CreatedBy", "IsActive", "IsDeleted", "LastModified", "LastModifiedBy", "Name", "Weight" },
            //    values: new object[,]
            //    {
            //        { new Guid("107eed1b-99be-4816-b92d-360b77a4a4c6"), "MED", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Medium", 2 },
            //        { new Guid("284ce9f9-a0e7-483f-9671-04fea3f9d6e0"), "HIGH", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "High", 3 },
            //        { new Guid("e4bfb355-1fd9-42c3-ab26-eae15e1d6778"), "URG", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Urgent", 4 },
            //        { new Guid("fb7fc901-0c33-4f5d-ab2b-fe9b09b1d522"), "LOW", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Low", 1 }
            //    });

            migrationBuilder.CreateIndex(
                name: "IX_ComplaintHistories_ComplaintId",
                table: "ComplaintHistories",
                column: "ComplaintId");

            migrationBuilder.CreateIndex(
                name: "IX_ComplaintComment_ComplaintId",
                table: "ComplaintComment",
                column: "ComplaintId");

            migrationBuilder.CreateIndex(
                name: "IX_ComplaintAttachments_ComplaintId",
                table: "ComplaintAttachments",
                column: "ComplaintId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_PanicResponders_EmergencyTypeId",
            //    table: "PanicResponders",
            //    column: "EmergencyTypeId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_ComplaintAttachments_Complaints_ComplaintId",
            //    table: "ComplaintAttachments",
            //    column: "ComplaintId",
            //    principalTable: "Complaints",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ComplaintComment_Complaints_ComplaintId",
                table: "ComplaintComment",
                column: "ComplaintId",
                principalTable: "Complaints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ComplaintHistories_Complaints_ComplaintId",
                table: "ComplaintHistories",
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

            migrationBuilder.DropForeignKey(
                name: "FK_ComplaintComment_Complaints_ComplaintId",
                table: "ComplaintComment");

            migrationBuilder.DropForeignKey(
                name: "FK_ComplaintHistories_Complaints_ComplaintId",
                table: "ComplaintHistories");

            //migrationBuilder.DropTable(
            //    name: "PanicResponders");

            migrationBuilder.DropIndex(
                name: "IX_ComplaintHistories_ComplaintId",
                table: "ComplaintHistories");

            migrationBuilder.DropIndex(
                name: "IX_ComplaintComment_ComplaintId",
                table: "ComplaintComment");

            migrationBuilder.DropIndex(
                name: "IX_ComplaintAttachments_ComplaintId",
                table: "ComplaintAttachments");

            //migrationBuilder.DeleteData(
            //    table: "ComplaintCategories",
            //    keyColumn: "Id",
            //    keyValue: new Guid("05ec0df1-7142-4549-8040-9b3042d90ab9"));

            //migrationBuilder.DeleteData(
            //    table: "ComplaintCategories",
            //    keyColumn: "Id",
            //    keyValue: new Guid("2ce0ae7b-1a10-42ad-823d-0c94e3e99804"));

            //migrationBuilder.DeleteData(
            //    table: "ComplaintCategories",
            //    keyColumn: "Id",
            //    keyValue: new Guid("73d55319-c4b8-4cb0-935e-59abc2e8e608"));

            //migrationBuilder.DeleteData(
            //    table: "ComplaintCategories",
            //    keyColumn: "Id",
            //    keyValue: new Guid("a81c3402-ead5-42d8-b961-97ec77bed9c5"));

            //migrationBuilder.DeleteData(
            //    table: "ComplaintCategories",
            //    keyColumn: "Id",
            //    keyValue: new Guid("d21db04f-328d-4236-8aa6-b121088cdc96"));

            //migrationBuilder.DeleteData(
            //    table: "ComplaintCategories",
            //    keyColumn: "Id",
            //    keyValue: new Guid("e99b2a7a-bf14-4441-b6cc-ef138ee382a6"));

            //migrationBuilder.DeleteData(
            //    table: "ComplaintPriorities",
            //    keyColumn: "Id",
            //    keyValue: new Guid("107eed1b-99be-4816-b92d-360b77a4a4c6"));

            //migrationBuilder.DeleteData(
            //    table: "ComplaintPriorities",
            //    keyColumn: "Id",
            //    keyValue: new Guid("284ce9f9-a0e7-483f-9671-04fea3f9d6e0"));

            //migrationBuilder.DeleteData(
            //    table: "ComplaintPriorities",
            //    keyColumn: "Id",
            //    keyValue: new Guid("e4bfb355-1fd9-42c3-ab26-eae15e1d6778"));

            //migrationBuilder.DeleteData(
            //    table: "ComplaintPriorities",
            //    keyColumn: "Id",
            //    keyValue: new Guid("fb7fc901-0c33-4f5d-ab2b-fe9b09b1d522"));

            migrationBuilder.AlterColumn<int>(
                name: "ComplaintId",
                table: "ComplaintHistories",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ComplaintId1",
                table: "ComplaintHistories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<int>(
                name: "ComplaintId",
                table: "ComplaintComment",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ComplaintId1",
                table: "ComplaintComment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

            //migrationBuilder.InsertData(
            //    table: "ComplaintCategories",
            //    columns: new[] { "Id", "Code", "Created", "CreatedBy", "Description", "IsActive", "IsDeleted", "LastModified", "LastModifiedBy", "Name" },
            //    values: new object[,]
            //    {
            //        { new Guid("150ba363-45ef-4f19-b03f-57876cc26562"), "ROAD", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Road / Infrastructure" },
            //        { new Guid("56214b01-8749-480d-b7a9-1d82a52d2003"), "SECURITY", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Security" },
            //        { new Guid("600a3d2a-b165-4b62-a254-d7592004ffc9"), "MAINT", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Maintenance" },
            //        { new Guid("a767a331-729b-4b49-95be-1798f154f855"), "ELECTRIC", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Electricity" },
            //        { new Guid("c2d252f8-def9-423a-88e7-c14ea6eb7263"), "OTHER", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Other" },
            //        { new Guid("f4bc1909-4a6c-4557-a543-0f088004ff45"), "WATER", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Water Supply" }
            //    });

            //migrationBuilder.InsertData(
            //    table: "ComplaintPriorities",
            //    columns: new[] { "Id", "Code", "Created", "CreatedBy", "IsActive", "IsDeleted", "LastModified", "LastModifiedBy", "Name", "Weight" },
            //    values: new object[,]
            //    {
            //        { new Guid("376b5c7d-282f-4a64-83c0-f665c30d8c2e"), "MED", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Medium", 2 },
            //        { new Guid("bdee021c-5c6b-419e-9be5-eb0ec3cb8fc7"), "URG", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Urgent", 4 },
            //        { new Guid("d96ae061-6c92-4ede-947d-94df256394fd"), "HIGH", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "High", 3 },
            //        { new Guid("f0b28ca3-6fb0-417f-9a68-ea7dea040b4a"), "LOW", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Low", 1 }
            //    });

            migrationBuilder.CreateIndex(
                name: "IX_ComplaintHistories_ComplaintId1",
                table: "ComplaintHistories",
                column: "ComplaintId1");

            migrationBuilder.CreateIndex(
                name: "IX_ComplaintComment_ComplaintId1",
                table: "ComplaintComment",
                column: "ComplaintId1");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ComplaintComment_Complaints_ComplaintId1",
                table: "ComplaintComment",
                column: "ComplaintId1",
                principalTable: "Complaints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ComplaintHistories_Complaints_ComplaintId1",
                table: "ComplaintHistories",
                column: "ComplaintId1",
                principalTable: "Complaints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
