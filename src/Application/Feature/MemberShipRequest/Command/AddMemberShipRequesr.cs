using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.MemberShipRequest.Queries;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.MemberShipRequest.Command;

public class CreateMemberShipRequestCommand : IRequest<SuccessResponse<string>>
{
    public Guid MemberShipCategoryId { get; set; }
    [Required]
    public string Cnic { get; set; } = default!;
    public bool LifeTime { get; set; }
    public DateTime? CnicExpiry { get; set; }
    [Required]
    public DateTime DOB { get; set; } = default!;
    [Required]
    public string Name { get; set; } = default!;
    [Required]
    public Gender Gender { get; set; } = default!;
    [Required]
    public MaritalStatus MaritalStatus { get; set; } = default!;
    public string? NTNno { get; set; }
    public string? PassportNo { get; set; }
    public string? PassportPlace { get; set; }
    public DateTime? PassportExpiry { get; set; }
    [Required]
    public Guid ReligonSect { get; set; } = default!;
    public string? otherReligon { get; set; }
    [Required]
    public Nationality Nationality { get; set; } = default!;
    public string? otherNationality { get; set; }
    [Required]
    public IFormFile profilePicture { get; set; } = default!;

    [Required]
    public IFormFile CnicFrontPicture { get; set; } = default!;

    [Required]
    public IFormFile CnicBackPicture { get; set; } = default!;

    public string? Qualification { get; set; }
    public Profession? Profession { get; set; }

    [Required]
    public string FatherName { get; set; } = default!;
    public bool FatherAlive { get; set; }

    public string? FatherCnic { get; set; }
    public string? FatherMobileNo { get; set; }
    public IFormFile? FatherCNICFrontImagePath { get; set; }

    public IFormFile? FatherCNICBackImagePath { get; set; }

    public IFormFile? FatherPicturePath { get; set; } // optional = default!;

    [Required]
    public string MotherName { get; set; } = default!;
    public bool MotherAlive { get; set; }

    public string? MotherCnic { get; set; }
    public string? MotherMobileNo { get; set; }
    public IFormFile? MotherCNICFrontImagePath { get; set; }

    public IFormFile? MotherCNICBackImagePath { get; set; }

    public IFormFile? MotherPicturePath { get; set; }

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

