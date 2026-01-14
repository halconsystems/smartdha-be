using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;

public class MemberSpouse : BaseAuditableEntity
{
    public Guid MemberShipId { get; set; }
    public MemberRequest? MemberRequest { get; set; }

    [Required]
    public string FullName { get; set; } = default!;
    public string? MobileNo { get; set; }
    public string? Email { get; set; }
    [MaxLength(500)]
    public string? PicturePath { get; set; }
    public Nationality Nationality { get; set; }
    public string? otherNationality { get; set; }

    [MaxLength(500)]
    public string? CnicFrontImage { get; set; }
    [MaxLength(500)]
    public string? CnicBackImage { get; set; }
    public string Cnic { get; set; } = default!;
    public DateTime CnicExpiry { get; set; } = default!;

    

}

