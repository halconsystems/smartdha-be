using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.ReligonSect.Queries;

public class ReligonSectDTO
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? DisplayName { get; set; }
    public string? Code { get; set; }
    public Guid ReligonId { get; set; }
}

