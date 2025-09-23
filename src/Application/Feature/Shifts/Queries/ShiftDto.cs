using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Shifts.Queries;
public class ShiftDto
{
    public Guid Id { get; set; }
    public string ShiftName { get; set; } = default!;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool? IsActive { get; set; }
}