    public List<MemberSpouseDTO>? MemberSpouses { get; set; }
    public List<MemberChildrenDTO>? MemberChildrens { get; set; }

}
public class CreateMemberShipRequestCommandHandler : IRequestHandler<CreateMemberShipRequestCommand, SuccessResponse<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _fileStorage;

    public CreateMemberShipRequestCommandHandler(IApplicationDbContext context, IFileStorageService fileStorageService, ISmsService otpService)
    {
        _context = context;
        _fileStorage = fileStorageService;
    }

    public async Task<SuccessResponse<string>> Handle(CreateMemberShipRequestCommand command, CancellationToken ct)
    {
        var FatherfrontPath = string.Empty;
        var FatherbackPath = string.Empty;
        var FatherprofilePicture = string.Empty;
        var motherfrontPath = string.Empty;
        var motherbackPath = string.Empty;
        var motherprofilePicture = string.Empty;
        var frontPath = await _fileStorage.SaveFileMemeberrequestAsync(command.CnicFrontPicture, "cnic", ct);
        var backPath = await _fileStorage.SaveFileMemeberrequestAsync(command.CnicBackPicture, "cnic", ct);
        var profilePicture = await _fileStorage.SaveFileMemeberrequestAsync(command.profilePicture, "cnic", ct);

        if(command.FatherPicturePath != null && command.FatherCNICFrontImagePath != null && command.FatherCNICBackImagePath != null)
        {
            FatherfrontPath = await _fileStorage.SaveFileMemeberrequestAsync(command.FatherCNICFrontImagePath, "cnic", ct);
            FatherbackPath = await _fileStorage.SaveFileMemeberrequestAsync(command.FatherCNICBackImagePath, "cnic", ct);
            FatherprofilePicture = await _fileStorage.SaveFileMemeberrequestAsync(command.FatherPicturePath, "cnic", ct);
        }

        if (command.MotherPicturePath != null && command.MotherCNICFrontImagePath != null && command.MotherCNICBackImagePath != null)
        {
            motherfrontPath = await _fileStorage.SaveFileMemeberrequestAsync(command.MotherCNICFrontImagePath, "cnic", ct);
            motherbackPath = await _fileStorage.SaveFileMemeberrequestAsync(command.MotherCNICBackImagePath, "cnic", ct);
            motherprofilePicture = await _fileStorage.SaveFileMemeberrequestAsync(command.MotherPicturePath, "cnic", ct);
        }
        //var backPath = await _fileStorage.SaveFileMemeberrequestAsync(command.CNICBackImage, "cnic", ct);


        var entity = new MemberRequest
        {
            MemberShipCategory = command.MemberShipCategoryId,
            CNIC = command.Cnic,
            LiveTime = command.LifeTime,
            CnicExpiryDate = command.CnicExpiry,
            Dob = command.DOB,
            Name = command.Name,
            Gender = command.Gender,
            MaritalStatus = command.MaritalStatus,
            NTNno = command.NTNno,
            PassportNo = command.PassportNo,
            PassportPlace = command.PassportPlace,
            PassportExpiry = command.PassportExpiry,
            ReligonSectId = command.ReligonSect,
            OtherReligion = command.otherReligon,
            Nationality = command.Nationality,
            OtherNationality = command.otherNationality,
            Qualification = command.Qualification,
            Profession = command.Profession,
            FatherName = command.FatherName,
            FatherAlive = command.FatherAlive,
            FatherCnic = command.FatherCnic,
            FatherMobileNo = command.FatherMobileNo,
            MotherName = command.MotherName,
            MotherAlive = command.MotherAlive,
            MotherCnic = command.MotherCnic,
            MotherMobileNo = command.MotherMobileNo,
            Email = command.Email,
            TelephoneResidence = command.TelephoneResidence,
            MobileNo1 = command.MobileNo1,
            MobileNo2 = command.MobileNo2,
            PresentAddress = command.PresentAddress,
            PresentCity = command.PresentCity,
            PresentCountry = command.PresentCountry,
            PermenantAddress = command.PermenantAddress,
            PermenantCity = command.PermenantCity,
            PermenantCountry = command.PermenantCountry,
            BestKnowledge = command.BestKnowledge,
            Rservation = command.Rservation,
            IsChild = command.IsChild,
            

            PicturePath = profilePicture,
            CNICFrontImagePath = frontPath,
            CNICBackImagePath = backPath,

            FatherPicturePath = FatherprofilePicture,
            FatherCNICFrontImagePath = FatherfrontPath,
            FatherCNICBackImagePath = FatherbackPath,

            MotherPicturePath = motherprofilePicture,
            MotherCNICFrontImagePath = motherfrontPath,
            MotherCNICBackImagePath = motherbackPath,
        };

        _context.MemberRequests.Add(entity);
        await _context.SaveChangesAsync(ct);
        if (command.MemberSpouses != null && command.MaritalStatus == MaritalStatus.Married)
        {
            var memberSpouse = command.MemberSpouses.Select(x => new MemberSpouse
            {
                MemberShipId = entity.Id,
                FullName = x.FullName,
                MobileNo = x.MobileNo,
                Email = x.Email,
                Nationality = x.Nationality,
                otherNationality = x.otherNationality,
                PicturePath =  _fileStorage.SaveFileMemeberrequestAsync(x.PicturePath, "cnic", ct).ToString(),
                CnicFrontImage = _fileStorage.SaveFileMemeberrequestAsync(x.CnicFrontImage, "cnic", ct).ToString(),
                CnicBackImage = _fileStorage.SaveFileMemeberrequestAsync(x.CnicBackImage, "cnic", ct).ToString(),
                Cnic = x.Cnic,
                CnicExpiry = x.CnicExpiry,
            }).ToList();

            _context.MemberSpouses.AddRange(memberSpouse);
            await _context.SaveChangesAsync(ct);
        }

        if(command.MemberChildrens != null && command.IsChild)
        {
            var memberChildrens = command.MemberChildrens.Select(x => new MemberChildren
            {
                MemberShipId = entity.Id,
                FullName = x.FullName,
                MobileNo = x.MobileNo,
                Relation = x.Relation,
                IsAdult = x.IsAdult,
                CnicNo = x.CnicNo,
                PicturePath = _fileStorage.SaveFileMemeberrequestAsync(x.PicturePath, "cnic", ct).ToString(),
                CNICFrontImagePath = _fileStorage.SaveFileMemeberrequestAsync(x.CNICFrontImagePath, "cnic", ct).ToString(),
                CNICBackImagePath = _fileStorage.SaveFileMemeberrequestAsync(x.CNICBackImagePath, "cnic", ct).ToString(),
                CnicExpiryDate = x.CnicExpiryDate,
            }).ToList();
            _context.MemberChildrens.AddRange(memberChildrens);
            await _context.SaveChangesAsync(ct);

        }

        return Success.Created(entity.Id.ToString());
    }
}
