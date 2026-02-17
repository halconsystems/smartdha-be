using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Vehicles.Commands.CreateVehicleCommandHandler;

public class CreateVehicleResponse
{
    public string Message { get; set; } = "";
    public bool Success { get; set; }
    public Guid Id { get; set; }
}
