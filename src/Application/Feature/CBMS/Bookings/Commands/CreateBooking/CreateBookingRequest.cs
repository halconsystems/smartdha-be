using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.Bookings.Commands.CreateBooking;
public class CreateBookingRequest
{
    public Guid ClubId { get; set; }
    public Guid FacilityId { get; set; }
    public Guid FacilityUnitId { get; set; }
    public BookingMode BookingMode { get; set; }
    // Slot-based
    public SlotBookingRequest? SlotRequest { get; set; }
    // Date-range
    public DateRangeBookingRequest? DateRangeRequest { get; set; }
    public decimal? DiscountPercent { get; set; }
}
