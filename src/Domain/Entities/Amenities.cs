using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;

[Table("Tbl_Amenities")]
public class Amenities : BaseAuditableEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AmenitiesID { get; set; }  // This is the identity column in SQL, not your GUID PK

    [Required]
    [MaxLength(255)]
    public string AmenitiesName { get; set; } = default!;  // Renamed for clarity, matches "Amenities" column

    [MaxLength(500)]
    public string? IconPath { get; set; }

    [MaxLength(1000)]
    public string? Remarks { get; set; }
}

