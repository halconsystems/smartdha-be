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
    DbSet<RoomCharge> RoomCharges { get; }
    DbSet<RoomImage> RoomImages { get; }
    DbSet<RoomRating> RoomRatings { get; }
    DbSet<RoomAvailability> RoomAvailabilities { get; }
    DbSet<Services> Services { get; }
    DbSet<ServiceMapping> ServiceMappings { get; }
    DbSet<ExtraServiceCharges> ExtraServiceCharges { get; }
    DbSet<RoomBooking> RoomBookings { get; }
    DbSet<UserClubMembership> UserClubMemberships { get; }
    DbSet<BookingGuest> BookingGuests { get; }
    DbSet<Reservation> Reservations { get; }
    DbSet<ReservationRoom> ReservationRooms { get; }
    DbSet<Payment> Payments { get; }
    DbSet<PaymentIntent> PaymentIntents { get; }


    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    DatabaseFacade Database { get; }
}
