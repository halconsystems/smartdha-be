using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Room.Commands.AddRoomImages;
public record AddRoomImagesCommand(
    Guid RoomId,
    List<AddRoomImageDto> Images
) : IRequest<SuccessResponse<List<Guid>>>;
public class AddRoomImagesCommandHandler
    : IRequestHandler<AddRoomImagesCommand, SuccessResponse<List<Guid>>>
{
    private readonly IOLMRSApplicationDbContext _ctx;

    public AddRoomImagesCommandHandler(IOLMRSApplicationDbContext ctx) => _ctx = ctx;

    public async Task<SuccessResponse<List<Guid>>> Handle(AddRoomImagesCommand request, CancellationToken ct)
    {
        // 1) Room must exist
        var roomExists = await _ctx.Rooms
            .AsNoTracking()
            .AnyAsync(r => r.Id == request.RoomId && (r.IsDeleted == false || r.IsDeleted == null), ct);

        if (!roomExists)
            throw new KeyNotFoundException("Room not found.");

        var images = request.Images ?? new();

        // 2) Enforce single Main image overall (DB already has filtered unique index; also check here)
        bool dbHasMain = await _ctx.RoomImages
            .AsNoTracking()
            .AnyAsync(x => x.RoomId == request.RoomId
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
        var entities = images.Select(i => new Domain.Entities.RoomImages
        {
            RoomId = request.RoomId,
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

        _ctx.RoomImages.AddRange(entities);
        await _ctx.SaveChangesAsync(ct);

        var ids = entities.Select(e => e.Id).ToList();
        return new SuccessResponse<List<Guid>>(ids, "Images added.");
    }
}

