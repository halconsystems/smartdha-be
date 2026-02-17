using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities.Smartdha;

namespace DHAFacilitationAPIs.Application.Feature.ResidenceProperty.Queries.GetPropertyById;

public class GetPropertyByIdResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public ResidentProperty? Data { get; set; }
}

