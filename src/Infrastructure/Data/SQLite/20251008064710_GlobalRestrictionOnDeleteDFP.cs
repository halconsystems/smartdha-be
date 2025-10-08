using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHAFacilitationAPIs.Infrastructure.Data.SQLite
{
    /// <inheritdoc />
    public partial class GlobalRestrictionOnDeleteDFP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppPermissions_SubModules_SubModuleId",
                table: "AppPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_AppRoleModules_AppRoles_RoleId",
                table: "AppRoleModules");

            migrationBuilder.DropForeignKey(
                name: "FK_AppRoleModules_Modules_ModuleId",
                table: "AppRoleModules");

            migrationBuilder.DropForeignKey(
                name: "FK_AppRolePermissions_AppRoles_RoleId",
                table: "AppRolePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_AppRolePermissions_SubModules_SubModuleId",
                table: "AppRolePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUserRoles_AppRoles_RoleId",
                table: "AppUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUserRoles_Users_UserId",
                table: "AppUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberTypeModuleAssignments_Modules_ModuleId",
                table: "MemberTypeModuleAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_NonMemberVerificationDocuments_NonMemberVerifications_VerificationId",
                table: "NonMemberVerificationDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_PanicActionLogs_PanicRequests_PanicRequestId",
                table: "PanicActionLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_PanicLocationUpdates_PanicRequests_PanicRequestId",
                table: "PanicLocationUpdates");

            migrationBuilder.DropForeignKey(
                name: "FK_PanicRequests_EmergencyTypes_EmergencyTypeId",
                table: "PanicRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestProcessSteps_RequestTrackings_RequestTrackingId",
                table: "RequestProcessSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleClaims_Role_RoleId",
                table: "RoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClaims_Users_UserId",
                table: "UserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLogins_Users_UserId",
                table: "UserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMembershipPurposes_MembershipPurposes_PurposeId",
                table: "UserMembershipPurposes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMembershipPurposes_Users_UserId",
                table: "UserMembershipPurposes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserModuleAssignments_Modules_ModuleId",
                table: "UserModuleAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserModuleAssignments_Users_UserId",
                table: "UserModuleAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissionAssignments_AppPermissions_PermissionId",
                table: "UserPermissionAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissionAssignments_Users_UserId",
                table: "UserPermissionAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Role_RoleId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSubModuleAssignments_SubModules_SubModuleId",
                table: "UserSubModuleAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSubModuleAssignments_Users_UserId",
                table: "UserSubModuleAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTokens_Users_UserId",
                table: "UserTokens");

            migrationBuilder.AddForeignKey(
                name: "FK_AppPermissions_SubModules_SubModuleId",
                table: "AppPermissions",
                column: "SubModuleId",
                principalTable: "SubModules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppRoleModules_AppRoles_RoleId",
                table: "AppRoleModules",
                column: "RoleId",
                principalTable: "AppRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppRoleModules_Modules_ModuleId",
                table: "AppRoleModules",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppRolePermissions_AppRoles_RoleId",
                table: "AppRolePermissions",
                column: "RoleId",
                principalTable: "AppRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppRolePermissions_SubModules_SubModuleId",
                table: "AppRolePermissions",
                column: "SubModuleId",
                principalTable: "SubModules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserRoles_AppRoles_RoleId",
                table: "AppUserRoles",
                column: "RoleId",
                principalTable: "AppRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserRoles_Users_UserId",
                table: "AppUserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberTypeModuleAssignments_Modules_ModuleId",
                table: "MemberTypeModuleAssignments",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NonMemberVerificationDocuments_NonMemberVerifications_VerificationId",
                table: "NonMemberVerificationDocuments",
                column: "VerificationId",
                principalTable: "NonMemberVerifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PanicActionLogs_PanicRequests_PanicRequestId",
                table: "PanicActionLogs",
                column: "PanicRequestId",
                principalTable: "PanicRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PanicLocationUpdates_PanicRequests_PanicRequestId",
                table: "PanicLocationUpdates",
                column: "PanicRequestId",
                principalTable: "PanicRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PanicRequests_EmergencyTypes_EmergencyTypeId",
                table: "PanicRequests",
                column: "EmergencyTypeId",
                principalTable: "EmergencyTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestProcessSteps_RequestTrackings_RequestTrackingId",
                table: "RequestProcessSteps",
                column: "RequestTrackingId",
                principalTable: "RequestTrackings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleClaims_Role_RoleId",
                table: "RoleClaims",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClaims_Users_UserId",
                table: "UserClaims",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogins_Users_UserId",
                table: "UserLogins",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMembershipPurposes_MembershipPurposes_PurposeId",
                table: "UserMembershipPurposes",
                column: "PurposeId",
                principalTable: "MembershipPurposes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMembershipPurposes_Users_UserId",
                table: "UserMembershipPurposes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserModuleAssignments_Modules_ModuleId",
                table: "UserModuleAssignments",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserModuleAssignments_Users_UserId",
                table: "UserModuleAssignments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissionAssignments_AppPermissions_PermissionId",
                table: "UserPermissionAssignments",
                column: "PermissionId",
                principalTable: "AppPermissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissionAssignments_Users_UserId",
                table: "UserPermissionAssignments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Role_RoleId",
                table: "UserRoles",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubModuleAssignments_SubModules_SubModuleId",
                table: "UserSubModuleAssignments",
                column: "SubModuleId",
                principalTable: "SubModules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubModuleAssignments_Users_UserId",
                table: "UserSubModuleAssignments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTokens_Users_UserId",
                table: "UserTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppPermissions_SubModules_SubModuleId",
                table: "AppPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_AppRoleModules_AppRoles_RoleId",
                table: "AppRoleModules");

            migrationBuilder.DropForeignKey(
                name: "FK_AppRoleModules_Modules_ModuleId",
                table: "AppRoleModules");

            migrationBuilder.DropForeignKey(
                name: "FK_AppRolePermissions_AppRoles_RoleId",
                table: "AppRolePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_AppRolePermissions_SubModules_SubModuleId",
                table: "AppRolePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUserRoles_AppRoles_RoleId",
                table: "AppUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUserRoles_Users_UserId",
                table: "AppUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberTypeModuleAssignments_Modules_ModuleId",
                table: "MemberTypeModuleAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_NonMemberVerificationDocuments_NonMemberVerifications_VerificationId",
                table: "NonMemberVerificationDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_PanicActionLogs_PanicRequests_PanicRequestId",
                table: "PanicActionLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_PanicLocationUpdates_PanicRequests_PanicRequestId",
                table: "PanicLocationUpdates");

            migrationBuilder.DropForeignKey(
                name: "FK_PanicRequests_EmergencyTypes_EmergencyTypeId",
                table: "PanicRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestProcessSteps_RequestTrackings_RequestTrackingId",
                table: "RequestProcessSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleClaims_Role_RoleId",
                table: "RoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClaims_Users_UserId",
                table: "UserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLogins_Users_UserId",
                table: "UserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMembershipPurposes_MembershipPurposes_PurposeId",
                table: "UserMembershipPurposes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMembershipPurposes_Users_UserId",
                table: "UserMembershipPurposes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserModuleAssignments_Modules_ModuleId",
                table: "UserModuleAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserModuleAssignments_Users_UserId",
                table: "UserModuleAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissionAssignments_AppPermissions_PermissionId",
                table: "UserPermissionAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissionAssignments_Users_UserId",
                table: "UserPermissionAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Role_RoleId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSubModuleAssignments_SubModules_SubModuleId",
                table: "UserSubModuleAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSubModuleAssignments_Users_UserId",
                table: "UserSubModuleAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTokens_Users_UserId",
                table: "UserTokens");

            migrationBuilder.AddForeignKey(
                name: "FK_AppPermissions_SubModules_SubModuleId",
                table: "AppPermissions",
                column: "SubModuleId",
                principalTable: "SubModules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppRoleModules_AppRoles_RoleId",
                table: "AppRoleModules",
                column: "RoleId",
                principalTable: "AppRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppRoleModules_Modules_ModuleId",
                table: "AppRoleModules",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppRolePermissions_AppRoles_RoleId",
                table: "AppRolePermissions",
                column: "RoleId",
                principalTable: "AppRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppRolePermissions_SubModules_SubModuleId",
                table: "AppRolePermissions",
                column: "SubModuleId",
                principalTable: "SubModules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserRoles_AppRoles_RoleId",
                table: "AppUserRoles",
                column: "RoleId",
                principalTable: "AppRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserRoles_Users_UserId",
                table: "AppUserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberTypeModuleAssignments_Modules_ModuleId",
                table: "MemberTypeModuleAssignments",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NonMemberVerificationDocuments_NonMemberVerifications_VerificationId",
                table: "NonMemberVerificationDocuments",
                column: "VerificationId",
                principalTable: "NonMemberVerifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PanicActionLogs_PanicRequests_PanicRequestId",
                table: "PanicActionLogs",
                column: "PanicRequestId",
                principalTable: "PanicRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PanicLocationUpdates_PanicRequests_PanicRequestId",
                table: "PanicLocationUpdates",
                column: "PanicRequestId",
                principalTable: "PanicRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PanicRequests_EmergencyTypes_EmergencyTypeId",
                table: "PanicRequests",
                column: "EmergencyTypeId",
                principalTable: "EmergencyTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestProcessSteps_RequestTrackings_RequestTrackingId",
                table: "RequestProcessSteps",
                column: "RequestTrackingId",
                principalTable: "RequestTrackings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleClaims_Role_RoleId",
                table: "RoleClaims",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClaims_Users_UserId",
                table: "UserClaims",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogins_Users_UserId",
                table: "UserLogins",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMembershipPurposes_MembershipPurposes_PurposeId",
                table: "UserMembershipPurposes",
                column: "PurposeId",
                principalTable: "MembershipPurposes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMembershipPurposes_Users_UserId",
                table: "UserMembershipPurposes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserModuleAssignments_Modules_ModuleId",
                table: "UserModuleAssignments",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserModuleAssignments_Users_UserId",
                table: "UserModuleAssignments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissionAssignments_AppPermissions_PermissionId",
                table: "UserPermissionAssignments",
                column: "PermissionId",
                principalTable: "AppPermissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissionAssignments_Users_UserId",
                table: "UserPermissionAssignments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Role_RoleId",
                table: "UserRoles",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubModuleAssignments_SubModules_SubModuleId",
                table: "UserSubModuleAssignments",
                column: "SubModuleId",
                principalTable: "SubModules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubModuleAssignments_Users_UserId",
                table: "UserSubModuleAssignments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTokens_Users_UserId",
                table: "UserTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
