using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.ConfirmBooking.Commands;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.Bookings.Queries.GetBookingDetail;
public record GetBookingDetailQuery(Guid BookingId)
    : IRequest<ApiResult<BookingDetailDto>>;
public class GetBookingDetailQueryHandler
    : IRequestHandler<GetBookingDetailQuery, ApiResult<BookingDetailDto>>
{
    private readonly ICBMSApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetBookingDetailQueryHandler(
        ICBMSApplicationDbContext db,
        ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ApiResult<BookingDetailDto>> Handle(
        GetBookingDetailQuery request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId.ToString();

        var booking = await _db.Bookings
            .Include(b => b.Facility)
            .Include(b => b.FacilityUnit)
            .FirstOrDefaultAsync(b =>
                b.Id == request.BookingId &&
                b.CreatedBy == userId,
                ct);

        if (booking == null)
            return ApiResult<BookingDetailDto>.Fail("Booking not found.");

        var dto = new BookingDetailDto
        {
            BookingId = booking.Id,

            FacilityName = booking.Facility.DisplayName,
            FacilityUnitName = booking.FacilityUnit.Name,

            BookingMode = booking.BookingMode,
            Status = booking.Status,
            PaymentStatus = booking.PaymentStatus,

            SubTotal = booking.SubTotal,
            DiscountAmount = booking.DiscountAmount,
            TaxAmount = booking.TaxAmount,
            TotalAmount = booking.TotalAmount,

            IsPaid = booking.IsPaid,
            Created = booking.Created
        };

        // SLOT BASED
        if (booking.BookingMode == BookingMode.SlotBased)
        {
            var slots = await _db.BookingSchedules
                .Where(x => x.BookingId == booking.Id)
                .OrderBy(x => x.StartTime)
                .ToListAsync(ct);

            dto.SlotDate = slots.First().Date;
            dto.Slots = slots.Select(x => new BookingSlotDto
            {
                StartTime = x.StartTime,
                EndTime = x.EndTime
            }).ToList();
        }
        // DATE RANGE
        else
        {
            var range = await _db.BookingDateRanges
                .FirstAsync(x => x.BookingId == booking.Id, ct);

            dto.FromDate = range.FromDate;
            dto.ToDate = range.ToDate;
        }

        return ApiResult<BookingDetailDto>.Ok(dto);
    }
}

