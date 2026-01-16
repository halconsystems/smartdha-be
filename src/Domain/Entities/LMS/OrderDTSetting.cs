using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.LMS;

public class OrderDTSetting : BaseAuditableEntity
{
    [Required]
    public Domain.Enums.Settings Name { get; set; } = default!;
    [Required]
    public Domain.Enums.ValueType ValueType { get; set; }
    [Required]
    public string DisplayName { get; set; } = default!;
    public string? DTCode { get; set; }
    public bool IsDiscount { get; set; }
    public string? Value { get; set; }
}
