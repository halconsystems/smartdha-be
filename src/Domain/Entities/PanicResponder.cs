using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class PanicResponder : BaseAuditableEntity
{
    [Required, MaxLength(50)]
    public string Name { get; set; } = default!;

    [Required, MaxLength(13)]
    public string CNIC { get; set; } = default!;

    [Required, MaxLength(15)]
    public string PhoneNumber { get; set; } = default!;

    [Required, MaxLength(50)]
    public string? Email { get; set; }

    [Required, MaxLength(10)]
    public string? Gender { get; set; }

    // Relationship
    public Guid EmergencyTypeId { get; set; }
    public EmergencyType EmergencyType { get; set; } = default!;
}
