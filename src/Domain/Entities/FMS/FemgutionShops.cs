using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.FMS;

public class FemgutionShops :BaseAuditableEntity
{
    [Required]
    public string Name { get; set; } = default!;
    [Required]
    public string DisplayName { get; set; } = default!;
    public string? Code { get; set; }
    [Required]
    public string Address { get; set; } = default!;
    [Required]
    public float? Latitude { get; set; } = default!;
    [Required]
    public float? Longitude { get; set; } = default!;
    [Required]
    public string OwnerName { get; set; } = default!;
    [Required]
    public string OwnerEmail { get; set; } = default!;
    [Required]
    public string OwnerPhone { get; set; } = default!;
    [Required]
    public string ShopPhoneNumber { get; set; } = default!;
}
