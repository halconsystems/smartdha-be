using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.FMS;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<UserModuleAssignment> UserModuleAssignments { get; }
    DbSet<ApplicationLog> ApplicationLogs { get; }
    DbSet<Membershipdetail> Membershipdetails { get; }
    DbSet<UserOtp> UserOtps { get; }
    DbSet<SMSLog> SMSLogs { get; }
    DbSet<MembershipPurpose> MembershipPurposes { get; }
    DbSet<NonMemberVerification> NonMemberVerifications { get; }
    DbSet<NonMemberVerificationDocument> NonMemberVerificationDocuments { get; }
    DbSet<MemberTypeModuleAssignment> MemberTypeModuleAssignments { get; }
    DbSet<Announcement> Announcements { get; }
    DbSet<RequestTracking> RequestTrackings { get; }
    DbSet<RequestProcessStep> RequestProcessSteps { get; }
    DbSet<UserMembershipPurpose> UserMembershipPurposes { get; }

    DbSet<RoleAssignment> RoleAssignments { get; }
    //User management and RBAC
    DbSet<Module> Modules { get; }
    DbSet<SubModule> SubModules { get; }
    DbSet<AppRolePermission> AppRolePermissions { get; } // Did not use but when needed i will use late
    DbSet<AppPermission> AppPermissions { get; }
    DbSet<AppUserRole> AppUserRoles { get; }
    DbSet<AppRole> AppRoles { get; }
    DbSet<AppRoleModule> AppRoleModules { get; }
    DbSet<UserSubModuleAssignment> UserSubModuleAssignments { get; }
    DbSet<UserPermissionAssignment> UserPermissionAssignments { get; }
    DbSet<UserClubAssignment> UserClubAssignments { get; }
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<UserActivityLog> UserActivityLogs { get; }
    //Panic Button Module
    DbSet<EmergencyType> EmergencyTypes { get; }
    DbSet<PanicRequest> PanicRequests { get; }
    DbSet<PanicActionLog> PanicActionLogs { get; }
    DbSet<PanicLocationUpdate> PanicLocationUpdates { get; }
    DbSet<PanicResponder> PanicResponders { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Complaint> Complaints { get; }
    DbSet<ComplaintAttachment> ComplaintAttachments { get; }
    DbSet<ComplaintCategory> ComplaintCategories { get; }
    DbSet<ComplaintHistory> ComplaintHistories { get; }
    DbSet<ComplaintPriority> ComplaintPriorities { get; }
    DbSet<Configuration> Configurations { get; }
    DbSet<SmartPayLog> SmartPayLogs { get; }
    DbSet<SvPoint> SvPoints { get; }
    DbSet<SvVehicle> SvVehicles { get; }
    DbSet<PanicDispatch> PanicDispatches { get; }
    DbSet<GoogleApiLog> GoogleApiLogs { get; }
    DbSet<SvVehicleAssignmentHistory> SvVehicleAssignmentHistories { get; }
    DbSet<UserLoginAudit> UserLoginAudits { get; }
    DbSet<FirebaseApiLog> FirebaseApiLogs { get; }
    DbSet<PanicDispatchMedia> PanicDispatchMedias { get; }
    DbSet<PanicReview> PanicReviews { get; }
    DbSet<PaymentIpnLog> PaymentIpnLogs { get; }
    DbSet<WebhookCallbackLog> WebhookCallbackLogs { get; }
    DbSet<MemberShips> MemberShips { get; }
    DbSet<MemberShipCatergories> MemberShipCatergories { get; }
    DbSet<Religion> Religions { get; }
    DbSet<ReligonSect> ReligonSects { get; }
    DbSet<MemberRequest> MemberRequests { get; }
    DbSet<MemberSpouse> MemberSpouses { get; }
    DbSet<MemberChildren> MemberChildrens { get; }
    DbSet<FemPhase> FemPhases { get; }
    DbSet<FemService> FemServices { get; }
    DbSet<TankerSize> TankerSizes { get; }
    DbSet<FemDTSetting> FemDTSettings { get; }
    DbSet<FemPaymentIpnLogs> FemPaymentIpnLogs { get; }
    DbSet<FemgutionShops> FemgutionShops { get; }
    DbSet<Fumigation> Fumigations { get; }
    DbSet<FumgationMedia> FumgationMedias { get; }
    DbSet<UserDevices> UserDevices { get; }


    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    DatabaseFacade Database { get; }

}
