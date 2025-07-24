using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Module> Modules { get; }
    DbSet<SubModule> SubModules { get; }
    DbSet<UserModuleAssignment> UserModuleAssignments { get; }
    DbSet<ApplicationLog> ApplicationLogs { get; }
    DbSet<RolePermission> RolePermissions { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
