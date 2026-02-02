using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Enums;
using DHAFacilitationAPIs.Domain.Enums.CBMS;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.Bookings.Commands.CreateBooking;

public record SlotBookingRequest(
    DateOnly Date,
    List<SlotTimeRange> Slots
);

public record SlotTimeRange(
    TimeOnly StartTime,
    TimeOnly EndTime
);
public record DateRangeBookingRequest(
    DateOnly FromDate,
    DateOnly ToDate
);
public record CreateBookingCommand(
    Guid ClubId,
    Guid FacilityId,
    Guid FacilityUnitId,
    BookingMode BookingMode,
    SlotBookingRequest? SlotRequest,
    DateRangeBookingRequest? DateRangeRequest,
    decimal? DiscountPercent
) : IRequest<ApiResult<Guid>>;
public class CreateBookingCommandHandler
    : IRequestHandler<CreateBookingCommand, ApiResult<Guid>>
{
    private readonly ICBMSApplicationDbContext _db;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _applicationDbContext;
    public CreateBookingCommandHandler(ICBMSApplicationDbContext db, ICurrentUserService currentUserService, UserManager<ApplicationUser> userManager, IApplicationDbContext applicationDbContext)
    {
        _db = db;
        _currentUserService = currentUserService;
        _userManager = userManager;
        _applicationDbContext = applicationDbContext;
    }

    public async Task<ApiResult<Guid>> Handle(
     CreateBookingCommand request,
     CancellationToken ct)
    {
        var clubFacility = await _db.ClubFacilities.FirstOrDefaultAsync(x =>
            x.ClubId == request.ClubId &&
            x.FacilityId == request.FacilityId &&
            x.IsAvailable,
            ct);
        
            if (clubFacility == null)
                return ApiResult<Guid>.Fail("Facility not available.");

            var currentUserId = _currentUserService.UserId.ToString();
            if (string.IsNullOrEmpty(currentUserId))
                throw new UnAuthorizedException("Invalid user context.");

            var user = await _userManager.FindByIdAsync(currentUserId);
            if (user == null)
                throw new UnAuthorizedException("User Not found!");


            //var getMember = await _applicationDbContext.UserMemberProfiles
            //    .FirstOrDefaultAsync(x => x.UserId == currentUserId && x.IsActive == true && x.IsDeleted != true);

            //if (getMember == null)
            //    throw new UnAuthorizedException("Membership details!");

            decimal basePrice;
            decimal subTotal;

            // ============================
            // SLOT BASED (MULTI SLOT)
            // ============================
            if (request.BookingMode == BookingMode.SlotBased)
            {
                if (request.SlotRequest == null || request.SlotRequest.Slots == null || !request.SlotRequest.Slots.Any())
                    return ApiResult<Guid>.Fail("Slot information required.");

                // 🔒 Conflict check for EACH slot
                foreach (var slot in request.SlotRequest.Slots)
                {
                    var conflict = await _db.BookingSchedules.AnyAsync(b =>
                        b.Booking.FacilityUnitId == request.FacilityUnitId &&
                        b.Date == request.SlotRequest.Date &&
                        slot.StartTime < b.EndTime &&
                        slot.EndTime > b.StartTime &&
                        b.Booking.Status != BookingStatus.Cancelled,
                        ct);

                    if (conflict)
                        return ApiResult<Guid>.Fail(
                            $"Slot {slot.StartTime} - {slot.EndTime} is already booked.");
                }

                basePrice = clubFacility.Price ?? 0m;
                subTotal = basePrice * request.SlotRequest.Slots.Count;
            }
            // ============================
            // DATE RANGE
            // ============================
            else
            {
                if (request.DateRangeRequest == null)
                    return ApiResult<Guid>.Fail("Date range required.");

                var conflict = await _db.BookingDateRanges.AnyAsync(b =>
                    b.Booking.FacilityUnitId == request.FacilityUnitId &&
                    request.DateRangeRequest.FromDate < b.ToDate &&
                    request.DateRangeRequest.ToDate > b.FromDate &&
                    b.Booking.Status != BookingStatus.Cancelled,
                    ct);

                if (conflict)
                    return ApiResult<Guid>.Fail("Date range not available.");

                var days =
                    request.DateRangeRequest.ToDate.DayNumber -
                    request.DateRangeRequest.FromDate.DayNumber;

                if (days <= 0)
                    return ApiResult<Guid>.Fail("Invalid date range.");

                basePrice = clubFacility.Price ?? 0m;
                subTotal = days * basePrice;
            }

            // ============================
            // DISCOUNT
            // ============================
            var discount = request.DiscountPercent.HasValue
                ? subTotal * request.DiscountPercent.Value / 100m
                : 0m;

            // ============================
            // TAX (example 16%)
            // ============================
            var tax = (subTotal - discount) * 0.16m;
            var total = subTotal - discount + tax;
            // ============================
            // SAVE BOOKING
            // ============================
            var booking = new Booking
            {
                BookingType = BookingType.Facility,
                BookingMode = request.BookingMode,

                MembershipdetailId = currentUserId,
                UserName = user.Name,
                UserContact = user.RegisteredMobileNo,
                Email = user.Email,

                ClubId = request.ClubId,
                FacilityId = request.FacilityId,
                FacilityUnitId = request.FacilityUnitId,

                SubTotal = subTotal,
                DiscountAmount = discount,
                TaxAmount = tax,
                TotalAmount = total,

                Status = BookingStatus.Confirmed,
                RequiresApproval = false
            };

            _db.Bookings.Add(booking);


            // ============================
            // SAVE CHILD RECORDS
            // ============================
            if (request.BookingMode == BookingMode.SlotBased)
            {
                foreach (var slot in request.SlotRequest!.Slots)
                {
                    _db.BookingSchedules.Add(new BookingSchedule
                    {
                        BookingId = booking.Id,
                        Date = request.SlotRequest.Date,
                        StartTime = slot.StartTime,
                        EndTime = slot.EndTime
                    });
                }
            }
            else
            {
                _db.BookingDateRanges.Add(new BookingDateRange
                {
                    BookingId = booking.Id,
                    FromDate = request.DateRangeRequest!.FromDate,
                    ToDate = request.DateRangeRequest.ToDate
                });
            }
        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        return ApiResult<Guid>.Ok(booking.Id);
    }
}

