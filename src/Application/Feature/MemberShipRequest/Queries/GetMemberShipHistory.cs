using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.MemberShipRequest.Queries;

public record GetMemberShipHistoryQuery(Guid Id) : IRequest<MemberRequestHistoryDTO>;
public class GetMemberShipHistoryQueryHandler : IRequestHandler<GetMemberShipHistoryQuery, MemberRequestHistoryDTO>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _fileStorageService;

    public GetMemberShipHistoryQueryHandler(IApplicationDbContext context,IFileStorageService fileStorageService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
    }

    public async Task<MemberRequestHistoryDTO> Handle(GetMemberShipHistoryQuery request, CancellationToken ct)
    {
        var memberRequest = await _context.MemberRequests
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        if (memberRequest == null) throw new KeyNotFoundException("Member Request Not Fount");

        var MemberShipCatergories = await _context.MemberShipCatergories.FirstOrDefaultAsync(x => x.Id == memberRequest.MemberShipCategory,ct);

        if (MemberShipCatergories == null) throw new KeyNotFoundException("Member Category Request Not Fount");


        var MemberShipPurpose = await _context.MemberShips.FirstOrDefaultAsync(x => x.Id == MemberShipCatergories.MemberShipId,  ct);


        var MemberReligonSect = await _context.ReligonSects.FirstOrDefaultAsync(x => x.Id == memberRequest.ReligonSectId, ct);

        if (MemberReligonSect == null) throw new KeyNotFoundException("Religon Section  Request Not Fount");

        var MemberReligon = await _context.Religions.FirstOrDefaultAsync(x => x.Id == MemberReligonSect.ReligonId, ct);



        var result = new MemberRequestHistoryDTO
        {
            Name = memberRequest.Name,
            MemberShipCategory = MemberShipCatergories.displayname,
            Purpose = MemberShipPurpose?.DisplayName,
            CNIC = memberRequest.CNIC,
            Email = memberRequest.Email,
            IsActive = memberRequest.IsActive,
            IsDelete = memberRequest.IsDeleted,
            LiveTime = memberRequest.LiveTime,
            CnicExpiryDate = memberRequest.CnicExpiryDate,
            Dob = memberRequest.Dob,
            Gender = memberRequest.Gender,
            MaritalStatus = memberRequest.MaritalStatus,
            NTNno = memberRequest.NTNno,
            PassportNo = memberRequest.PassportNo,
            PassportPlace = memberRequest.PassportPlace,
            PassportExpiry = memberRequest.PassportExpiry,
            Religion = MemberReligon?.DisplayName,
            ReligionSect = MemberReligonSect.DisplayName,
            OtherReligion = memberRequest.OtherReligion,
            Nationality = memberRequest.Nationality,
            OtherNationality = memberRequest.OtherNationality,
            CNICFrontImagePath = !string.IsNullOrWhiteSpace(memberRequest.CNICFrontImagePath)
                ? _fileStorageService.GetPublicUrl(memberRequest.CNICFrontImagePath)
                : null,
            CNICBackImagePath = !string.IsNullOrWhiteSpace(memberRequest.CNICBackImagePath)
                ? _fileStorageService.GetPublicUrl(memberRequest.CNICBackImagePath)
                : null,
            PicturePath = !string.IsNullOrWhiteSpace(memberRequest.PicturePath)
                ? _fileStorageService.GetPublicUrl(memberRequest.PicturePath)
                : null,
            Qualification = memberRequest.Qualification,
            Profession = memberRequest.Profession,
            FatherName = memberRequest.FatherName,
            FatherCnic = memberRequest.FatherCnic,
            FatherAlive = memberRequest.FatherAlive,
            FatherMobileNo = memberRequest.FatherMobileNo,
            FatherCNICFrontImagePath = !string.IsNullOrWhiteSpace(memberRequest.FatherCNICFrontImagePath)
                ? _fileStorageService.GetPublicUrl(memberRequest.FatherCNICFrontImagePath)
                : null,
            FatherCNICBackImagePath = !string.IsNullOrWhiteSpace(memberRequest.FatherCNICBackImagePath)
                ? _fileStorageService.GetPublicUrl(memberRequest.FatherCNICBackImagePath)
                : null,
            FatherPicturePath = !string.IsNullOrWhiteSpace(memberRequest.FatherPicturePath)
                ? _fileStorageService.GetPublicUrl(memberRequest.FatherPicturePath)
                : null,
            MotherName = memberRequest.FatherName,
            MotherCnic = memberRequest.FatherCnic,
            MotherAlive = memberRequest.FatherAlive,
            MotherMobileNo = memberRequest.FatherMobileNo,
            MotherCNICFrontImagePath = !string.IsNullOrWhiteSpace(memberRequest.MotherCNICFrontImagePath)
                ? _fileStorageService.GetPublicUrl(memberRequest.MotherCNICFrontImagePath)
                : null,
            MotherCNICBackImagePath = !string.IsNullOrWhiteSpace(memberRequest.MotherCNICBackImagePath)
                ? _fileStorageService.GetPublicUrl(memberRequest.MotherCNICBackImagePath)
                : null,
            MotherPicturePath = !string.IsNullOrWhiteSpace(memberRequest.MotherPicturePath)
                ? _fileStorageService.GetPublicUrl(memberRequest.MotherPicturePath)
                : null,
            IsChild = memberRequest.IsChild,
            TelephoneResidence = memberRequest.TelephoneResidence,
            MobileNo1 = memberRequest.MobileNo1,
            MobileNo2 = memberRequest.MobileNo2,
            PresentAddress = memberRequest.PresentAddress,
            PresentCity = memberRequest.PresentCity,
            PresentCountry = memberRequest.PresentCountry,
            PermenantAddress = memberRequest.PermenantAddress,
            PermenantCity = memberRequest.PermenantCity,
            PermenantCountry = memberRequest.PermenantCountry,
            BestKnowledge = memberRequest.BestKnowledge,
            Rservation = memberRequest.Rservation,
        };

        return result;
    }
}

