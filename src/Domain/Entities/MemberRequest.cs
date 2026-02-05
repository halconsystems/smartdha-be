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
    public Ranks? Ranks { get; set; }
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

    //Military Service
    public string? Serving { get; set; }
    public string? ServiceNo { get; set; }
    public string? ArmsSvc { get; set; }
    public CauseRetirement? CauseRetirement { get; set; }

    [Required]
    [MaxLength(500)]
    public string CNICFrontImagePath { get; set; } = default!;

    [Required]
    [MaxLength(500)]
    public string? CNICBackImagePath { get; set; } = default!;

    [Required]
    public string PicturePath { get; set; } = default!;
    [MaxLength(500)]
    public string? NonPassportCopy {  get; set; }
    public string? Qualification { get; set; }
    public Profession? Profession { get; set; }

    [Required]
    public string FatherName { get; set; } = default!;
    public bool FatherAlive { get; set; }

    public string? FatherCnic { get; set; }
    public string? FatherMobileNo { get; set; }
    public string? FatherCNICFrontImagePath { get; set; }

    public string? FatherCNICBackImagePath { get; set; }

    public string? FatherPicturePath { get; set; } // optional 

    [Required]
    public string MotherName { get; set; } = default!;
    public bool MotherAlive { get; set; }

    public string? MotherCnic { get; set; }
    public string? MotherMobileNo { get; set; }
    
    public string? MotherCNICFrontImagePath { get; set; }

    public string? MotherCNICBackImagePath { get; set; }

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
    public string? PresentCountry { get; set; }

    [Required]
    public string PermenantAddress { get; set; } = default!;
    [Required]
    public string PermenantCity { get; set; } = default!;
    public string? PermenantCountry { get; set; }

    public bool BestKnowledge { get; set; }
    public bool Rservation { get; set; }

    public VerificationStatus Status { get; set; } = VerificationStatus.Pending;

    public DateOnly ValidUntil { get; set; }

}

