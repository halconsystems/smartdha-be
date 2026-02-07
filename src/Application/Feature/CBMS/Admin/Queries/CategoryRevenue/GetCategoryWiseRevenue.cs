using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.Admin.Queries.CategoryRevenue;
public record GetCategoryWiseRevenueQuery()
    : IRequest<ApiResult<List<CategoryRevenueDto>>>;
public class GetCategoryWiseRevenueHandler
    : IRequestHandler<GetCategoryWiseRevenueQuery, ApiResult<List<CategoryRevenueDto>>>
{
    private readonly ICBMSApplicationDbContext _db;
    private readonly IClubAccessService _clubAccess;
    private readonly ICurrentUserService _currentUserService;

    public GetCategoryWiseRevenueHandler(ICBMSApplicationDbContext db, IClubAccessService clubAccess, ICurrentUserService currentUserService)
    {
        _db = db;
        _clubAccess = clubAccess;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResult<List<CategoryRevenueDto>>> Handle(
        GetCategoryWiseRevenueQuery request,
        CancellationToken ct)
    {
        var userId = _currentUserService.UserId.ToString();
        if (string.IsNullOrEmpty(userId))
            throw new UnAuthorizedException("Invalid user context.");

        // 🔑 Resolve club access
        var allowedClubIds =
            await _clubAccess.GetAllowedClubIdsAsync(userId, ct);

        // 🔹 Base paid bookings query
        var bookings = _db.Bookings
            .AsNoTracking()
            .Where(b => b.PaymentStatus == PaymentStatus.Paid);

        // 🔒 Apply club restriction if needed
        if (allowedClubIds != null)
            bookings = bookings
                .Where(b => allowedClubIds.Contains(b.ClubId));

        // 🔹 Category-wise revenue
        var result = await (
            from b in bookings
            join f in _db.Facilities on b.FacilityId equals f.Id
            join c in _db.ClubServiceCategories on f.ClubCategoryId equals c.Id
            group b by new { c.Id, c.Name } into g
            select new CategoryRevenueDto(
                g.Key.Id,
                g.Key.Name,
                g.Sum(x => x.TotalAmount)
            )
        ).ToListAsync(ct);

        return ApiResult<List<CategoryRevenueDto>>.Ok(result);
    }
}

