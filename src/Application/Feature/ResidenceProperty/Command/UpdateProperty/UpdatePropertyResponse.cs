using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.ResidenceProperty.Command.UpdateProperty;

public class UpdatePropertyResponse
{
    public string Message { get; set; } = "";
    public bool Success { get; set; }
    public Guid Id { get; set; }
}
