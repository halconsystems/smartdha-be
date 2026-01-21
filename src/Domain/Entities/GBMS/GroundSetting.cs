using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.GBMS;

public class GroundSetting : BaseAuditableEntity
{
    public Guid GroundId { get; set; }
    public Guid SettingId { get; set; }
    public DiscountSetting? Setting { get; set; }
    public string? DTCode { get; set; }
    public bool IsDiscount { get; set; }
}

