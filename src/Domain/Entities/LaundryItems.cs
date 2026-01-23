using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;

public class LaundryItems : BaseAuditableEntity
{
    [Required]
    public string Name { get; set; } = default!;
    [Required]
    public string DisplayName { get; set; } = default!;
    public string? Code { get; set; }
    public string? ItemPrice { get; set; }
    public Guid CategoryId { get; set; }
    public LaundryCategory? LaundryCategory { get; set; }
    public string? ItemImage { get; set; }

}
