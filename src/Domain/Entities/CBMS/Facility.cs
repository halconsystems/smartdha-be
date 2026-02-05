using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;
public class Facility : BaseAuditableEntity
{

    public Guid ClubCategoryId { get; set; }
    public ClubServiceCategory ClubCategory { get; set; } = default!;

    [Required, MaxLength(150)]
    public string Name { get; set; } = default!;

    [Required, MaxLength(150)]
    public string DisplayName { get; set; } = default!;

    [Required, MaxLength(30)]
    public string Code { get; set; } = default!;

    public string? ImageURL { get; set; }
    public string? Description { get; set; }

    public FoodType? FoodType { get; set; }
}

