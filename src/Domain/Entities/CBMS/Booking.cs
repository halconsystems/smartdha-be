using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;
public class Booking : BaseAuditableEntity
{
    public BookingType BookingType { get; set; }     // Facility / Event etc
    public BookingMode BookingMode { get; set; }     // SlotBased / DayBased
    // Who booked
    public string MembershipdetailId { get; set; } = default!;      // Member / Staff
    public string UserName { get; set; } = default!;
    public string? UserContact { get; set; }
    public string? Email { get; set; }
    // Where
    public Guid ClubId { get; set; }
    public Club Club { get; set; } = default!;
    public Guid FacilityId { get; set; }
    public Facility Facility { get; set; } = default!;
    public Guid FacilityUnitId { get; set; }   // physical instance
    public FacilityUnit FacilityUnit { get; set; } = default!;  // physical instance
    // Status
    // ===== FINANCIAL =====
    public decimal SubTotal { get; set; }        // Base price
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsPaid { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public BookingStatus Status { get; set; } = default!;
    // Approval
    public bool RequiresApproval { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedOn { get; set; }
    public string? Remarks { get; set; }
}
