using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using static System.Net.Mime.MediaTypeNames;

namespace DHAFacilitationAPIs.Application.Feature.User.Commands.UserImage;

public record AddUserImagesCommand(
    IFormFile File,
    string? ImageName,
    string? Description,
    ImageCategory Category
) : IRequest<SuccessResponse<Guid>>;
public class AddUserImagesCommandHandler
    : IRequestHandler<AddUserImagesCommand, SuccessResponse<Guid>>
{
    private readonly IApplicationDbContext _ctx;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileStorageService _fileStorage;

    public AddUserImagesCommandHandler(
        IApplicationDbContext ctx,
        UserManager<ApplicationUser> userManager,
        ICurrentUserService currentUser,
        IFileStorageService fileStorage)
    {
        _ctx = ctx;
        _userManager = userManager;
        _currentUser = currentUser;
        _fileStorage = fileStorage;
    }

    public async Task<SuccessResponse<Guid>> Handle(
        AddUserImagesCommand request,
        CancellationToken ct)
    {
        // 1️⃣ Current user
        var userId = _currentUser.UserId.ToString()
            ?? throw new UnauthorizedAccessException();

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new KeyNotFoundException("User not found");

        // 2️⃣ Save file HERE (correct place)
        var folder = $"users/{userId}/profile";

        var filePath = await _fileStorage.SaveFileAsync(
            request.File,
            folder,
            ct
        );

        var dto = new AddProfileImageDTO(
            ImageURL: filePath,
            ImageExtension: Path.GetExtension(filePath),
            ImageName: request.ImageName,
            Description: request.Description,
            Category: request.Category
        );

        // 3️⃣ Insert / Update DB
        var userImage = await _ctx.UserImages
            .FirstOrDefaultAsync(x => x.UserId == Guid.Parse(user.Id), ct);

        if (userImage == null)
        {
            userImage = new UserImages
            {
                UserId = Guid.Parse(user.Id),
                ImageURL = dto.ImageURL,
                ImageExtension = dto.ImageExtension,
                ImageName = dto.ImageName,
                Description = dto.Description,
                Category = dto.Category
            };
            _ctx.UserImages.Add(userImage);
        }
        else
        {
            userImage.ImageURL = dto.ImageURL;
            userImage.ImageExtension = dto.ImageExtension;
            userImage.ImageName = dto.ImageName;
            userImage.Description = dto.Description;
            userImage.Category = dto.Category;
        }

        await _ctx.SaveChangesAsync(ct);


        return new SuccessResponse<Guid>(
            userImage.Id,
            "Profile image saved successfully."
        );
    }
}


