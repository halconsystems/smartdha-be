using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Domain.Entities;

[Table("BowserRequests", Schema = "dbo")]
public class OLH_BowserRequest : BaseAuditableEntity
{
    public string RequestNo { get; set; } = default!; // e.g., BZR-20250907-0001
    public DateTime RequestDate { get; set; }

    public Guid PhaseId { get; set; }               // selected from dropdown
    public OLH_Phase Phase { get; set; } = default!;

    public Guid BowserCapacityId { get; set; }      // must be allowed for Phase
    public OLH_BowserCapacity BowserCapacity { get; set; } = default!;

    public DateTime RequestedDeliveryDate { get; set; }
    public DateTime? PlannedDeliveryDate { get; set; }
    public DateTime? DeliveryDate { get; set; }

    // Address (keep simple; still useful for navigation)
    public string? Ext { get; set; }                // e.g., Sector/Street ext if used
    public string Street { get; set; } = default!;
    public string Address { get; set; } = default!;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }

    // Assignment (nullable until dispatch)
    public Guid? AssignedDriverId { get; set; }
    public OLH_DriverInfo? AssignedDriver { get; set; }
    public Guid? AssignedVehicleId { get; set; }
    public OLH_Vehicle? AssignedVehicle { get; set; }

    // Money
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "PKR";

    // ✨ replaced lookup with enum
    public BowserStatus Status { get; set; } = BowserStatus.Draft;

    // ✨ replaced lookup with enum
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    public string? CustomerId { get; set; }         // Identity user
    public string? Notes { get; set; }
}
