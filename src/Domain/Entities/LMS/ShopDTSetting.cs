using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.LMS;

public class ShopDTSetting : BaseAuditableEntity
{
    [Required]
    public Guid ShopId { get; set; }
    public Shops Shop { get; set; } = default!;

    [Required]
    public Guid OrderDTSettingId { get; set; }
    public OrderDTSetting OrderDTSetting { get; set; } = default!;

    [Required]
    public string Value { get; set; } = default!; // "5", "10", "18"
}
