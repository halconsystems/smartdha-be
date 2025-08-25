using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class OLH_VehicleOwner :BaseAuditableEntity
{
    [Required]
    public Guid VehicleOwnerId { get; set; }

    [Required]
    public string VehicleOwner { get; set; } = default!;


}
