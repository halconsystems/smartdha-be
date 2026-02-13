using System.Threading;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.UserFamily.Commands.AddUserFamilyCommandHandler;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Application.Feature.UserFamily.UserFamilyCommands.AddUserFamilyCommandHandler;

public class AddUserFamilyCommandHandler : IRequestHandler<AddUserFamilyCommand, AddUserFamilyResponse>
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

    public async Task<AddUserFamilyResponse> Handle(AddUserFamilyCommand request, CancellationToken cancellationToken)
    {
        var response = new AddUserFamilyResponse();
        string? profileImagePath = null;
        if (request.ProfilePicture != null)
        {
            profileImagePath = await _fileStorage.SaveFileAsync(
                request.ProfilePicture,
                "uploads/smartdha/userfamily",
                cancellationToken);
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
            DateOfBirth = request.DOB,
            Cnic = request.CNIC,
            FatherOrHusbandName = request.FatherName,
            ProfilePicture = profileImagePath,
            PhoneNumber = request.PhoneNo,
            Created = DateTime.UtcNow,
            CreatedBy = createdBy,
        };

        await _smartDhaContext.UserFamilies.AddAsync(entity, cancellationToken);
        await _smartDhaContext.SaveChangesAsync(cancellationToken);

        response.Success = true;
        response.Message = "Family member added successfully.";
        response.Id = entity.Id;
        return response;
    }
}
