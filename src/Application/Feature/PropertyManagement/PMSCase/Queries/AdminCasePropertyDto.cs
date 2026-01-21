using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Queries;
public class AdminCasePropertyDto
{
    public Guid PropertyId { get; set; }
    public string PropertyNo { get; set; } = default!;
    public string PlotNo { get; set; } = default!;
    public string? Sector { get; set; } = default!;
    public string? Area { get; set; }
}

