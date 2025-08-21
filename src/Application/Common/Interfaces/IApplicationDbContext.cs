using DHAFacilitationAPIs.Domain.Entities;
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
    DbSet<UserPermission> UserPermissions { get; }

    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    DatabaseFacade Database { get; }
}
