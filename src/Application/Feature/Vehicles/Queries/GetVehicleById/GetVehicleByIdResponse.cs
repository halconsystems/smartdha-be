using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities.Smartdha;

namespace DHAFacilitationAPIs.Application.Feature.Vehicles.Queries.GetVehicleById;

public class GetVehicleByIdResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public Vehicle? Data { get; set; }
}
