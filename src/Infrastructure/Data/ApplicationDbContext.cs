using System.Reflection;
using System.Reflection.Emit;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Common;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ApplicationUser = DHAFacilitationAPIs.Domain.Entities.ApplicationUser;

namespace DHAFacilitationAPIs.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    private readonly IUser _loggedInUser;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IUser loggedInUser) : base(options)
    {
        _loggedInUser = loggedInUser;
    }

    public DbSet<Domain.Entities.Module> Modules => Set<Domain.Entities.Module>();
    public DbSet<SubModule> SubModules => Set<SubModule>();
    public DbSet<UserModuleAssignment> UserModuleAssignments => Set<UserModuleAssignment>();
    public DbSet<ApplicationLog> ApplicationLogs => Set<ApplicationLog>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RoleAssignment> RoleAssignments => Set<RoleAssignment>();
    public DbSet<Membershipdetail> Membershipdetails => Set<Membershipdetail>();
    public DbSet<SMSLog> SMSLogs => Set<SMSLog>();
    public DbSet<UserOtp> UserOtps => Set<UserOtp>();
    public DbSet<MembershipPurpose> MembershipPurposes => Set<MembershipPurpose>();
    public DbSet<NonMemberVerification> NonMemberVerifications => Set<NonMemberVerification>();
    public DbSet<NonMemberVerificationDocument> NonMemberVerificationDocuments => Set<NonMemberVerificationDocument>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        IdentityBuilder(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private static void IdentityBuilder(ModelBuilder builder)
    {
        builder.Entity<ApplicationUser>(
            entity =>
            {
                entity.ToTable("Users");
                entity.Property(e => e.Id).HasMaxLength(85);
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.NormalizedEmail).HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.NormalizedUserName).HasMaxLength(100);
            });

        builder.Entity<IdentityRole>(
            entity =>
            {
                entity.ToTable("Role");
                entity.Property(e => e.Id).HasMaxLength(85);
                entity.Property(e => e.NormalizedName).HasMaxLength(100);
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.ConcurrencyStamp).HasMaxLength(85);
            });

        builder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.ToTable("UserRoles");
            entity.Property(e => e.UserId).HasMaxLength(85);
            entity.Property(e => e.RoleId).HasMaxLength(85);
        });

        builder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.ToTable("UserClaims");
            entity.Property(e => e.Id).HasMaxLength(85);
            entity.Property(e => e.UserId).HasMaxLength(85);
        });

        builder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.ToTable("UserLogins");
            entity.Property(e => e.LoginProvider).HasMaxLength(85);
            entity.Property(e => e.ProviderKey).HasMaxLength(85);
            entity.Property(e => e.UserId).HasMaxLength(85);
        });

        builder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.ToTable("UserTokens").HasKey(e => new { e.UserId, e.LoginProvider });
            entity.Property(e => e.UserId).HasMaxLength(85);
        });

        builder.Entity<IdentityRoleClaim<string>>(entity =>
        {
            entity.ToTable("RoleClaims");
            entity.Property(e => e.Id).HasMaxLength(85);
            entity.Property(e => e.RoleId).HasMaxLength(85);
        });

        builder.Entity<SubModule>()
        .HasOne(s => s.Module)
        .WithMany(m => m.SubModules)
        .HasForeignKey(s => s.ModuleId);

    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        // Define the Pakistan Standard Time zone
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
        var pakistanTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<BaseAuditableEntity> entry in ChangeTracker.Entries<BaseAuditableEntity>())
        {

            string entityName = entry.Metadata.ClrType.Name;

            if (entityName == "UserRefreshToken")
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.Created = pakistanTime;
                        entry.Entity.CreatedBy = _loggedInUser?.Id;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModified = DateTime.Now;
                        entry.Entity.LastModifiedBy = _loggedInUser?.Id;
                        break;
                }
            }
            else
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.Created = pakistanTime;
                        entry.Entity.CreatedBy = _loggedInUser?.Id;
                        entry.Entity.IsActive = true;
                        entry.Entity.IsDeleted = false;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModified = pakistanTime;
                        entry.Entity.LastModifiedBy = _loggedInUser?.Id;
                        break;
                }
            }
            // Handle ApplicationLog entries
            foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<ApplicationLog> transactionEntry in ChangeTracker.Entries<ApplicationLog>().Where(e => e.State == EntityState.Added))
            {
                transactionEntry.Entity.CreatedDateTime = pakistanTime;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
