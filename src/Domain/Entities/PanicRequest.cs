using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class PanicRequest : BaseAuditableEntity
{
    // who raised it (Identity UserId is typically string)
    [Required] public string RequestedByUserId { get; set; } = default!;

    // selected emergency type
    [Required] public Guid EmergencyTypeId { get; set; }
    public EmergencyType EmergencyType { get; set; } = default!;

    // geo
    [Column(TypeName = "decimal(9,6)")] public decimal Latitude { get; set; }
    [Column(TypeName = "decimal(9,6)")] public decimal Longitude { get; set; }
    [MaxLength(300)] public string? Address { get; set; }

    // lifecycle
    public PanicStatus Status { get; set; } = PanicStatus.Created;
    public DateTime? AcknowledgedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    [MaxLength(36)] public string? AssignedToUserId { get; set; }

    // human friendly tracking id, e.g. PAN-20250906-000123
    [Required, MaxLength(40)] public string CaseNo { get; set; } = default!;

    // optional notes/media
    [MaxLength(1000)] public string? Notes { get; set; }
    [MaxLength(500)] public string? MediaUrl { get; set; }

    // optimistic concurrency
    [Timestamp] public byte[] RowVersion { get; set; } = default!;
}
