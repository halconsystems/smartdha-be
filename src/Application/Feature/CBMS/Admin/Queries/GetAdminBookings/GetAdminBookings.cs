using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.Admin.Queries.GetAdminBookings;
public record GetAdminBookingsQuery(
    Guid? CategoryId,
    Guid? FacilityId,
    Guid? FacilityUnitId,
    BookingStatus? BookingStatus,
    PaymentStatus? PaymentStatus,
    DateOnly? FromDate,
    DateOnly? ToDate
) : IRequest<ApiResult<List<AdminBookingListDto>>>;
public class GetAdminBookingsHandler
    : IRequestHandler<GetAdminBookingsQuery, ApiResult<List<AdminBookingListDto>>>
{
    private readonly ICBMSApplicationDbContext _db;
    private readonly IClubAccessService _clubAccess;
    private readonly ICurrentUserService _currentUserService;

    public GetAdminBookingsHandler(ICBMSApplicationDbContext db, IClubAccessService clubAccess, ICurrentUserService currentUserService)
    { 
         _db = db;
         _clubAccess = clubAccess;
            _currentUserService = currentUserService;
    }
    public async Task<ApiResult<List<AdminBookingListDto>>> Handle(
        GetAdminBookingsQuery request,
        CancellationToken ct)
    {
        var userId = _currentUserService.UserId.ToString();
        if (string.IsNullOrEmpty(userId))
            throw new UnAuthorizedException("Invalid user context.");

        // 🔑 Resolve club access ONCE
        var allowedClubIds =
            await _clubAccess.GetAllowedClubIdsAsync(userId, ct);

        // 🔹 Base query
        var bookingsQuery = _db.Bookings
            .AsNoTracking()
            .AsQueryable();

        // 🔒 Apply club restriction
        if (allowedClubIds != null)
            bookingsQuery = bookingsQuery
                .Where(b => allowedClubIds.Contains(b.ClubId));

        // 🔹 Join rest of data
        var query =
            from b in bookingsQuery
            join f in _db.Facilities on b.FacilityId equals f.Id
            join u in _db.FacilityUnits on b.FacilityUnitId equals u.Id
            join c in _db.ClubServiceCategories on f.ClubCategoryId equals c.Id
            select new { b, f, u, c };

        // 🔹 Filters
        if (request.CategoryId.HasValue)
            query = query.Where(x => x.c.Id == request.CategoryId);

        if (request.FacilityId.HasValue)
            query = query.Where(x => x.f.Id == request.FacilityId);

        if (request.FacilityUnitId.HasValue)
            query = query.Where(x => x.u.Id == request.FacilityUnitId);

        if (request.BookingStatus.HasValue)
            query = query.Where(x => x.b.Status == request.BookingStatus);

        if (request.PaymentStatus.HasValue)
            query = query.Where(x => x.b.PaymentStatus == request.PaymentStatus);

        if (request.FromDate.HasValue)
            query = query.Where(x => x.b.Created.Date >= request.FromDate.Value.ToDateTime(TimeOnly.MinValue));

        if (request.ToDate.HasValue)
            query = query.Where(x => x.b.Created.Date <= request.ToDate.Value.ToDateTime(TimeOnly.MaxValue));

        var result = await query
            .OrderByDescending(x => x.b.Created)
            .Select(x => new AdminBookingListDto(
                x.b.Id,
                x.b.Created,

                x.c.Name,
                x.f.DisplayName,
                x.u.Name,

                x.b.UserName,
                x.b.UserContact ?? string.Empty,

                x.b.BookingMode,
                x.b.Status,
                x.b.PaymentStatus,

                x.b.SubTotal,
                x.b.DiscountAmount,
                x.b.TaxAmount,
                x.b.TotalAmount
            ))
            .ToListAsync(ct);

        return ApiResult<List<AdminBookingListDto>>.Ok(result);
    }
}

