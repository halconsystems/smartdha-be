using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.Admin.Queries.AdminBookingDashboard;
public record GetAdminBookingDashboardQuery()
    : IRequest<ApiResult<AdminBookingDashboardDto>>;
public class GetAdminBookingDashboardHandler
    : IRequestHandler<GetAdminBookingDashboardQuery, ApiResult<AdminBookingDashboardDto>>
{
    private readonly ICBMSApplicationDbContext _db;
    private readonly IClubAccessService _clubAccess;
    private readonly ICurrentUserService _currentUserService;
    private readonly ISmartPayService _smartPayService;

    public GetAdminBookingDashboardHandler(ICBMSApplicationDbContext db, IClubAccessService clubAccess, ICurrentUserService currentUserService, ISmartPayService smartPayService)
    {
        _db = db;
        _clubAccess = clubAccess;
        _currentUserService = currentUserService;
        _smartPayService = smartPayService;
    }
    public async Task<ApiResult<AdminBookingDashboardDto>> Handle(
        GetAdminBookingDashboardQuery request,
        CancellationToken ct)
    {
        var userId = _currentUserService.UserId.ToString();
        if (string.IsNullOrEmpty(userId))
            throw new UnAuthorizedException("Invalid user context.");

        // 🔑 Resolve allowed clubs
        var allowedClubIds =
            await _clubAccess.GetAllowedClubIdsAsync(userId, ct);

        var today = DateTime.Today;
        var weekStart = today.AddDays(-7);
        var monthStart = new DateTime(today.Year, today.Month, 1);

        // 🔹 Base bookings query (PAID only)
        var bookings = _db.Bookings
            .AsNoTracking()
            .Where(b => b.PaymentStatus == PaymentStatus.Paid)
            .AsQueryable();


        // 🔒 Apply club restriction if not SuperAdmin
        if (allowedClubIds != null)
        {
            bookings = bookings
                .Where(b => allowedClubIds.Contains(b.ClubId));
        }

        // =========================
        // TODAY
        // =========================
        var todayData = await bookings
            .Where(b => b.Created >= today)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Count = g.Count(),
                Revenue = g.Sum(x => x.TotalAmount)
            })
            .FirstOrDefaultAsync(ct);

        // =========================
        // THIS WEEK
        // =========================
        var weekData = await bookings
            .Where(b => b.Created >= weekStart)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Count = g.Count(),
                Revenue = g.Sum(x => x.TotalAmount)
            })
            .FirstOrDefaultAsync(ct);

        // =========================
        // THIS MONTH
        // =========================
        var monthData = await bookings
            .Where(b => b.Created >= monthStart)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Count = g.Count(),
                Revenue = g.Sum(x => x.TotalAmount)
            })
            .FirstOrDefaultAsync(ct);

        return ApiResult<AdminBookingDashboardDto>.Ok(
            new AdminBookingDashboardDto(
                todayData?.Count ?? 0,
                todayData?.Revenue ?? 0,

                weekData?.Count ?? 0,
                weekData?.Revenue ?? 0,

                monthData?.Count ?? 0,
                monthData?.Revenue ?? 0
            ));
    }
}

