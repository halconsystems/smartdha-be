using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class SvPoint : BaseAuditableEntity
{
    // e.g. "POINT-01", "PHASE-6-GATE"
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? Address { get; set; } 
    // For map
    public double Latitude { get; set; }   
    public double Longitude { get; set; }  
    public ICollection<SvVehicle> Vehicles { get; set; } = new List<SvVehicle>();
}
