using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;

public class ReligonSect : BaseAuditableEntity
{
    public string? Name { get; set; }
    public string? DisplayName { get; set; }
    public string? Code { get; set; }
    public Guid ReligonId { get; set; }
    public Religion? Religons { get; set; }
}

