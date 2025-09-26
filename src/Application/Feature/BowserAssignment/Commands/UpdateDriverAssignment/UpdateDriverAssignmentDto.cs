using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.BowserAssignment.Commands.UpdateDriverAssignment;
public class UpdateDriverAssignmentDto
{
    public Guid BowserRequestId { get; set; }
    public Guid? DriverId { get; set; }
    public Guid? VehicleId { get; set; }
    public DateTime? PlannedDeliveryDate { get; set; }  
}
