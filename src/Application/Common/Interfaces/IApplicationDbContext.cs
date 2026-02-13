using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<UserModuleAssignment> UserModuleAssignments { get; }
    DbSet<ApplicationLog> ApplicationLogs { get; }
    DbSet<UserOtp> UserOtps { get; }
    DbSet<MembershipPurpose> MembershipPurposes { get; }
    DbSet<NonMemberVerification> NonMemberVerifications { get; }
    DbSet<UserMembershipPurpose> UserMembershipPurposes { get; }
    //User management and RBAC
    DbSet<Module> Modules { get; }
    DbSet<SubModule> SubModules { get; }
    DbSet<AppRolePermission> AppRolePermissions { get; } // Did not use but when needed i will use late
    DbSet<AppPermission> AppPermissions { get; }
    DbSet<AppUserRole> AppUserRoles { get; }
    DbSet<AppRole> AppRoles { get; }
    DbSet<UserSubModuleAssignment> UserSubModuleAssignments { get; }
    DbSet<UserPermissionAssignment> UserPermissionAssignments { get; }
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<UserActivityLog> UserActivityLogs { get; }
    //Panic Button Module
    DbSet<UserLoginAudit> UserLoginAudits { get; }
    DbSet<WebhookCallbackLog> WebhookCallbackLogs { get; }
    DbSet<UserDevices> UserDevices { get; }
    DbSet<UserDeleteRequest> UserDeleteRequests { get; }
    DbSet<UserImages> UserImages { get; }
    DbSet<UserMemberProfile> UserMemberProfiles { get; }




    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    DatabaseFacade Database { get; }

}
