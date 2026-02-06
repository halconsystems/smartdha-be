using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.MemberShipRequest.Queries;

public class MemberRequestHistoryDTO
{
    public Guid Id { get; set; }
    public string? MemberShipCategory { get; set; }
    public string? Purpose { get; set; }

    public string? CNIC { get; set; } = default!;
    public bool? LiveTime { get; set; }
    public DateTime? CnicExpiryDate { get; set; }
    public DateTime? Dob { get; set; }
    public string? Name { get; set; }
    public Gender? Gender { get; set; }
    public MaritalStatus? MaritalStatus { get; set; }
    public VerificationStatus? status { get; set; }
    public string? NTNno { get; set; }
    public string? PassportNo { get; set; }
    public string? PassportPlace { get; set; }
    public DateTime? PassportExpiry { get; set; }

    public string? Religion { get; set; } 
    public string? ReligionSect { get; set; } 
    public string? OtherReligion { get; set; }
    public Nationality? Nationality { get; set; }
    public string? OtherNationality { get; set; }

    public string? CNICFrontImagePath { get; set; }

    public string? CNICBackImagePath { get; set; }
    public string? PicturePath { get; set; } // optional 
    public string? Qualification { get; set; }
    public Profession? Profession { get; set; }

    public string? FatherName { get; set; }
    public bool? FatherAlive { get; set; }

    public string? FatherCnic { get; set; }
    public string? FatherMobileNo { get; set; }
    public string? FatherCNICFrontImagePath { get; set; }

    public string? FatherCNICBackImagePath { get; set; }
    public string? FatherPicturePath { get; set; }

    public string? MotherName { get; set; }
    public bool? MotherAlive { get; set; }

    public string? MotherCnic { get; set; }
    public string? MotherMobileNo { get; set; }
    public string? MotherCNICFrontImagePath { get; set; }

    public string? MotherCNICBackImagePath { get; set; }

    public string? MotherPicturePath { get; set; } // optional 

    public bool IsChild { get; set; }
    public string? Email { get; set; }
    public string? TelephoneResidence { get; set; }
    public string? MobileNo1 { get; set; }
    public string? MobileNo2 { get; set; }
    public string? PresentAddress { get; set; }
    public string? PresentCity { get; set; }
    public string? PresentCountry { get; set; }
    public string? PermenantAddress { get; set; }
    public string? PermenantCity { get; set; }
    public string? PermenantCountry { get; set; }

    public bool BestKnowledge { get; set; }
    public bool Rservation { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsDelete { get; set; }

    public List<MemberChildrenDTO>? MemberChildrens { get; set; }
    public List<MemberSpouseDTO>? MemberSpouses { get; set; }

}
public class MemberPersonalDTO
{
    public Guid Id { get; set; }
    public string? MemberShipCategory { get; set; }
    public string? Purpose { get; set; }

    public string? CNIC { get; set; } = default!;
    public bool? LiveTime { get; set; }
    public DateTime? CnicExpiryDate { get; set; }
    public DateTime? Dob { get; set; }
    public string? Name { get; set; }
    public Gender? Gender { get; set; }
    public MaritalStatus? MaritalStatus { get; set; }
    public VerificationStatus? status { get; set; }
    public string? NTNno { get; set; }
    public string? PassportNo { get; set; }
    public string? PassportPlace { get; set; }
    public DateTime? PassportExpiry { get; set; }

    public string? Religion { get; set; } 
    public string? ReligionSect { get; set; } 
    public string? OtherReligion { get; set; }
    public Nationality? Nationality { get; set; }
    public string? OtherNationality { get; set; }

    public string? CNICFrontImagePath { get; set; }

    public string? CNICBackImagePath { get; set; }
    public string? PicturePath { get; set; } // optional 
    public string? Qualification { get; set; }
    public Profession? Profession { get; set; }

    public string? FatherName { get; set; }
    public bool? FatherAlive { get; set; }

    public string? FatherCnic { get; set; }
    public string? FatherMobileNo { get; set; }
    public string? FatherCNICFrontImagePath { get; set; }

    public string? FatherCNICBackImagePath { get; set; }
    public string? FatherPicturePath { get; set; }

    public string? MotherName { get; set; }
    public bool? MotherAlive { get; set; }

    public string? MotherCnic { get; set; }
    public string? MotherMobileNo { get; set; }
    public string? MotherCNICFrontImagePath { get; set; }

    public string? MotherCNICBackImagePath { get; set; }

    public string? MotherPicturePath { get; set; } // optional 

    public bool IsChild { get; set; }
    public string? Email { get; set; }
    public string? TelephoneResidence { get; set; }
    public string? MobileNo1 { get; set; }
    public string? MobileNo2 { get; set; }
    public string? PresentAddress { get; set; }
    public string? PresentCity { get; set; }
    public string? PresentCountry { get; set; }
    public string? PermenantAddress { get; set; }
    public string? PermenantCity { get; set; }
    public string? PermenantCountry { get; set; }

    public bool BestKnowledge { get; set; }
    public bool Rservation { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsDelete { get; set; }

    public List<MemberChildrenDTO>? MemberChildrens { get; set; }
    public List<MemberSpouseDTO>? MemberSpouses { get; set; }

}

public class MemberShipDataDTO
{
    public Guid Id { get; set; }
    public string? MemberShipCategory { get; set; }
    public string? Purpose { get; set; }
}

public class PersonalParticularDTO
{
    public string? personalNo { get; set; }
    public Ranks? Rank { get; set; }
}
