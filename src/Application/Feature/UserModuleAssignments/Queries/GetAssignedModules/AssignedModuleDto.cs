using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.UserModuleAssignments.Queries.GetAssignedModules;
public class AssignedModuleDto
{
    public Guid AssignmentId { get; set; }
    public Guid? ModuleId { get; set; }
    public string? ModuleName { get; set; }
    public string? ModuleDescription { get; set; }

    public bool? IsActive { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime Created { get; set; }
}
