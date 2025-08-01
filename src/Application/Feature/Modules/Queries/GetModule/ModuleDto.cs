using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Modules.Queries.GetModule;
public class ModuleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Remarks { get; set; } = default!;
    public AppType AppType { get; set; }
}
