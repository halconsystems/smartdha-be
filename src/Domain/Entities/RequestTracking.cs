using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class RequestTracking : BaseAuditableEntity
{

    public string? MemPK { get; set; }
    public string? CNIC { get; set; }

    [Required]
    public long TrackingId { get; set; }  // Should be unique


    [Required]
    public string Status { get; set; } = "Pending";  // Pending, Approved, Rejected

    public string? Remarks { get; set; }
    public string? Description { get; set; }
    public int? ProcessId { get; set; }
    public string PLot_ID { get; set; } = string.Empty;
    public string PLOT_NO { get; set; } = string.Empty;

    public List<RequestProcessStep> ProcessSteps { get; set; } = new();
}

