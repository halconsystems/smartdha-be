using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.ReservationDashboard;
public class ReservationDashboardDto
{
    public int TotalReservations { get; set; }
    public int ActiveReservations { get; set; }
    public int CancelledReservations { get; set; }
    public int CompletedReservations { get; set; }

    public decimal TotalRevenue { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal PendingAmount { get; set; }

    public Dictionary<string, int> ReservationsByClub { get; set; } = new();
    public Dictionary<string, int> ReservationsByStatus { get; set; } = new();

    // 👇 Stage-wise summary
    public int ReservationStageApproved { get; set; }
    public int ReservationStagePending { get; set; }
    public int PaymentStageApproved { get; set; }
    public int PaymentStagePending { get; set; }
    public int BookingStageApproved { get; set; }
    public int BookingStagePending { get; set; }
}

