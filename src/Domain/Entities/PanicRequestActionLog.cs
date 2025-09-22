using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class PanicRequestActionLog : BaseAuditableEntity
{
    [Required] public Guid PanicRequestId { get; set; }
    public PanicRequest PanicRequest { get; set; } = default!;

    [Required] public string ActionByUserId { get; set; } = default!;
    [Required, MaxLength(50)] public string Action { get; set; } = default!; // e.g. "ACK", "ASSIGN", "RESOLVE", "CANCEL"
    [MaxLength(1000)] public string? Remarks { get; set; }

    public PanicStatus FromStatus { get; set; }
    public PanicStatus ToStatus { get; set; }
}

