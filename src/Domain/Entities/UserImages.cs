using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class UserImages : BaseAuditableEntity
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public string ImageURL { get; set; } = default!;

    [Required]
    public string ImageExtension { get; set; } = default!;

    public string? ImageName { get; set; }
    public string? Description { get; set; }

    [Required]
    public ImageCategory Category { get; set; } = ImageCategory.Main; // default to Main
}

