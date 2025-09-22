using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class PanicLocationUpdate : BaseAuditableEntity
{
    [Required]
    public Guid PanicRequestId { get; set; }
    public PanicRequest PanicRequest { get; set; } = default!;

    [Column(TypeName = "decimal(9,6)")]
    public decimal Latitude { get; set; }

    [Column(TypeName = "decimal(9,6)")]
    public decimal Longitude { get; set; }

    public float? AccuracyMeters { get; set; }

    // Always prefer UTC for server-side timestamps
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
}
