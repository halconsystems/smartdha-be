//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using DHAFacilitationAPIs.Application.Common.Interfaces;
//using DHAFacilitationAPIs.Application.Feature.MemberShipRequest.Queries;
//using DHAFacilitationAPIs.Application.ViewModels;
//using DHAFacilitationAPIs.Domain.Entities;
//using DHAFacilitationAPIs.Domain.Enums;
//using Microsoft.AspNetCore.Http;

//namespace DHAFacilitationAPIs.Application.Feature.MemberShipRenewal.Command;

//public class CreateMemberShipRenewalCommand : IRequest<SuccessResponse<string>>
//{
//    [Required]
//    public string MemPk { get; set; } = default!;
//    public string? PresentAddress { get; set; }
//    public string? permenantAddress { get; set; }
   

//}
//public class CreateMemberShipRequestCommandHandler : IRequestHandler<CreateMemberShipRenewalCommand, SuccessResponse<string>>
//{
//    private readonly IApplicationDbContext _context;
//    private readonly IFileStorageService _fileStorage;
//    private readonly IProcedureService _procedure;

//    public CreateMemberShipRequestCommandHandler(IApplicationDbContext context, IFileStorageService fileStorageService, ISmsService otpService,IProcedureService procedure)
//    {
//        _context = context;
//        _fileStorage = fileStorageService;
//        _procedure = procedure;
//    }

//    public async Task<SuccessResponse<string>> Handle(CreateMemberShipRenewalCommand command, CancellationToken ct)
//    {
//        try
//        {
//            var existMember = _procedure.

//            var entity = new MemberRequest
//            {
//                MemberShipCategory = command.MemberShipCategoryId,
//                CNIC = command.Cnic,
//                LiveTime = command.LifeTime,
//                CnicExpiryDate = command.CnicExpiry,
//                Dob = command.DOB,
//                Name = command.Name,
//                Gender = command.Gender,
//                MaritalStatus = command.MaritalStatus,
//                NTNno = command.NTNno,
//                PassportNo = command.PassportNo,
//                PassportPlace = command.PassportPlace,
//                PassportExpiry = command.PassportExpiry,
//                ReligonSectId = command.ReligonSect,
//                OtherReligion = command.otherReligon,
//                Nationality = command.Nationality,
//                OtherNationality = command.otherNationality,
//                Qualification = command.Qualification,
//                Profession = command.Profession,
//                FatherName = command.FatherName,
//                FatherAlive = command.FatherAlive,
//                FatherCnic = command.FatherCnic,
//                FatherMobileNo = command.FatherMobileNo,
//                MotherName = command.MotherName,
//                MotherAlive = command.MotherAlive,
//                MotherCnic = command.MotherCnic,
//                MotherMobileNo = command.MotherMobileNo,
//                Email = command.Email,
//                TelephoneResidence = command.TelephoneResidence,
//                MobileNo1 = command.MobileNo1,
//                MobileNo2 = command.MobileNo2,
//                PresentAddress = command.PresentAddress,
//                PresentCity = command.PresentCity,
//                PresentCountry = command.PresentCountry,
//                PermenantAddress = command.PermenantAddress,
//                PermenantCity = command.PermenantCity,
//                PermenantCountry = command.PermenantCountry,
//                BestKnowledge = command.BestKnowledge,
//                Rservation = command.Rservation,
//                IsChild = command.IsChild,


//                PicturePath = profilePicture,
//                CNICFrontImagePath = frontPath,
//                CNICBackImagePath = backPath,

//                FatherPicturePath = FatherprofilePicture,
//                FatherCNICFrontImagePath = FatherfrontPath,
//                FatherCNICBackImagePath = FatherbackPath,

//                MotherPicturePath = motherprofilePicture,
//                MotherCNICFrontImagePath = motherfrontPath,
//                MotherCNICBackImagePath = motherbackPath,

//                Ranks = command.Ranks,
//                Serving = command.Serving,
//                ServiceNo = command.ServiceNo,
//                ArmsSvc = command.ArmsSvc,
//                CauseRetirement = command.CauseRetirement,

//            };

//            _context.MemberRequests.Add(entity);
//            await _context.SaveChangesAsync(ct);
//            if (command.MemberSpouses != null && command.MaritalStatus == MaritalStatus.Married)
//            {
//                var memberSpouse = command.MemberSpouses.Select(x => new MemberSpouse
//                {
//                    MemberShipId = entity.Id,
//                    FullName = x.FullName,
//                    MobileNo = x.MobileNo,
//                    Email = x.Email,
//                    Nationality = x.Nationality,
//                    otherNationality = x.otherNationality,
//                    PicturePath = _fileStorage.SaveFileMemeberrequestAsync(x.PicturePath, "cnic", ct).ToString(),
//                    CnicFrontImage = _fileStorage.SaveFileMemeberrequestAsync(x.CnicFrontImage, "cnic", ct).ToString(),
//                    CnicBackImage = _fileStorage.SaveFileMemeberrequestAsync(x.CnicBackImage, "cnic", ct).ToString(),
//                    Cnic = x.Cnic,
//                    CnicExpiry = x.CnicExpiry,
//                }).ToList();

//                _context.MemberSpouses.AddRange(memberSpouse);
//                await _context.SaveChangesAsync(ct);
//            }

//            if (command.MemberChildrens != null && command.IsChild)
//            {
//                var memberChildrens = command.MemberChildrens.Select(x => new MemberChildren
//                {
//                    MemberShipId = entity.Id,
//                    FullName = x.FullName,
//                    MobileNo = x.MobileNo,
//                    Relation = x.Relation,
//                    IsAdult = x.IsAdult,
//                    CnicNo = x.CnicNo,
//                    PicturePath = _fileStorage.SaveFileMemeberrequestAsync(x.PicturePath, "cnic", ct).ToString(),
//                    CNICFrontImagePath = _fileStorage.SaveFileMemeberrequestAsync(x.CNICFrontImagePath, "cnic", ct).ToString(),
//                    CNICBackImagePath = _fileStorage.SaveFileMemeberrequestAsync(x.CNICBackImagePath, "cnic", ct).ToString(),
//                    CnicExpiryDate = x.CnicExpiryDate,
//                }).ToList();
//                _context.MemberChildrens.AddRange(memberChildrens);
//                await _context.SaveChangesAsync(ct);

//            }

//            return Success.Created(entity.Id.ToString());
//        }
//        catch (Exception ex)
//        {

//            throw new Exception(ex.Message);
//        }
//    }
//}
