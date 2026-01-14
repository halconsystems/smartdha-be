using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Migrations.PMSApplicationDb
{
    /// <inheritdoc />
    public partial class CompletePMSChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrerequisiteDefinitions",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    MinLength = table.Column<int>(type: "int", nullable: true),
                    MaxLength = table.Column<int>(type: "int", nullable: true),
                    AllowedExtensions = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrerequisiteDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Properties",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Sector = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phase = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OwnerCnic = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCategories",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceProcesses",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IsFeeAtSubmission = table.Column<bool>(type: "bit", nullable: false),
                    IsVoucherPossible = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceProcesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceProcesses_ServiceCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ServiceCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcessPrerequisites",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProcessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrerequisiteDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    RequiredByStepNo = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessPrerequisites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessPrerequisites_PrerequisiteDefinitions_PrerequisiteDefinitionId",
                        column: x => x.PrerequisiteDefinitionId,
                        principalTable: "PrerequisiteDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProcessPrerequisites_ServiceProcesses_ProcessId",
                        column: x => x.ProcessId,
                        principalTable: "ServiceProcesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcessSteps",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProcessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StepNo = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DirectorateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequiresVoucher = table.Column<bool>(type: "bit", nullable: false),
                    RequiresPaymentBeforeNext = table.Column<bool>(type: "bit", nullable: false),
                    SlaHours = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessSteps_Directorates_DirectorateId",
                        column: x => x.DirectorateId,
                        principalTable: "Directorates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProcessSteps_ServiceProcesses_ProcessId",
                        column: x => x.ProcessId,
                        principalTable: "ServiceProcesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyCases",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserPropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProcessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CurrentStepNo = table.Column<int>(type: "int", nullable: false),
                    CurrentStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CaseNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ApplicantName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ApplicantCnic = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ApplicantMobile = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ApplicantRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyCases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyCases_ProcessSteps_CurrentStepId",
                        column: x => x.CurrentStepId,
                        principalTable: "ProcessSteps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PropertyCases_Properties_UserPropertyId",
                        column: x => x.UserPropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropertyCases_ServiceProcesses_ProcessId",
                        column: x => x.ProcessId,
                        principalTable: "ServiceProcesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CaseDocuments",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrerequisiteDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseDocuments_PrerequisiteDefinitions_PrerequisiteDefinitionId",
                        column: x => x.PrerequisiteDefinitionId,
                        principalTable: "PrerequisiteDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CaseDocuments_PropertyCases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "PropertyCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CasePrerequisiteValues",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrerequisiteDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ValueText = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ValueNumber = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ValueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValueBool = table.Column<bool>(type: "bit", nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerificationRef = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CasePrerequisiteValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CasePrerequisiteValues_PrerequisiteDefinitions_PrerequisiteDefinitionId",
                        column: x => x.PrerequisiteDefinitionId,
                        principalTable: "PrerequisiteDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CasePrerequisiteValues_PropertyCases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "PropertyCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CaseStepHistories",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StepId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    PerformedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NextStepNo = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseStepHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseStepHistories_ProcessSteps_StepId",
                        column: x => x.StepId,
                        principalTable: "ProcessSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CaseStepHistories_PropertyCases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "PropertyCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CaseVouchers",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    VoucherNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GeneratedAtStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseVouchers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseVouchers_ProcessSteps_GeneratedAtStepId",
                        column: x => x.GeneratedAtStepId,
                        principalTable: "ProcessSteps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CaseVouchers_PropertyCases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "PropertyCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CasePayments",
                columns: table => new
                {
                    Ser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VoucherId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Method = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GatewayResponse = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CollectedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CasePayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CasePayments_CaseVouchers_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "CaseVouchers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CasePayments_PropertyCases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "PropertyCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Directorates_Code",
                table: "Directorates",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CaseDocuments_CaseId",
                table: "CaseDocuments",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseDocuments_PrerequisiteDefinitionId",
                table: "CaseDocuments",
                column: "PrerequisiteDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_CasePayments_CaseId",
                table: "CasePayments",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CasePayments_VoucherId",
                table: "CasePayments",
                column: "VoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_CasePrerequisiteValues_CaseId_PrerequisiteDefinitionId",
                table: "CasePrerequisiteValues",
                columns: new[] { "CaseId", "PrerequisiteDefinitionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CasePrerequisiteValues_PrerequisiteDefinitionId",
                table: "CasePrerequisiteValues",
                column: "PrerequisiteDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseStepHistories_CaseId",
                table: "CaseStepHistories",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseStepHistories_StepId",
                table: "CaseStepHistories",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseVouchers_CaseId",
                table: "CaseVouchers",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseVouchers_GeneratedAtStepId",
                table: "CaseVouchers",
                column: "GeneratedAtStepId");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteDefinitions_Code",
                table: "PrerequisiteDefinitions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessPrerequisites_PrerequisiteDefinitionId",
                table: "ProcessPrerequisites",
                column: "PrerequisiteDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessPrerequisites_ProcessId_PrerequisiteDefinitionId",
                table: "ProcessPrerequisites",
                columns: new[] { "ProcessId", "PrerequisiteDefinitionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessSteps_DirectorateId",
                table: "ProcessSteps",
                column: "DirectorateId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessSteps_ProcessId_StepNo",
                table: "ProcessSteps",
                columns: new[] { "ProcessId", "StepNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PropertyCases_CaseNo",
                table: "PropertyCases",
                column: "CaseNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PropertyCases_CurrentStepId",
                table: "PropertyCases",
                column: "CurrentStepId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyCases_ProcessId",
                table: "PropertyCases",
                column: "ProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyCases_UserPropertyId",
                table: "PropertyCases",
                column: "UserPropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCategories_Code",
                table: "ServiceCategories",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProcesses_CategoryId_Code",
                table: "ServiceProcesses",
                columns: new[] { "CategoryId", "Code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaseDocuments");

            migrationBuilder.DropTable(
                name: "CasePayments");

            migrationBuilder.DropTable(
                name: "CasePrerequisiteValues");

            migrationBuilder.DropTable(
                name: "CaseStepHistories");

            migrationBuilder.DropTable(
                name: "ProcessPrerequisites");

            migrationBuilder.DropTable(
                name: "CaseVouchers");

            migrationBuilder.DropTable(
                name: "PrerequisiteDefinitions");

            migrationBuilder.DropTable(
                name: "PropertyCases");

            migrationBuilder.DropTable(
                name: "ProcessSteps");

            migrationBuilder.DropTable(
                name: "Properties");

            migrationBuilder.DropTable(
                name: "ServiceProcesses");

            migrationBuilder.DropTable(
                name: "ServiceCategories");

            migrationBuilder.DropIndex(
                name: "IX_Directorates_Code",
                table: "Directorates");
        }
    }
}
