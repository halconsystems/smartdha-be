using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Common;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
    public DbSet<AppRolePermission> AppRolePermissions => Set<AppRolePermission>();
    public DbSet<RoleAssignment> RoleAssignments => Set<RoleAssignment>();
    public DbSet<Membershipdetail> Membershipdetails => Set<Membershipdetail>();
    public DbSet<SMSLog> SMSLogs => Set<SMSLog>();
    public DbSet<UserOtp> UserOtps => Set<UserOtp>();
    public DbSet<MembershipPurpose> MembershipPurposes => Set<MembershipPurpose>();
    public DbSet<NonMemberVerification> NonMemberVerifications => Set<NonMemberVerification>();
    public DbSet<NonMemberVerificationDocument> NonMemberVerificationDocuments => Set<NonMemberVerificationDocument>();
    public DbSet<MemberTypeModuleAssignment> MemberTypeModuleAssignments => Set<MemberTypeModuleAssignment>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<RequestTracking> RequestTrackings => Set<RequestTracking>();
    public DbSet<RequestProcessStep> RequestProcessSteps => Set<RequestProcessStep>();
    public DbSet<UserMembershipPurpose> UserMembershipPurposes => Set<UserMembershipPurpose>();
    public DbSet<AppPermission> AppPermissions => Set<AppPermission>();
    public DbSet<AppRole> AppRoles => Set<AppRole>();
    public DbSet<AppUserRole> AppUserRoles => Set<AppUserRole>();
    public DbSet<AppRoleModule> AppRoleModules => Set<AppRoleModule>();
    public DbSet<UserSubModuleAssignment> UserSubModuleAssignments => Set<UserSubModuleAssignment>();
    public DbSet<UserPermissionAssignment> UserPermissionAssignments => Set<UserPermissionAssignment>();
    public DbSet<UserClubAssignment> UserClubAssignments => Set<UserClubAssignment>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<UserActivityLog> UserActivityLogs => Set<UserActivityLog>();
    //Panic Button
   public DbSet<EmergencyType> EmergencyTypes => Set<EmergencyType>();
   public DbSet<PanicRequest> PanicRequests => Set<PanicRequest>();
   public DbSet<PanicActionLog> PanicActionLogs => Set<PanicActionLog>();
   public DbSet<PanicLocationUpdate> PanicLocationUpdates => Set<PanicLocationUpdate>();


    public new DbSet<TEntity> Set<TEntity>() where TEntity : class => base.Set<TEntity>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        //builder.ApplyConfiguration(new EmergencyTypeConfig());
        //builder.ApplyConfiguration(new PanicRequestConfig());
        //builder.ApplyConfiguration(new PanicRequestActionLogConfig());
        //builder.ApplyConfiguration(new PanicLocationUpdateConfig());

        // Seed lookup (DON'T set Ser in seeds)
        //builder.Entity<EmergencyType>().HasData(
        //    new EmergencyType { Id = Guid.NewGuid(), Code = 1, Name = "Rescue", Description = "General rescue", HelplineNumber = "1122", IsActive = true, IsDeleted = false, Created = DateTime.UtcNow },
        //    new EmergencyType { Id = Guid.NewGuid(), Code = 2, Name = "Fire Brigade", Description = "Fire emergency", HelplineNumber = "16", IsActive = true, IsDeleted = false, Created = DateTime.UtcNow },
        //    new EmergencyType { Id = Guid.NewGuid(), Code = 3, Name = "Health Emergency", Description = "Ambulance/medical", HelplineNumber = "115", IsActive = true, IsDeleted = false, Created = DateTime.UtcNow },
        //    new EmergencyType { Id = Guid.NewGuid(), Code = 4, Name = "Police", Description = "Police helpline", HelplineNumber = "15", IsActive = true, IsDeleted = false, Created = DateTime.UtcNow },
        //    new EmergencyType { Id = Guid.NewGuid(), Code = 99, Name = "Other", Description = "Other emergency", HelplineNumber = null, IsActive = true, IsDeleted = false, Created = DateTime.UtcNow }
        //);

        base.OnModelCreating(builder);
        IdentityBuilder(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    private static void IdentityBuilder(ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                builder.Entity(entityType.ClrType)
                    .Property<int>("Ser")
                    .ValueGeneratedOnAdd()
                    .HasColumnOrder(1);

                builder.Entity(entityType.ClrType)
                    .Property<Guid>("Id")
                    .HasColumnOrder(2);
            }
        }

        // ---------------- Identity User ----------------
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users");
            entity.Property(e => e.Id).HasMaxLength(85);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(100);
        });

        // ---------------- Identity Role ----------------
        builder.Entity<IdentityRole>(entity =>
        {
            entity.ToTable("Role"); // keep default Identity roles separate
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

        // ---------------- App RBAC Tables ----------------
        builder.Entity<AppRole>(entity =>
        {
            entity.ToTable("AppRoles"); // avoid conflict with IdentityRole
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        builder.Entity<AppUserRole>(entity =>
        {
            entity.ToTable("AppUserRoles");
            entity.HasOne(ur => ur.User)
                  .WithMany(u => u.UserRoles)
                  .HasForeignKey(ur => ur.UserId);

            entity.HasOne(ur => ur.Role)
                  .WithMany(r => r.UserRoles)
                  .HasForeignKey(ur => ur.RoleId);
        });

        builder.Entity<AppRolePermission>(entity =>
        {
            entity.ToTable("AppRolePermissions");

            entity.HasOne(rp => rp.Role)
                  .WithMany(r => r.RolePermissions)
                  .HasForeignKey(rp => rp.RoleId);

            entity.HasOne(rp => rp.SubModule)
                  .WithMany()
                  .HasForeignKey(rp => rp.SubModuleId);
        });

        // ---------------- App Modules ----------------
        builder.Entity<SubModule>()
            .HasOne(s => s.Module)
            .WithMany(m => m.SubModules)
            .HasForeignKey(s => s.ModuleId);

        // Example other relation (yours)
        builder.Entity<RequestProcessStep>()
            .HasOne(p => p.RequestTracking)
            .WithMany(t => t.ProcessSteps)
            .HasForeignKey(p => p.RequestTrackingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserModuleAssignment>()
       .HasQueryFilter(x => x.IsDeleted == null || x.IsDeleted == false);

        builder.Entity<UserSubModuleAssignment>()
            .HasQueryFilter(x => x.IsDeleted == null || x.IsDeleted == false);

        builder.Entity<UserClubAssignment>()
            .HasQueryFilter(x => x.IsDeleted == null || x.IsDeleted == false);

        builder.Entity<UserPermissionAssignment>()
            .HasQueryFilter(x => x.IsDeleted == null || x.IsDeleted == false);

        //Panic Button Module
        builder.Entity<EmergencyType>()
    .HasQueryFilter(e => e.IsDeleted != true);




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
                        entry.Entity.IsActive = true;
                        entry.Entity.IsDeleted = false;
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
        var auditEntries = OnBeforeSaveChanges();
        var result =  base.SaveChangesAsync(cancellationToken);
        if (auditEntries.Any())
        {
             OnAfterSaveChangesAsync(auditEntries);
        }

        return result;
        //return base.SaveChangesAsync(cancellationToken);
    }
    private List<AuditEntry> OnBeforeSaveChanges()
    {
        ChangeTracker.DetectChanges();
        var auditEntries = new List<AuditEntry>();

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            var auditEntry = new AuditEntry(entry)
            {
                TableName = entry.Metadata.GetTableName() ?? entry.Entity.GetType().Name,
                UserId = "SYSTEM" // 👈 inject _currentUser.UserId here
            };
            auditEntries.Add(auditEntry);

            foreach (var property in entry.Properties)
            {
                string propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.NewValues[propertyName] = property.CurrentValue;
                        break;

                    case EntityState.Deleted:
                        auditEntry.OldValues[propertyName] = property.OriginalValue;
                        break;

                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                        }
                        break;
                }
            }
        }

        // Save audit entries temporarily
        foreach (var auditEntry in auditEntries)
        {
            AuditLogs.Add(auditEntry.ToAuditLog());
        }

        return auditEntries;
    }
    private Task OnAfterSaveChangesAsync(List<AuditEntry> auditEntries)
    {
        // Here you could push to ELK, Serilog, Kafka, etc.
        return Task.CompletedTask;
    }

}
