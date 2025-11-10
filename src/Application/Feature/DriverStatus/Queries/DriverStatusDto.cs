using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.DriverStatus.Queries;
public class DriverStatusDto
{
    public Guid Id { get; set; }
    public Domain.Enums.DriverStatus Status { get; set; } = default!;
    public bool? IsActive { get; set; }
}

