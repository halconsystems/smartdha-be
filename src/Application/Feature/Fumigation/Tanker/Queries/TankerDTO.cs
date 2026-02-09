using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.Tanker.Queries;

public class TankerDTO
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? DisplayName { get; set; }
    public string? Code { get; set; }
    public string? Price { get; set; }
    public bool? IsActive { get; set; }
    public string? ServiceId { get; set; }
    public string? ServiceName { get; set; }
}
