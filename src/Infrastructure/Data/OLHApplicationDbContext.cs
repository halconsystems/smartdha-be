using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Common;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Infrastructure.Data;
public class OLHApplicationDbContext : DbContext, IOLHApplicationDbContext
{
    private readonly IUser _loggedInUser;
    public OLHApplicationDbContext(DbContextOptions<OLHApplicationDbContext> options, IUser loggedInUser) : base(options)
    {
        _loggedInUser = loggedInUser;
    }
    public DbSet<OLH_BowserCapacity> BowserCapacitys => Set<OLH_BowserCapacity>();
    public DbSet<OLH_BowserCapacityRate> BowserCapacityRates => Set<OLH_BowserCapacityRate>();
    public DbSet<OLH_BowserDriverShift> BowserDriverShifts => Set<OLH_BowserDriverShift>();
    public DbSet<OLH_BowserRequest> BowserRequests => Set<OLH_BowserRequest>();
    public DbSet<OLH_DriverInfo> DriverInfos => Set<OLH_DriverInfo>();
    public DbSet<OLH_DriverStatus> DriverStatuses => Set<OLH_DriverStatus>();
    public DbSet<OLH_Shift> Shifts => Set<OLH_Shift>();
    public DbSet<OLH_Vehicle> Vehicles => Set<OLH_Vehicle>();
    public DbSet<OLH_VehicleMake> VehicleMakes { get; set; }
    public DbSet<OLH_VehicleModel> VehicleModels { get; set; }
    public DbSet<OLH_VehicleOwner> VehicleOwners => Set<OLH_VehicleOwner>();
    public DbSet<OLH_VehicleStatus> VehicleStatuses => Set<OLH_VehicleStatus>();
    public DbSet<OLH_VehicleType> VehicleTypes => Set<OLH_VehicleType>();
    //New Table created by sl
    public DbSet<OLH_Phase> Phases => Set<OLH_Phase>();
    public DbSet<OLH_PhaseCapacity> PhaseCapacities => Set<OLH_PhaseCapacity>();
    public DbSet<OLH_DriverShift> DriverShifts => Set<OLH_DriverShift>();
    public DbSet<OLH_BowserRequestStatusHistory> BowserRequestStatusHistorys => Set<OLH_BowserRequestStatusHistory>();
    public DbSet<OLH_BowserAssignmentHistory> BowserAssignmentHistorys => Set<OLH_BowserAssignmentHistory>();
    public DbSet<OLH_Payment> Payments => Set<OLH_Payment>();
    public DbSet<OLH_Refund> Refunds => Set<OLH_Refund>();
    public DbSet<OLH_BowserRequestNextStatus> BowserRequestsNextStatus => Set<OLH_BowserRequestNextStatus>();

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

        // Unique + operational indexes
        modelBuilder.Entity<OLH_BowserRequest>().HasIndex(x => x.RequestNo).IsUnique();
        modelBuilder.Entity<OLH_DriverShift>().HasIndex(x => new { x.DriverId, x.DutyDate }).IsUnique();
        modelBuilder.Entity<OLH_DriverShift>().HasIndex(x => new { x.VehicleId, x.DutyDate }).IsUnique();

        // PhaseCapacity uniqueness (one active row per Phase×Capacity×window recommended)
        modelBuilder.Entity<OLH_PhaseCapacity>()
            .HasIndex(x => new { x.PhaseId, x.BowserCapacityId, x.EffectiveFrom, x.EffectiveTo });

        // Delete behaviors to preserve history
        modelBuilder.Entity<OLH_BowserRequest>()
            .HasOne(x => x.AssignedDriver).WithMany()
            .HasForeignKey(x => x.AssignedDriverId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OLH_BowserRequest>()
            .HasOne(x => x.AssignedVehicle).WithMany()
            .HasForeignKey(x => x.AssignedVehicleId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OLH_BowserRequestNextStatus>()
            .HasOne(x => x.Status).WithMany().HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<OLH_BowserRequestNextStatus>()
            .HasOne(x => x.NextStatus).WithMany().HasForeignKey(x => x.NextStatusId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<OLH_BowserRequestNextStatus>()
            .HasIndex(x => new { x.StatusId, x.NextStatusId }).IsUnique();

        // Money
        modelBuilder.Entity<OLH_BowserRequest>().Property(x => x.Amount).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<OLH_BowserCapacity>().Property(x => x.Capacity).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<OLH_PhaseCapacity>().Property(x => x.BaseRate).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<OLH_Payment>().Property(x => x.Amount).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<OLH_Refund>().Property(x => x.Amount).HasColumnType("decimal(18,2)");

        // TimeOnly conversions (if needed)
        modelBuilder.Entity<OLH_Shift>().Property(x => x.StartTime)
            .HasConversion(v => v.ToTimeSpan(), v => TimeOnly.FromTimeSpan(v));
        modelBuilder.Entity<OLH_Shift>().Property(x => x.EndTime)
            .HasConversion(v => v.ToTimeSpan(), v => TimeOnly.FromTimeSpan(v));

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
