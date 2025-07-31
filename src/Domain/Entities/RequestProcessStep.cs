using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class RequestProcessStep : BaseAuditableEntity
{
    public Guid RequestTrackingId { get; set; } // FK by convention
    public long TrackingId { get; set; }
    public string? StepName { get; set; }
    public int Sequence {  get; set; }
    public string Status { get; set; } = "Pending";  // Pending or Completed
    public string? Remarks { get; set; }
    public RequestTracking? RequestTracking { get; set; }
}

