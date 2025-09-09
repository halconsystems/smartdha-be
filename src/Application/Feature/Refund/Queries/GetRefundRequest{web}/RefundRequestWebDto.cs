using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Refund.Queries.GetRefundRequest_web_;
public class RefundRequestWebDto
{
    public Guid RefundRequestId { get; set; }
    public Guid ReservationId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal AmountPaid { get; set; }
   
    [Column(TypeName = "decimal(18,2)")]
    public decimal RefundableAmount { get; set; }
    public string Status { get; set; } = default!;
    public DateOnly RequestedAtDateOnly { get; set; }
    public TimeOnly RequestedAtTimeOnly { get; set; }


    // Club info
    public Guid ClubId { get; set; }
    public string ClubName { get; set; } = default!;

    // User info
    public string UserId { get; set; } = Guid.NewGuid().ToString();
    public string UserName { get; set; } = default!;
    public string CNIC { get; set; } = default!;
    public string? RegisteredMobileNo { get; set; } = default!;

    // Room bookings inside the reservation
    public List<RoomBookingDto> RoomBookings { get; set; } = new();
}

public class RoomBookingDto
{
    public Guid RoomBookingId { get; set; }
    public string? RoomName { get; set; } 
    public string RoomNo { get; set; } = default!;
}
