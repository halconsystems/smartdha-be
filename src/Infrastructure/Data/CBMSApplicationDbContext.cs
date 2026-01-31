using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Common;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Infrastructure.Data;
public class CBMSApplicationDbContext : DbContext, ICBMSApplicationDbContext
{
    private readonly IUser _loggedInUser;
    public CBMSApplicationDbContext(DbContextOptions<CBMSApplicationDbContext> options, IUser loggedInUser) : base(options)
    {
        _loggedInUser = loggedInUser;
    }

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Club> Clubs => Set<Club>();
    public DbSet<ClubImages> ClubImages => Set<ClubImages>();
    public DbSet<ClubCategory> ClubCategories => Set<ClubCategory>();
    public DbSet<Facility> Facilities => Set<Facility>();
    public DbSet<ClubFacility> ClubFacilities => Set<ClubFacility>();
    public DbSet<FacilityUnit> FacilityUnits => Set<FacilityUnit>();
    public DbSet<FacilityUnitBookingConfig> FacilityUnitBookingConfigs => Set<FacilityUnitBookingConfig>();
    public DbSet<FacilityUnitService> FacilityUnitServices => Set<FacilityUnitService>();
    public DbSet<FacilityUnitSlot> FacilityUnitSlots => Set<FacilityUnitSlot>();
    public DbSet<FacilitiesImage> FacilitiesImages => Set<FacilitiesImage>();


    //Bookings table here 
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingSchedule> BookingSchedules => Set<BookingSchedule>();
    public DbSet<BookingService> BookingServices => Set<BookingService>();

    public override async Task<int> SaveChangesAsync(
         CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.Created = now;
                entry.Entity.CreatedBy = _loggedInUser.Id;
                entry.Entity.IsActive = true;
                entry.Entity.IsDeleted = false;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.LastModified = now;
                entry.Entity.LastModifiedBy = _loggedInUser.Id;
            }

            // Soft Delete support
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.IsActive = false;
                entry.Entity.LastModified = now;
                entry.Entity.LastModifiedBy = _loggedInUser.Id;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply configurations if you use IEntityTypeConfiguration<>
        modelBuilder.ApplyConfigurationsFromAssembly(
       typeof(CBMSApplicationDbContext).Assembly,
       t => t.Namespace!.Contains("Data.CBMS"));

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasOne(b => b.Club)
                .WithMany()
                .HasForeignKey(b => b.ClubId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(b => b.Facility)
                .WithMany()
                .HasForeignKey(b => b.FacilityId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(b => b.FacilityUnit)
                .WithMany()
                .HasForeignKey(b => b.FacilityUnitId)
                .OnDelete(DeleteBehavior.NoAction);
        });


        base.OnModelCreating(modelBuilder);

    }
}
