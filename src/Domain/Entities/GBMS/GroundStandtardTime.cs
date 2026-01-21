using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.GBMS;

public class GroundStandtardTime : BaseAuditableEntity
{
    public Guid GroundId { get; set; }
    public Grounds? Grounds { get; set; }
    [Required] public TimeOnly CheckInTime { get; set; }
    [Required] public TimeOnly CheckOutTime { get; set; }
}
