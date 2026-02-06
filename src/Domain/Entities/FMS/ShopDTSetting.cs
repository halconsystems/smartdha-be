using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities.LMS;

namespace DHAFacilitationAPIs.Domain.Entities.FMS;

public class ShopDTSetting : BaseAuditableEntity
{

    [Required]
    public Guid ShopId { get; set; }
    public FemgutionShops Shop { get; set; } = default!;

    [Required]
    public Guid FemDTSettingId { get; set; }
    public FemDTSetting FemDTSetting { get; set; } = default!;

    [Required]
    public string Value { get; set; } = default!; // "5", "10", "18"
}
