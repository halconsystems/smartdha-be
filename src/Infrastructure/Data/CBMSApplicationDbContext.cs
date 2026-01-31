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
    public DbSet<ClubCategory> ClubCategories => Set<ClubCategory>(); // Events, Allaid Facilities, Sports etc
    public DbSet<Facility> Facilities => Set<Facility>(); // Events -> Banquet Hall, Sports -> Tennis Court , Padel Courts etc
    public DbSet<FacilityService> FacilityServices => Set<FacilityService>(); //Banquet Hall -> Catering, Decoration etc
    public DbSet<FacilitiesImage> FacilitiesImages => Set<FacilitiesImage>(); //Banquet Hall -> Image1, Image2 etc
    public DbSet<ClubFacility> ClubFacilities => Set<ClubFacility>(); // Club specific facilities with pricing and availability
    public DbSet<FacilityUnit> FacilityUnits => Set<FacilityUnit>(); //Banquet Hall (Banquet Hall 1, Banquet Hall 2), Tennis Court (Tennis Court A, Tennis Court B)
    public DbSet<FacilityUnitBookingConfig> FacilityUnitBookingConfigs => Set<FacilityUnitBookingConfig>(); // Banquet Hall 1 (Hourly, Daily), Tennis Court A (Hourly)
    public DbSet<FacilityUnitService> FacilityUnitServices => Set<FacilityUnitService>(); // Banquet Hall 1 -> Catering; Banquet Hall 2 -> Decoration;Banquet Hall 3 -> Catering,Decoration etc

    //Bookings table here 
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingSchedule> BookingSchedules => Set<BookingSchedule>();
    public DbSet<BookingDateRange> BookingDateRanges => Set<BookingDateRange>();
    public DbSet<BookingService> BookingServices => Set<BookingService>();
    public DbSet<FacilityUnitImage> FacilityUnitImages => Set<FacilityUnitImage>();

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

        modelBuilder.Entity<FacilityUnitService>(entity =>
        {
            entity.HasOne(x => x.FacilityUnit)
                .WithMany()
                .HasForeignKey(x => x.FacilityUnitId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(x => x.FacilityService)
                .WithMany()
                .HasForeignKey(x => x.FacilityServiceId)
                .OnDelete(DeleteBehavior.NoAction);
        });


        base.OnModelCreating(modelBuilder);

    }
}
