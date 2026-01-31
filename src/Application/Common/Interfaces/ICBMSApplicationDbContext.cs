using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;
public interface ICBMSApplicationDbContext
{
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<Club> Clubs { get; }
    DbSet<ClubImages> ClubImages { get; }
    DbSet<ClubCategory> ClubCategories { get; }
    DbSet<Facility> Facilities { get; }
    DbSet<FacilitiesImage> FacilitiesImages { get; }
    DbSet<FacilityService> FacilityServices { get; }

    DbSet<ClubFacility> ClubFacilities { get; }
    DbSet<FacilityUnit> FacilityUnits { get; }
    DbSet<FacilityUnitBookingConfig> FacilityUnitBookingConfigs { get; }
    DbSet<FacilityUnitService> FacilityUnitServices { get; }

    //Booking related
    DbSet<Booking> Bookings { get; }
    DbSet<BookingSchedule> BookingSchedules { get; }
    DbSet<BookingService> BookingServices { get; }
    DbSet<BookingDateRange> BookingDateRanges { get; }
    DbSet<FacilityUnitImage> FacilityUnitImages { get; }


    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    DatabaseFacade Database { get; }
}
