using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class RefundRequest : BaseAuditableEntity
{
    [Required]
    public Guid ReservationId { get; set; }
    public Reservation Reservation { get; set; } = default!;

    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateOnly RequestedAtDateOnly { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public TimeOnly RequestedAtTimeOnly { get; set; } = TimeOnly.FromDateTime(DateTime.UtcNow);

    [Column(TypeName = "decimal(18,2)")]
    public decimal AmountPaid { get; set; }  // Amount initially paid against the reservation

    [Column(TypeName = "decimal(18,2)")]
    public decimal RefundableAmount { get; set; } // Amount refunded as per club policy

    public RefundStatus Status { get; set; } = RefundStatus.Pending;

    public string? Notes { get; set; } // admin comments
}

