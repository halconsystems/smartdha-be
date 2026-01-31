using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;
public class FacilityUnitImage : BaseAuditableEntity
{
    [Required]
    public Guid FacilityUnitId { get; set; }
    public FacilityUnit FacilityUnit { get; set; } = default!;

    [Required]
    public string ImageURL { get; set; } = default!;

    [Required]
    public string ImageExtension { get; set; } = default!;

    public string? ImageName { get; set; }
    public string? Description { get; set; }

    [Required]
    public ImageCategory Category { get; set; } = ImageCategory.Main;
}

