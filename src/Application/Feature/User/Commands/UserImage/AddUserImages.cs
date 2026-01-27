using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryItems;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using static System.Net.Mime.MediaTypeNames;

namespace DHAFacilitationAPIs.Application.Feature.User.Commands.UserImage;

public record AddUserImagesCommand(
    AddProfileImageDTO images
) : IRequest<SuccessResponse<Guid>>;
public class AddUserImagesCommandHandler
    : IRequestHandler<AddUserImagesCommand, SuccessResponse<Guid>>
{
    private readonly IApplicationDbContext _ctx;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUser;

    public AddUserImagesCommandHandler(IApplicationDbContext ctx, UserManager<ApplicationUser> userManager, ICurrentUserService currentUser )
    {
        _ctx = ctx;
        _userManager = userManager;
        _currentUser = currentUser;
    }

    public async Task<SuccessResponse<Guid>> Handle(AddUserImagesCommand request, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId.ToString();
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == currentUserId);

        if (user == null)
            throw new KeyNotFoundException("User not found.");

        var userImages = await _ctx.UserImages.FirstOrDefaultAsync(u => u.Id == Guid.Parse(user.Id),ct);

         if (userImages == null)
        {
            var entities = new UserImages
            {
                UserId = Guid.Parse(user.Id),
                ImageURL = request.images.ImageURL,
                ImageExtension = request.images.ImageExtension,
                ImageName = request.images.ImageName,
                Description = request.images.Description,
                Category = request.images.Category,

            };

            _ctx.UserImages.Add(entities);

            await _ctx.SaveChangesAsync(ct);

            return new SuccessResponse<Guid>(entities.Id, "Images added.");
        }
        else
        {
            userImages.UserId = Guid.Parse(user.Id);
            userImages.ImageURL = request.images.ImageURL;
            userImages.ImageExtension = request.images.ImageExtension;
            userImages.ImageName = request.images.ImageName;
            userImages.Description = request.images.Description;
            userImages.Category = request.images.Category;
        }

        await _ctx.SaveChangesAsync(ct);

        return new SuccessResponse<Guid>(userImages.Id, "Images added.");

    }
}

