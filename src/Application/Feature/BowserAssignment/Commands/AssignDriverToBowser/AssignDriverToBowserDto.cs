using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.BowserAssignment.Commands.AssignDriverToBowser;
public class AssignDriverToBowserDto
{
    public Guid BowserRequestId { get; set; }
    public Guid DriverId { get; set; }
    public Guid VehicleId { get; set; }
}
