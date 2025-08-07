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
public class OLMRSApplicationDbContext : DbContext, IOLMRSApplicationDbContext
{
    private readonly IUser _loggedInUser;
    public OLMRSApplicationDbContext(DbContextOptions<OLMRSApplicationDbContext> options, IUser loggedInUser) : base(options) 
    { 
        _loggedInUser = loggedInUser;
    }

    // Newly added DbSets
    public DbSet<Club> Clubs => Set<Club>();
    public DbSet<RoomCategory> RoomCategories => Set<RoomCategory>();
    public DbSet<ResidenceType> ResidenceTypes => base.Set<ResidenceType>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<RoomCharges> RoomCharges => Set<RoomCharges>();
    public DbSet<RoomImages> RoomImages => Set<RoomImages>();
    public DbSet<Services> Services => Set<Services>();
    public DbSet<ServiceMapping> ServiceMappings => Set<ServiceMapping>();
    public DbSet<ExtraServiceCharges> ExtraServiceCharges => Set<ExtraServiceCharges>();
    public DbSet<RoomBooking> RoomBookings => Set<RoomBooking>();
    public DbSet<UserClubMembership> UserClubMembership => Set<UserClubMembership>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OLMRSApplicationDbContext).Assembly);
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
