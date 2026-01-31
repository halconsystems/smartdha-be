using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;
public class Booking : BaseAuditableEntity
{
    public BookingType BookingType { get; set; }

    // Who booked
    public Guid UserId { get; set; }        // Member / Staff
    public string UserName { get; set; } = default!;
    public string? UserContact { get; set; }

    // Where
    public Guid ClubId { get; set; }
    public Club Club { get; set; } = default!;

    public Guid FacilityId { get; set; }
    public Facility Facility { get; set; } = default!;

    // Status
    public BookingStatus Status { get; set; } = BookingStatus.Draft;

    // Financial
    public decimal? TotalAmount { get; set; }
    public bool IsPaid { get; set; }

    // Approval
    public bool RequiresApproval { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedOn { get; set; }

    public string? Remarks { get; set; }

}
