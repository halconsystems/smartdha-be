using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.Admin.Queries.GetAllAdminBookings;
public record GetAllAdminBookingsQuery()
    : IRequest<ApiResult<List<AdminBookingListDto>>>;
public class GetAllAdminBookingsHandler
    : IRequestHandler<GetAllAdminBookingsQuery, ApiResult<List<AdminBookingListDto>>>
{
    private readonly ICBMSApplicationDbContext _db;
    private readonly IClubAccessService _clubAccess;
    private readonly ICurrentUserService _currentUser;

    public GetAllAdminBookingsHandler(
        ICBMSApplicationDbContext db,
        IClubAccessService clubAccess,
        ICurrentUserService currentUser)
    {
        _db = db;
        _clubAccess = clubAccess;
        _currentUser = currentUser;
    }

    public async Task<ApiResult<List<AdminBookingListDto>>> Handle(
        GetAllAdminBookingsQuery request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId.ToString();
        if (string.IsNullOrEmpty(userId))
            throw new UnAuthorizedException("Invalid user context.");

        // 🔑 Resolve club access
        var allowedClubIds =
            await _clubAccess.GetAllowedClubIdsAsync(userId, ct);

        // 🔹 Base bookings query
        var bookingsQuery = _db.Bookings
            .AsNoTracking()
            .AsQueryable();

        // 🔒 Apply club restriction (non-super admin)
        if (allowedClubIds != null)
            bookingsQuery = bookingsQuery
                .Where(b => allowedClubIds.Contains(b.ClubId));

        // 🔹 Join required tables
        var result = await (
            from b in bookingsQuery
            join f in _db.Facilities on b.FacilityId equals f.Id
            join u in _db.FacilityUnits on b.FacilityUnitId equals u.Id
            join c in _db.ClubServiceCategories on f.ClubCategoryId equals c.Id
            orderby b.Created descending
            select new AdminBookingListDto(
                b.Id,
                b.Created,

                c.Name,
                f.DisplayName,
                u.Name,

                b.UserName,
                b.UserContact ?? string.Empty,

                b.BookingMode,
                b.Status,
                b.PaymentStatus,

                b.SubTotal,
                b.DiscountAmount,
                b.TaxAmount,
                b.TotalAmount
            )
        ).ToListAsync(ct);

        return ApiResult<List<AdminBookingListDto>>.Ok(result);
    }
}

