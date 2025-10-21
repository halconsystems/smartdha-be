using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Bowzer.Web.Queries.BowserDashboard;
public record GetBowserDashboardQuery : IRequest<BowserDashboardDto>;

public class GetBowserDashboardHandler : IRequestHandler<GetBowserDashboardQuery, BowserDashboardDto>
{
    private readonly IOLHApplicationDbContext _context;

    public GetBowserDashboardHandler(IOLHApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BowserDashboardDto> Handle(GetBowserDashboardQuery request, CancellationToken ct)
    {
        var dto = new BowserDashboardDto();

        // --- 1️⃣ Requests Summary ---
        var requests = await _context.BowserRequests
            .Include(r => r.Phase)
            .Include(r => r.BowserCapacity)
            .ToListAsync(ct);

        dto.TotalRequests = requests.Count;
        dto.CompletedRequests = requests.Count(r => r.Status == BowserStatus.Completed);
        dto.CancelledRequests = requests.Count(r => r.Status == BowserStatus.Cancelled);
        dto.FailedRequests = requests.Count(r => r.Status == BowserStatus.Failed);
        dto.ActiveRequests = requests.Count(r =>
            r.Status == BowserStatus.Submitted ||
            r.Status == BowserStatus.Accepted ||
            r.Status == BowserStatus.Dispatched ||
            r.Status == BowserStatus.OnRoute ||
            r.Status == BowserStatus.Arrived ||
            r.Status == BowserStatus.Delivering);

        // --- 2️⃣ Operational Stats ---
        dto.TotalDrivers = await _context.DriverInfos.CountAsync(ct);
        dto.AvailableDrivers = await _context.DriverInfos
            .CountAsync(d => d.DriverStatus.Status == "Available", ct);
        dto.OnDutyDrivers = await _context.DriverInfos
            .CountAsync(d => d.DriverStatus.Status == "OnDuty", ct);

        dto.TotalVehicles = await _context.Vehicles.CountAsync(ct);
        dto.ActiveVehicles = await _context.Vehicles
            .CountAsync(v => v.VehicleStatus.Status == "Active", ct);
        dto.InMaintenanceVehicles = await _context.Vehicles
            .CountAsync(v => v.VehicleStatus.Status == "Maintenance", ct);

        dto.TotalPhases = await _context.Phases.CountAsync(ct);
        dto.TotalCapacities = await _context.BowserCapacitys.CountAsync(ct);

        // --- 3️⃣ Financial Summary ---
        var payments = await _context.Payments.ToListAsync(ct);
        dto.TotalPaid = payments.Where(p => p.Status == PaymentStatus.Paid).Sum(p => p.Amount);
        dto.TotalRefunded = payments.Where(p => p.Status == PaymentStatus.Refunded).Sum(p => p.Amount);
        dto.TotalRevenue = dto.TotalPaid - dto.TotalRefunded;
        dto.PaidRequests = requests.Count(r => r.PaymentStatus == PaymentStatus.Paid);
        dto.AwaitingPaymentRequests = requests.Count(r => r.PaymentStatus == PaymentStatus.Pending);

        // --- 4️⃣ Stage Distribution ---
        dto.RequestStageDistribution = Enum.GetValues(typeof(BowserStatus))
            .Cast<BowserStatus>()
            .ToDictionary(
                s => s.ToString(),
                s => requests.Count(r => r.Status == s)
            );

        // --- 5️⃣ Phase-wise Stats ---
        dto.PhaseWiseRequests = requests
            .GroupBy(r => r.Phase.Name)
            .Select(g => new PhaseWiseStatsDto
            {
                PhaseName = g.Key,
                TotalRequests = g.Count(),
                CompletedRequests = g.Count(x => x.Status == BowserStatus.Completed),
                CancelledRequests = g.Count(x => x.Status == BowserStatus.Cancelled),
                TotalRevenue = g.Where(x => x.PaymentStatus == PaymentStatus.Paid).Sum(x => x.Amount)
            }).ToList();

        // --- 6️⃣ Capacity-wise Stats ---
        dto.CapacityWiseRequests = requests
            .GroupBy(r => $"{r.BowserCapacity.Capacity} {r.BowserCapacity.Unit}")
            .Select(g => new CapacityWiseStatsDto
            {
                CapacityLabel = g.Key,
                TotalRequests = g.Count(),
                TotalRevenue = g.Where(x => x.PaymentStatus == PaymentStatus.Paid).Sum(x => x.Amount)
            }).ToList();

        return dto;
    }
}

