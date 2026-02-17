using System.Threading;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.UserFamily.Commands.AddUserFamilyCommandHandler;
using DHAFacilitationAPIs.Domain.Constants;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Application.Feature.UserFamily.UserFamilyCommands.AddUserFamilyCommandHandler;

public class AddUserFamilyCommandHandler : IRequestHandler<AddUserFamilyCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartDhaContext;
    private readonly IFileStorageService _fileStorage;
    private readonly UserManager<ApplicationUser> _userManager;

    public AddUserFamilyCommandHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager ,ISmartdhaDbContext smartDhaContext, IFileStorageService fileStorage)
    {
        _context = context;
        _smartDhaContext = smartDhaContext;
        _fileStorage = fileStorage;
        _userManager = userManager;
    }

    public async Task<Result<Guid>> Handle(AddUserFamilyCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var response = new AddUserFamilyResponse();
            string? profileImagePath = null;
            if (request.ProfilePicture != null)
            {
                //profileImagePath = await _fileStorage.SaveFileAsync(
                //    request.ProfilePicture,
                //    "uploads/smartdha/userfamily",
                //    cancellationToken);
                profileImagePath = await _fileStorage.SaveFileInternalAsync(
                file: request.ProfilePicture,
                moduleFolder: FileStorageConstants.Modules.SmartDHA,
                subFolder: "userfamily",
                ct: cancellationToken,
                maxBytes: FileStorageConstants.MaxSize.Image,
                allowedExtensions: FileStorageConstants.Extensions.Images,
                allowedMimeTypes: FileStorageConstants.MimeTypes.Images);
            }
            string? createdBy = null;
            if (request.UserId != Guid.Empty)
            {
                var appUser = await _userManager.FindByIdAsync(request.UserId.ToString());
                if (appUser != null)
                    createdBy = appUser.Id;
            }
            var entity = new Domain.Entities.Smartdha.UserFamily
            {
                Name = request.Name,
                Relation = (RelationUserFamily)request.Relation,
                DateOfBirth = request.DOB.Date,
                Cnic = request.CNIC,
                FatherOrHusbandName = request.FatherName,
                ProfilePicture = profileImagePath,
                PhoneNumber = request.PhoneNo,
                Created = DateTime.UtcNow,
                CreatedBy = createdBy,
            };

            await _smartDhaContext.UserFamilies.AddAsync(entity, cancellationToken);
            await _smartDhaContext.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(entity.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure(new[] { ex.Message });
        }
    }
}
