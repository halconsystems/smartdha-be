using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;
//public class Facility : BaseAuditableEntity
//{
//    // Example: NOC / Lease / Mutation / Site Plan
//    [Required, MaxLength(150)]
//    public string Name { get; set; } = default!;
//    public string DisplayName { get; set; } = default!;

//    [Required, MaxLength(30)]
//    public string Code { get; set; } = default!;
//    // 🔥 ADD THIS
//    public Guid ClubCategoryId { get; set; }
//    public ClubCategory ClubCategory { get; set; } = default!;

//    public string? ImageURL { get; set; }
//    public string? Description { get; set; }
//    public FoodType? FoodType { get; set; }
//    public string? Price { get; set; }
//    public bool? IsAvailable { get; set; }
//    public bool? IsPriceVisible { get; set; }
//    public bool? Action { get; set; }
//    public string? ActionName { get; set; }
//    public string? ActionType { get; set; }
//}

public class Facility : BaseAuditableEntity
{

    [Required, MaxLength(150)]
    public string Name { get; set; } = default!;

    [Required, MaxLength(150)]
    public string DisplayName { get; set; } = default!;

    [Required, MaxLength(30)]
    public string Code { get; set; } = default!;

    public Guid ClubCategoryId { get; set; }
    public ClubCategory ClubCategory { get; set; } = default!;

    public string? ImageURL { get; set; }
    public string? Description { get; set; }

    public FoodType? FoodType { get; set; }
}

