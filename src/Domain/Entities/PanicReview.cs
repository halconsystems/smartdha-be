using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class PanicReview : BaseAuditableEntity
{
    // FK → Panic
    public Guid PanicRequestId { get; set; }
    public PanicRequest PanicRequest { get; set; } = default!;
    // Rating: 1–5
    public int Rating { get; set; }
    // Optional feedback
    public string? ReviewText { get; set; }
}

