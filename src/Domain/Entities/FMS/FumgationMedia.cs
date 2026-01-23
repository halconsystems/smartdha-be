using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.FMS;

public class FumgationMedia : BaseAuditableEntity
{
    public Guid FemugationId { get; set; }
    public Fumigation? FemugationDispatchs { get; set; }

    public string FilePath { get; set; } = default!;   // relative path
    public FMType MediaType { get; set; }

    public string? Caption { get; set; }
}
