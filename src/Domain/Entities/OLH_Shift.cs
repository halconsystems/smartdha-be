using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class OLH_Shift:BaseAuditableEntity
{
    public Guid ShiftId { get; set; }

    public string Shift { get; set; } = default!;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }



}
