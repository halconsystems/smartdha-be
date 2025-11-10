using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Bowzer.Web.Queries.BowserDashboard;
public class BowserDashboardDto
{
    // --- Requests Summary ---
    public int TotalRequests { get; set; }
    public int ActiveRequests { get; set; }
    public int CompletedRequests { get; set; }
    public int CancelledRequests { get; set; }
    public int FailedRequests { get; set; }

    // --- Operational Metrics ---
    public int TotalDrivers { get; set; }
    public int AvailableDrivers { get; set; }
    public int OnDutyDrivers { get; set; }

    public int TotalVehicles { get; set; }
    public int ActiveVehicles { get; set; }
    public int InMaintenanceVehicles { get; set; }

    public int TotalPhases { get; set; }
    public int TotalCapacities { get; set; }

    // --- Financial Summary ---
    public decimal TotalRevenue { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TotalRefunded { get; set; }
    public int PaidRequests { get; set; }
    public int AwaitingPaymentRequests { get; set; }

    // --- Stage Distribution ---
    public Dictionary<string, int> RequestStageDistribution { get; set; } = new();

    // --- Optional Charts ---
    public List<PhaseWiseStatsDto> PhaseWiseRequests { get; set; } = new();
    public List<CapacityWiseStatsDto> CapacityWiseRequests { get; set; } = new();
}

public class PhaseWiseStatsDto
{
    public string PhaseName { get; set; } = string.Empty;
    public int TotalRequests { get; set; }
    public int CompletedRequests { get; set; }
    public int CancelledRequests { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class CapacityWiseStatsDto
{
    public string CapacityLabel { get; set; } = string.Empty; // e.g. "9 Gallon"
    public int TotalRequests { get; set; }
    public decimal TotalRevenue { get; set; }
}

