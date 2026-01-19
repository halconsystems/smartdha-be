using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.OrderTaxDiscount.Queries;

public class OrderDTSettingDTO
{
    public Guid Id {  get; set; }
    public Domain.Enums.Settings Name { get; set; }
    [Required]
    public Domain.Enums.ValueType ValueType { get; set; }
    public string? DisplayName { get; set; }
    public string? DTCode { get; set; }
    public bool IsDiscount { get; set; }
    public string? Value { get; set; }
}
