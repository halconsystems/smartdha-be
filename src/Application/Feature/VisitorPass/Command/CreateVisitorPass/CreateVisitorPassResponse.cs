using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.VisitorPass.Command.CreateVisitorPass;

public class CreateVisitorPassResponse
{   
    public Guid Id { get; set; }
    public string Message { get; set; } = string.Empty;
}
