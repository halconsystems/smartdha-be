using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubImages.Command;
using DHAFacilitationAPIs.Application.Feature.Room.Commands.AddRoomImages;
using DHAFacilitationAPIs.Application.Feature.User.Commands.UserImage;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubServices.Command;

public record AddClubServiceImageCommand(
    Guid ServiceId,
    List<AddClubServiceImageDto> Images
) : IRequest<SuccessResponse<List<Guid>>>;
public class AddClubServiceImageCommandHandler
    : IRequestHandler<AddClubServiceImageCommand, SuccessResponse<List<Guid>>>
{
    private readonly IOLMRSApplicationDbContext _ctx;

    public AddClubServiceImageCommandHandler(IOLMRSApplicationDbContext ctx) => _ctx = ctx;

    public async Task<SuccessResponse<List<Guid>>> Handle(AddClubServiceImageCommand request, CancellationToken ct)
    {
        // 1) Room must exist
        var serviceExists = await _ctx.ClubProcess
            .AsNoTracking()
            .AnyAsync(r => r.Id == request.ServiceId && (r.IsDeleted == false || r.IsDeleted == null), ct);

        if (!serviceExists)
            throw new KeyNotFoundException("Service not found.");

        var images = request.Images ?? new();

        // 2) Enforce single Main image overall (DB already has filtered unique index; also check here)
        bool dbHasMain = await _ctx.ClubServiceImages
            .AsNoTracking()
            .AnyAsync(x => x.ServiceId == request.ServiceId
                        && x.Category == ImageCategory.Main
                        && (x.IsDeleted == false || x.IsDeleted == null), ct);

        int incomingMainCount = images.Count(i => i.Category == ImageCategory.Main);
        if (dbHasMain && incomingMainCount > 0)
            throw new InvalidOperationException("Room already has a Main image. Remove/replace it before adding a new Main.");

        if (!dbHasMain && incomingMainCount > 1)
            throw new InvalidOperationException("Only one Main image is allowed per room.");

        // If you want to auto-promote the first incoming image as Main when none exists:
        // if (!dbHasMain && incomingMainCount == 0 && images.Count > 0)
        // {
        //     var first = images[0];
        //     images[0] = first with { Category = ImageCategory.Main };
        // }

        // 3) Map and save
        var entities = images.Select(i => new Domain.Entities.CBMS.ClubServiceImages
        {
            ServiceId = request.ServiceId,
            ImageURL = i.ImageURL,
            ImageExtension = i.ImageExtension,
            ImageName = i.ImageName,
            Description = i.Description,
            Category = i.Category,

            // If your audit fields aren’t set by interceptors, set defaults:
            IsActive = true,
            IsDeleted = false,
            Created = DateTime.UtcNow
        }).ToList();

        _ctx.ClubServiceImages.AddRange(entities);
        await _ctx.SaveChangesAsync(ct);

        var ids = entities.Select(e => e.Id).ToList();
        return new SuccessResponse<List<Guid>>(ids, "Images added.");
    }
}
