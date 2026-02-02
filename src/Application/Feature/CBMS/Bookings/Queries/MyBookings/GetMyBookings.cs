using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.Bookings.Queries.MyBookings;
public record GetMyBookingsQuery
    : IRequest<ApiResult<List<MyBookingSummaryDto>>>;
public class GetMyBookingsQueryHandler
    : IRequestHandler<GetMyBookingsQuery, ApiResult<List<MyBookingSummaryDto>>>
{
    private readonly ICBMSApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetMyBookingsQueryHandler(
        ICBMSApplicationDbContext db,
        ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ApiResult<List<MyBookingSummaryDto>>> Handle(
        GetMyBookingsQuery request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId.ToString();

        var bookings = await _db.Bookings
            .AsNoTracking()
            .Where(b => b.CreatedBy == userId)
            .OrderByDescending(b => b.Created)
            .Select(b => new MyBookingSummaryDto
            {
                BookingId = b.Id,

                FacilityName = b.Facility.DisplayName,
                FacilityUnitName = b.FacilityUnit.Name,
                ClubName=b.Club.Name,

                BookingMode = b.BookingMode,
                Status = b.Status,
                PaymentStatus = b.PaymentStatus,

                TotalAmount = b.TotalAmount,
                IsPaid = b.IsPaid,

                Created = b.Created
            })
            .ToListAsync(ct);

        return ApiResult<List<MyBookingSummaryDto>>.Ok(bookings);
    }
}

