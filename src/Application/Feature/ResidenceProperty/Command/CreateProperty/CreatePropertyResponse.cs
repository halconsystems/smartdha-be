using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.ResidenceProperty.Command.CreateProperty;

public class CreatePropertyResponse
{
    public string Message { get; set; } = string.Empty;
    public Guid Id { get; set; } = Guid.Empty;
}

