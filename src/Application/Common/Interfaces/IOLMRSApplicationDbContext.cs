using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;
public interface IOLMRSApplicationDbContext
{
    // New Room Booking–related DbSets
    DbSet<Club> Clubs { get; }
    DbSet<RoomCategory> RoomCategories { get; }
    DbSet<ResidenceType> ResidenceTypes { get; }
    DbSet<Room> Rooms { get; }
    DbSet<RoomCharges> RoomCharges { get; }
    DbSet<RoomImages> RoomImages { get; }
    DbSet<RoomRatings> RoomRatings { get; }
    DbSet<RoomAvailability> RoomsAvailability { get; }
    DbSet<Services> Services { get; }
    DbSet<ServiceMapping> ServiceMappings { get; }
    DbSet<ExtraServiceCharges> ExtraServiceCharges { get; }
    DbSet<RoomBooking> RoomBookings { get; }
    DbSet<UserClubMembership> UserClubMembership { get; }
    DbSet<RoomAvailability> RoomAvailabilities { get; }


    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    DatabaseFacade Database { get; }
}
