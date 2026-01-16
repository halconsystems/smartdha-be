using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;

public class DiscountSetting : BaseAuditableEntity
{
    public Settings Name { get; set; }
    public Enums.ValueType ValueType { get; set; }
    public string? Value { get; set; }
    public string? Code { get; set; }
    public string? DisplayName { get; set; }
    public bool IsDiscount { get; set; }
}
