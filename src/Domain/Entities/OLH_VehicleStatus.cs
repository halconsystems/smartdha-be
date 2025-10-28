using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class OLH_VehicleStatus : BaseAuditableEntity
{
    [Required] public VehicleStatus Status { get; set; } = VehicleStatus.Active; // Active/Maintenance/Inactive
    public string? Remarks { get; set; }
}
