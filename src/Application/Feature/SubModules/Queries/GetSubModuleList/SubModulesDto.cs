using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.SubModules.Queries.GetSubModuleList;
public class SubModulesDto
{
    public Guid Id { get; set; }
    public string Value { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public bool RequiresPermission { get; set; }
    public Guid? ModuleId { get; set; }


}
