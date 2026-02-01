using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubImages.Command;
using DHAFacilitationAPIs.Application.Feature.CBMS.FacilityImages.Commands;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnitImages.Commands;
public record AddFacilityUnitImagesCommand(
    Guid FacilityUnitId,
    List<AddClubImageDTO> Images
) : IRequest<SuccessResponse<List<Guid>>>;
public class AddFacilityUnitImagesCommandHandler
    : IRequestHandler<AddFacilityUnitImagesCommand, SuccessResponse<List<Guid>>>
{
    private readonly ICBMSApplicationDbContext _ctx;

    public AddFacilityUnitImagesCommandHandler(ICBMSApplicationDbContext ctx) => _ctx = ctx;

    public async Task<SuccessResponse<List<Guid>>> Handle(AddFacilityUnitImagesCommand request, CancellationToken ct)
    {
        // 1) Room must exist
        var roomExists = await _ctx.FacilityUnits
            .AsNoTracking()
            .AnyAsync(r => r.Id == request.FacilityUnitId && (r.IsDeleted == false || r.IsDeleted == null), ct);

        if (!roomExists)
            throw new KeyNotFoundException("Club not found.");

        var images = request.Images ?? new();

        // 2) Enforce single Main image overall (DB already has filtered unique index; also check here)
        bool dbHasMain = await _ctx.FacilityUnitImages
            .AsNoTracking()
            .AnyAsync(x => x.FacilityUnitId == request.FacilityUnitId
                        && x.Category == ImageCategory.Main
                        && (x.IsDeleted == false || x.IsDeleted == null), ct);

        int incomingMainCount = images.Count(i => i.Category == ImageCategory.Main);
        if (dbHasMain && incomingMainCount > 0)
            throw new InvalidOperationException("Club already has a Main image. Remove/replace it before adding a new Main.");

        if (!dbHasMain && incomingMainCount > 1)
            throw new InvalidOperationException("Only one Main image is allowed per Club.");

        // If you want to auto-promote the first incoming image as Main when none exists:
        // if (!dbHasMain && incomingMainCount == 0 && images.Count > 0)
        // {
        //     var first = images[0];
        //     images[0] = first with { Category = ImageCategory.Main };
        // }

        // 3) Map and save
        var entities = images.Select(i => new FacilityUnitImage
        {
            FacilityUnitId = request.FacilityUnitId,
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

        _ctx.FacilityUnitImages.AddRange(entities);
        await _ctx.SaveChangesAsync(ct);

        var ids = entities.Select(e => e.Id).ToList();
        return new SuccessResponse<List<Guid>>(ids, "Images added.");
    }
}

