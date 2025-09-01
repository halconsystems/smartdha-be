using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class OLH_VehicleStatus:BaseAuditableEntity
{
    [Required]
    public Guid VehicleStatusID { get; set; }
    [Required]    
    public string VehicleStatus { get; set; } = default!;

    public string Remarks { get; set; } = default!;
}
