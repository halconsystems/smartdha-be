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

    public int AwaitingPaymentReservations { get; set; } // instead of PendingAmount

    public List<ClubDashboardDto> Clubs { get; set; } = new();

    // Stage tracking
    public int ReservationStageApproved { get; set; }
    public int ReservationStagePending { get; set; }
    public int PaymentStageApproved { get; set; }
    public int PaymentStagePending { get; set; }
    public int BookingStageApproved { get; set; }
    public int BookingStagePending { get; set; }
}

public class ClubDashboardDto
{
    public string ClubName { get; set; } = string.Empty;
    public int TotalReservations { get; set; }
    public int ConfirmedReservations { get; set; }
    public int CancelledReservations { get; set; }
    public int AwaitingPaymentReservations { get; set; }
    public int TotalRoomsBooked { get; set; }
    public decimal ConfirmationPercentage { get; set; }
}


