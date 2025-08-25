using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class OLH_VehicleType:BaseAuditableEntity
{
    [Required]
    public Guid VehicleTypeId { get; set; }

    [Required]
    public string VehilceType { get; set; }= default!;

}
