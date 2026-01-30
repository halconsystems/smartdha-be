//using System;
//using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

//namespace DHAFacilitationAPIs.Infrastructure.Data.SQLite
//{
//    /// <inheritdoc />
//    public partial class updatenonmemberverification : Migration
//    {
//        /// <inheritdoc />
//        protected override void Up(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.AlterColumn<string>(
//                name: "UtilityBill",
//                table: "NonMemberVerificationDocuments",
//                type: "nvarchar(500)",
//                maxLength: 500,
//                nullable: true,
//                oldClrType: typeof(string),
//                oldType: "nvarchar(500)",
//                oldMaxLength: 500);

//            migrationBuilder.AlterColumn<string>(
//                name: "ProfilePicture",
//                table: "NonMemberVerificationDocuments",
//                type: "nvarchar(500)",
//                maxLength: 500,
//                nullable: true,
//                oldClrType: typeof(string),
//                oldType: "nvarchar(500)",
//                oldMaxLength: 500);

//            migrationBuilder.AlterColumn<string>(
//                name: "CNICFrontImagePath",
//                table: "NonMemberVerificationDocuments",
//                type: "nvarchar(500)",
//                maxLength: 500,
//                nullable: true,
//                oldClrType: typeof(string),
//                oldType: "nvarchar(500)",
//                oldMaxLength: 500);

//            migrationBuilder.AlterColumn<string>(
//                name: "CNICBackImagePath",
//                table: "NonMemberVerificationDocuments",
//                type: "nvarchar(500)",
//                maxLength: 500,
//                nullable: true,
//                oldClrType: typeof(string),
//                oldType: "nvarchar(500)",
//                oldMaxLength: 500);

//            migrationBuilder.AddColumn<Guid>(
//                name: "RoleId",
//                table: "Directorate",
//                type: "uniqueidentifier",
//                nullable: false,
//                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

//            migrationBuilder.CreateIndex(
//                name: "IX_CaseStepHistory_DirectorateId",
//                table: "CaseStepHistory",
//                column: "DirectorateId");

//            migrationBuilder.AddForeignKey(
//                name: "FK_CaseStepHistory_Directorate_DirectorateId",
//                table: "CaseStepHistory",
//                column: "DirectorateId",
//                principalTable: "Directorate",
//                principalColumn: "Id",
//                onDelete: ReferentialAction.Restrict);
//        }

//        /// <inheritdoc />
//        protected override void Down(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.DropForeignKey(
//                name: "FK_CaseStepHistory_Directorate_DirectorateId",
//                table: "CaseStepHistory");

//            migrationBuilder.DropIndex(
//                name: "IX_CaseStepHistory_DirectorateId",
//                table: "CaseStepHistory");

//            migrationBuilder.DropColumn(
//                name: "RoleId",
//                table: "Directorate");

//            migrationBuilder.AlterColumn<string>(
//                name: "UtilityBill",
//                table: "NonMemberVerificationDocuments",
//                type: "nvarchar(500)",
//                maxLength: 500,
//                nullable: false,
//                defaultValue: "",
//                oldClrType: typeof(string),
//                oldType: "nvarchar(500)",
//                oldMaxLength: 500,
//                oldNullable: true);

//            migrationBuilder.AlterColumn<string>(
//                name: "ProfilePicture",
//                table: "NonMemberVerificationDocuments",
//                type: "nvarchar(500)",
//                maxLength: 500,
//                nullable: false,
//                defaultValue: "",
//                oldClrType: typeof(string),
//                oldType: "nvarchar(500)",
//                oldMaxLength: 500,
//                oldNullable: true);

//            migrationBuilder.AlterColumn<string>(
//                name: "CNICFrontImagePath",
//                table: "NonMemberVerificationDocuments",
//                type: "nvarchar(500)",
//                maxLength: 500,
//                nullable: false,
//                defaultValue: "",
//                oldClrType: typeof(string),
//                oldType: "nvarchar(500)",
//                oldMaxLength: 500,
//                oldNullable: true);

//            migrationBuilder.AlterColumn<string>(
//                name: "CNICBackImagePath",
//                table: "NonMemberVerificationDocuments",
//                type: "nvarchar(500)",
//                maxLength: 500,
//                nullable: false,
//                defaultValue: "",
//                oldClrType: typeof(string),
//                oldType: "nvarchar(500)",
//                oldMaxLength: 500,
//                oldNullable: true);
//        }
//    }
//}
