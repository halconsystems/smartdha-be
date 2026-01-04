using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.UpdateSvVehicleLocation;
public class UpdateVehicleLocationRequest
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

