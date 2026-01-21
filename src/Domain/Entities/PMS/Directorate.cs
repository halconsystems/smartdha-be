using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.PMS;
public class Directorate : BaseAuditableEntity
{
    // Example: CC, Land, TP&BC, Finance
    [Required, MaxLength(100)]
    public string Name { get; set; } = default!;

    // Short code like "CC", "LAND", "TPBC", "FIN"
    [Required, MaxLength(20)]
    public string Code { get; set; } = default!;
    public Guid ModuleId { get; set; } = default!;
    }
