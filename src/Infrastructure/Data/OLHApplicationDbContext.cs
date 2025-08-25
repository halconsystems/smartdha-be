using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Common;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Infrastructure.Data;
public class OLHApplicationDbContext : DbContext, IOLHApplicationDbContext
{
    private readonly IUser _loggedInUser;
    public OLHApplicationDbContext(DbContextOptions<OLHApplicationDbContext> options, IUser loggedInUser) : base(options)
    {
        _loggedInUser = loggedInUser;
    }
    public DbSet<OLH_BowserCapacity> BowserCapacitys =>Set<OLH_BowserCapacity>();

    public DbSet<OLH_BowserCapacityRate> BowserCapacityRates => Set<OLH_BowserCapacityRate>();

    public DbSet<OLH_BowserDriverShift> OLH_BowserDriverShifts => Set<OLH_BowserDriverShift>();

    public DbSet<OLH_BowserRequest> OLH_BowserRequests => Set<OLH_BowserRequest>();

    public DbSet<OLH_DriverInfo> DriverInfos => Set<OLH_DriverInfo>();

    public DbSet<OLH_DriverStatus> DriverStatuses => Set<OLH_DriverStatus>();

    public DbSet<OLH_Shift> OLH_Shifts => Set<OLH_Shift>();

    public DbSet<OLH_Vehicle> OLH_Vehicles => Set<OLH_Vehicle>();

    public DbSet<OLH_VehicleOwner> OLH_VehicleOwners => Set<OLH_VehicleOwner>();

    public DbSet<OLH_VehicleStatus> OLH_VehicleStatuses => Set<OLH_VehicleStatus>();

    public DbSet<OLH_VehicleType> OLH_VehicleTypes => Set<OLH_VehicleType>();

    public new DbSet<TEntity> Set<TEntity>() where TEntity : class
        => base.Set<TEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OLHApplicationDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property<int>("Ser")
                    .ValueGeneratedOnAdd()
                    .HasColumnOrder(1);

                modelBuilder.Entity(entityType.ClrType)
                    .Property<Guid>("Id")
                    .HasColumnOrder(2);
            }
        }

      
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
        return base.SaveChangesAsync(cancellationToken);
    }

}
