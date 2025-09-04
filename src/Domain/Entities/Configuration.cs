using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class Configuration : BaseAuditableEntity
{
    [Required]
    [MaxLength(100)]
    public string Key { get; set; } = default!;   // e.g. "PaymentExpiryMinutes"

    [Required]
    [MaxLength(200)]
    public string Value { get; set; } = default!; // e.g. "60"

    [MaxLength(50)]
    public string? DataType { get; set; }         // e.g. "int", "decimal", "bool"
}
