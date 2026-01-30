using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Entities.GBMS;
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
    DbSet<RefundRequest> RefundRequests { get; }
    DbSet<RefundPolicy> RefundPolicies { get; }
    DbSet<ClubBookingStandardTime> ClubBookingStandardTimes { get; }
    DbSet<DiscountSetting> DiscountSettings { get; }
    DbSet<Grounds> Grounds { get; }
    DbSet<GroundSlots> GroundSlots { get; }
    DbSet<GroundImages> GroundImages { get; }
    DbSet<GroundSetting> GroundSettings { get; }
    DbSet<GroundStandtardTime> GroundStandtardTimes { get; }
    DbSet<GroundPaymentIpnLogs> GroundPaymentIpnLogs { get; }
    DbSet<GroundBooking> GroundBookings { get; }
    DbSet<GroundBookingSlot> GroundBookingSlots { get; }
    DbSet<ClubCategory> ClubCategories { get; }
    DbSet<ClubImages> ClubImages { get; }
    DbSet<ClubPrerequisiteDefinitions> ClubPrerequisiteDefinitions { get; }
    DbSet<ClubFeeDefinition> ClubFeeDefinition { get; }
    DbSet<ClubFeeOption> ClubFeeOption { get; }
    DbSet<ClubFeeCategory> ClubFeeCategory { get; }
    DbSet<Facility> Facilities { get; }
    DbSet<ClubFacility> ClubFacilities { get; }



    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    DatabaseFacade Database { get; }
}
