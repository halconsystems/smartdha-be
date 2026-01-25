using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;

public class ClubPrerequisiteDefinitions : BaseAuditableEntity
{
    // Example: "CNIC Front", "NADRA Verification", "Site Plan PDF"
    [Required, MaxLength(150)]
    public string Name { get; set; } = default!;

    // Unique code for API/UI mapping
    [Required, MaxLength(50)]
    public string Code { get; set; } = default!;

    public PrerequisiteType Type { get; set; }

    // For text/number limits
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }

    // If it is a file, you can restrict extensions (".pdf,.jpg")
    [MaxLength(200)]
    public string? AllowedExtensions { get; set; }
}
