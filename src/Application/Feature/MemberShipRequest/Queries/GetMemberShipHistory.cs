using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.MemberShipRequest.Queries;

public record GetMemberShipHistoryQuery(Guid Id) : IRequest<MemberShipPrintDTO>;
public class GetMemberShipHistoryQueryHandler : IRequestHandler<GetMemberShipHistoryQuery, MemberShipPrintDTO>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _fileStorageService;

    public GetMemberShipHistoryQueryHandler(IApplicationDbContext context,IFileStorageService fileStorageService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
    }

    public async Task<MemberShipPrintDTO> Handle(GetMemberShipHistoryQuery request, CancellationToken ct)
    {
        var memberRequest = await _context.MemberRequests
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        if (memberRequest == null) throw new KeyNotFoundException("Member Request Not Fount");

        var memberSpouses = await _context.MemberSpouses
            .Where(x => x.MemberShipId == memberRequest.Id)
            .AsNoTracking()
            .ToListAsync(ct);

        var memberChildren = await _context.MemberChildrens
            .Where(x => x.MemberShipId == memberRequest.Id)
            .AsNoTracking().ToListAsync(ct);    

        var MemberShipCatergories = await _context.MemberShipCatergories.FirstOrDefaultAsync(x => x.Id == memberRequest.MemberShipCategory,ct);

        if (MemberShipCatergories == null) throw new KeyNotFoundException("Member Category Request Not Fount");


        var MemberShipPurpose = await _context.MemberShips.FirstOrDefaultAsync(x => x.Id == MemberShipCatergories.MemberShipId,  ct);


        var MemberReligonSect = await _context.ReligonSects.FirstOrDefaultAsync(x => x.Id == memberRequest.ReligonSectId, ct);

        if (MemberReligonSect == null) throw new KeyNotFoundException("Religon Section  Request Not Fount");

        var MemberReligon = await _context.Religions.FirstOrDefaultAsync(x => x.Id == MemberReligonSect.ReligonId, ct);


        var result = new MemberShipPrintDTO
        {
            MemberShipDataDTO = new MemberShipDataDTO
            {
                Id = memberRequest.Id,
                MemberShipCategory = MemberShipCatergories.displayname,
                Purpose = MemberShipPurpose?.DisplayName,
                VerificationStatus = memberRequest.Status,
            },
            PersonalParticular = new MemberPersonalDTO
            {
                PersonalNo = memberRequest.ServiceNo,
                Ranks = memberRequest.Ranks,
                Serving = memberRequest.Serving,
                SOD = null,
                CauseRetirement = memberRequest.CauseRetirement,
                MaritalStatus = memberRequest.MaritalStatus,
                CNIC = memberRequest.CNIC,
                PassportNo = memberRequest.PassportNo,
                Religion = MemberReligon?.DisplayName,
                ReligionSect = MemberReligonSect?.DisplayName,
                KhandanNo = string.Empty,
                Dob = DateOnly.FromDateTime(memberRequest.Dob),
                VisibleMark = string.Empty,
                Domicile = string.Empty,
                Nationality = memberRequest.Nationality,
            },
            PersonalInformation = new PersonalInformationDTO
            {
                Qualification = memberRequest.Qualification,
                AddressOrganization = string.Empty,
                Profession = memberRequest.Profession
            },
            Address = new AddressDTO
            {
                PresentAddress = memberRequest.PresentAddress,
                PresentCity = memberRequest.PresentCity,
                PresentCountry  = memberRequest.PresentCountry,
                PermenantAddress = memberRequest.PermenantAddress,
                PermenantCity = memberRequest.PermenantCity,
                PermenantCountry = memberRequest.PermenantCountry,
                MailingAddress = memberRequest.PermenantAddress,
                Email = memberRequest.Email
            },

            Contact = new ContactDTO
            {
                TelephoneResidence = memberRequest.TelephoneResidence,
                TelephoneOff = memberRequest.TelephoneResidence,
                MobileNo1 = memberRequest.MobileNo1,
                Fax = string.Empty,
                Other = string.Empty
            },
            Parents = new ParentsDTO
            {
                FatherName = memberRequest.FatherName,
                FatherCnic = memberRequest.FatherCnic,
                FatherMobileNo = memberRequest.FatherMobileNo,
                FatherPicturePath = !string.IsNullOrWhiteSpace(memberRequest.FatherPicturePath)
                ? _fileStorageService.GetPublicUrl(memberRequest.FatherPicturePath)
                : null,
                MotherName = memberRequest.MotherName,
                MotherCnic = memberRequest.MotherCnic,
                MotherMobileNo = memberRequest.MotherMobileNo,
                MotherPicturePath = !string.IsNullOrWhiteSpace(memberRequest.MotherPicturePath)
                ? _fileStorageService.GetPublicUrl(memberRequest.MotherPicturePath)
                : null,
            },
            Spouses = memberSpouses.Select(s => new SpouseDTO
            {
                
                Name = s.FullName,
                Cnic = s.Cnic,
                MobileNo = s.MobileNo,
                PicturePath = !string.IsNullOrWhiteSpace(s.PicturePath)
                    ? _fileStorageService.GetPublicUrl(s.PicturePath)
                    : null
            }).ToList(),
            Children = memberChildren.Select(s => new ChildDTO
            {
                
                Name = s.FullName,
                Cnic = s.CnicNo,
                MobileNo = s.MobileNo,
                PicturePath = !string.IsNullOrWhiteSpace(s.PicturePath)
                    ? _fileStorageService.GetPublicUrl(s.PicturePath)
                    : null,
                IsAdult = s.IsAdult,
            }).ToList(),

            Reference = new List<ReferenceDTO>
            {
                new ReferenceDTO
                {
                    Name = string.Empty,
                    Cnic = string.Empty,
                    MemNo = string.Empty,
                    Address = string.Empty,
                    SO = string.Empty,
                }
            }.ToList(),

            Coperate = new CoperateDTO
            {
                NameOrganization = string.Empty,
                RegistrationNo = string.Empty,
                HeadName = string.Empty,
                OgranizationType = Domain.Enums.OgranizationType.Other,
                BusinessNature = string.Empty,
                CompanyType = Domain.Enums.CompanyType.PatnerSHip,
                RegisteredScep = false,
                BoardDirectorName = new List<string>(),

            },
            OperatingPerson = new OperatingPersonDTO
            {
                Name = string.Empty,
                Cnic = string.Empty,
                Nicop = string.Empty,
                PassportNo = string.Empty,
                Designation = string.Empty,
                Address = string.Empty,
                ContactNoMobile = string.Empty,
                ContactNoOffice = string.Empty,
                ContactNoResidence  = string.Empty,
            },
            LegalReference = new LegalReference
            {
                PlotNo = string.Empty,
                MemNo = string.Empty,
                Relation = string.Empty,
                Remarks = string.Empty,
            },
            ForeignCoperateReference = new ForeignCoperateReference
            {
                PlotNo = string.Empty,
                MemNo = string.Empty,
                Relation = string.Empty,
                Remarks = string.Empty,
            }
        };

        //var result = new MemberRequestHistoryDTO
        //{
        //    Id = memberRequest.Id,
        //    Name = memberRequest.Name,
        //    MemberShipCategory = MemberShipCatergories.displayname,
        //    Purpose = MemberShipPurpose?.DisplayName,
        //    CNIC = memberRequest.CNIC,
        //    Email = memberRequest.Email,
        //    IsActive = memberRequest.IsActive,
        //    IsDelete = memberRequest.IsDeleted,
        //    LiveTime = memberRequest.LiveTime,
        //    CnicExpiryDate = memberRequest.CnicExpiryDate,
        //    Dob = memberRequest.Dob,
        //    Gender = memberRequest.Gender,
        //    MaritalStatus = memberRequest.MaritalStatus,
        //    NTNno = memberRequest.NTNno,
        //    PassportNo = memberRequest.PassportNo,
        //    PassportPlace = memberRequest.PassportPlace,
        //    PassportExpiry = memberRequest.PassportExpiry,
        //    Religion = MemberReligon?.DisplayName,
        //    ReligionSect = MemberReligonSect.DisplayName,
        //    OtherReligion = memberRequest.OtherReligion,
        //    Nationality = memberRequest.Nationality,
        //    OtherNationality = memberRequest.OtherNationality,
        //    CNICFrontImagePath = !string.IsNullOrWhiteSpace(memberRequest.CNICFrontImagePath)
        //        ? _fileStorageService.GetPublicUrl(memberRequest.CNICFrontImagePath)
        //        : null,
        //    CNICBackImagePath = !string.IsNullOrWhiteSpace(memberRequest.CNICBackImagePath)
        //        ? _fileStorageService.GetPublicUrl(memberRequest.CNICBackImagePath)
        //        : null,
        //    PicturePath = !string.IsNullOrWhiteSpace(memberRequest.PicturePath)
        //        ? _fileStorageService.GetPublicUrl(memberRequest.PicturePath)
        //        : null,
        //    Qualification = memberRequest.Qualification,
        //    Profession = memberRequest.Profession,
        //    FatherName = memberRequest.FatherName,
        //    FatherCnic = memberRequest.FatherCnic,
        //    FatherAlive = memberRequest.FatherAlive,
        //    FatherMobileNo = memberRequest.FatherMobileNo,
        //    FatherCNICFrontImagePath = !string.IsNullOrWhiteSpace(memberRequest.FatherCNICFrontImagePath)
        //        ? _fileStorageService.GetPublicUrl(memberRequest.FatherCNICFrontImagePath)
        //        : null,
        //    FatherCNICBackImagePath = !string.IsNullOrWhiteSpace(memberRequest.FatherCNICBackImagePath)
        //        ? _fileStorageService.GetPublicUrl(memberRequest.FatherCNICBackImagePath)
        //        : null,
        //    FatherPicturePath = !string.IsNullOrWhiteSpace(memberRequest.FatherPicturePath)
        //        ? _fileStorageService.GetPublicUrl(memberRequest.FatherPicturePath)
        //        : null,
        //    MotherName = memberRequest.FatherName,
        //    MotherCnic = memberRequest.FatherCnic,
        //    MotherAlive = memberRequest.FatherAlive,
        //    MotherMobileNo = memberRequest.FatherMobileNo,
        //    MotherCNICFrontImagePath = !string.IsNullOrWhiteSpace(memberRequest.MotherCNICFrontImagePath)
        //        ? _fileStorageService.GetPublicUrl(memberRequest.MotherCNICFrontImagePath)
        //        : null,
        //    MotherCNICBackImagePath = !string.IsNullOrWhiteSpace(memberRequest.MotherCNICBackImagePath)
        //        ? _fileStorageService.GetPublicUrl(memberRequest.MotherCNICBackImagePath)
        //        : null,
        //    MotherPicturePath = !string.IsNullOrWhiteSpace(memberRequest.MotherPicturePath)
        //        ? _fileStorageService.GetPublicUrl(memberRequest.MotherPicturePath)
        //        : null,
        //    IsChild = memberRequest.IsChild,
        //    TelephoneResidence = memberRequest.TelephoneResidence,
        //    MobileNo1 = memberRequest.MobileNo1,
        //    MobileNo2 = memberRequest.MobileNo2,
        //    PresentAddress = memberRequest.PresentAddress,
        //    PresentCity = memberRequest.PresentCity,
        //    PresentCountry = memberRequest.PresentCountry,
        //    PermenantAddress = memberRequest.PermenantAddress,
        //    PermenantCity = memberRequest.PermenantCity,
        //    PermenantCountry = memberRequest.PermenantCountry,
        //    BestKnowledge = memberRequest.BestKnowledge,
        //    Rservation = memberRequest.Rservation,
        //    status = memberRequest.Status
        //};

        return result;
    }
}

