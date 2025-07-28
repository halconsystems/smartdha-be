using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Module> Modules { get; }
    DbSet<SubModule> SubModules { get; }
    DbSet<UserModuleAssignment> UserModuleAssignments { get; }
    DbSet<ApplicationLog> ApplicationLogs { get; }
    DbSet<RolePermission> RolePermissions { get; }
    DbSet<RoleAssignment> RoleAssignments { get; }
    DbSet<Membershipdetail> Membershipdetails { get; }
    DbSet<UserOtp> UserOtps { get; }
    DbSet<SMSLog> SMSLogs { get; }
    DbSet<MembershipPurpose> MembershipPurposes { get; }
    DbSet<NonMemberVerification> NonMemberVerifications { get; }
    DbSet<NonMemberVerificationDocument> NonMemberVerificationDocuments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    DatabaseFacade Database { get; }
}
