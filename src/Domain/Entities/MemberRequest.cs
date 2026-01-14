using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;

public class MemberRequest : BaseAuditableEntity
{
    public Guid MemberShipCategory { get; set; }
    public MemberShipCatergories? MemberShipCatergories { get; set; }

    [Required]
    public string CNIC { get; set; } = default!;
    public bool LiveTime { get; set; }
    public DateTime? CnicExpiryDate { get; set; }
    public DateTime Dob { get; set; }
    [Required]
    public string Name { get; set; } = default!;
    public Gender Gender { get; set; }
    public MaritalStatus MaritalStatus { get; set; }
    public string? NTNno { get; set; }
    public string? PassportNo { get; set; }
    public string? PassportPlace { get; set; }
    public DateTime? PassportExpiry { get; set; }

    public Guid ReligonSectId { get; set; }
    [Required]
    public ReligonSect ReligonSect { get; set; } = default!;
    public string? OtherReligion { get; set; }
    public Nationality Nationality { get; set; }
    public string? OtherNationality { get; set; }

    [Required]
    [MaxLength(500)]
    public string CNICFrontImagePath { get; set; } = default!;

    [Required]
    [MaxLength(500)]
    public string? CNICBackImagePath { get; set; } = default!;

    [Required]
    [MaxLength(500)]
    public string? PicturePath { get; set; } // optional 
    public string? Qualification { get; set; }
    public Profession? Profession { get; set; }

    [Required]
    public string FatherName { get; set; } = default!;
    public bool FatherAlive { get; set; }

    public string? FatherCnic { get; set; }
    public string? FatherMobileNo { get; set; }
    [Required]
    [MaxLength(500)]
    public string FatherCNICFrontImagePath { get; set; } = default!;

    [MaxLength(500)]
    public string? FatherCNICBackImagePath { get; set; }

    [Required]
    [MaxLength(500)]
    public string? FatherPicturePath { get; set; } // optional 

    [Required]
    public string MotherName { get; set; } = default!;
    public bool MotherAlive { get; set; }

    public string? MotherCnic { get; set; }
    public string? MotherMobileNo { get; set; }
    [Required]
    [MaxLength(500)]
    public string MotherCNICFrontImagePath { get; set; } = default!;

    [MaxLength(500)]
    public string? MotherCNICBackImagePath { get; set; }

    [Required]
    [MaxLength(500)]
    public string? MotherPicturePath { get; set; } // optional 

    public bool IsChild { get; set; }
    [Required]
    public string Email { get; set; } = default!;
    public string? TelephoneResidence { get; set; }
    [Required]
    public string MobileNo1 { get; set; } = default!;
    public string? MobileNo2 { get; set; }

    [Required]
    public string PresentAddress { get; set; } = default!;
    [Required]
    public string PresentCity { get; set; } = default!;
    public string PresentCountry { get; set; } = default!;

    [Required]
    public string PermenantAddress { get; set; } = default!;
    [Required]
    public string PermenantCity { get; set; } = default!;
    public string PermenantCountry { get; set; } = default!;

    public bool BestKnowledge { get; set; }
    public bool Rservation { get; set; }

}

