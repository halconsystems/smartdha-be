using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.AssignPanicToVehicle;
public class AssignPanicToVehicleRequest
{
    public Guid VehicleId { get; set; }
    public string? ControlRoomRemarks { get; set; }
}

