using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Vehicles.Commands.UpdateVehicle;

public class UpdateVehicleResponse
{
    public string Message { get; set; } = "";
    public bool Success { get; set; }

}
