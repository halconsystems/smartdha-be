using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.Bookings.Queries.MyBookings;
public class MyBookingSummaryDto
{
    public Guid BookingId { get; set; }
    public string? ClubName { get; set; }
    public string FacilityName { get; set; } = default!;
    public string FacilityUnitName { get; set; } = default!;

    public BookingMode BookingMode { get; set; }
    public BookingStatus Status { get; set; }
    public PaymentStatus PaymentStatus { get; set; }

    public decimal TotalAmount { get; set; }
    public bool IsPaid { get; set; }

    public DateTime Created { get; set; }
}

