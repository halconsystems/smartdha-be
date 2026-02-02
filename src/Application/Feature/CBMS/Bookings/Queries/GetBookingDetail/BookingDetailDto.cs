using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.Bookings.Queries.GetBookingDetail;
public class BookingDetailDto
{
    public Guid BookingId { get; set; }

    public string FacilityName { get; set; } = default!;
    public string FacilityUnitName { get; set; } = default!;

    public BookingMode BookingMode { get; set; }
    public BookingStatus Status { get; set; }
    public PaymentStatus PaymentStatus { get; set; }

    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }

    public bool IsPaid { get; set; }

    // Slot-based
    public DateOnly? SlotDate { get; set; }
    public List<BookingSlotDto>? Slots { get; set; }

    // Date-range
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }

    public DateTime Created { get; set; }
}
public class BookingSlotDto
{
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}
