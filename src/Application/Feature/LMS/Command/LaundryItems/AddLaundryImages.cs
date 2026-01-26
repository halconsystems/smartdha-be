using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Room.Commands.AddRoomImages;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryItems;


public record AddLaundryImagesCommand(
    Guid LaundryId,
    AddLaundryImageDTO images
) : IRequest<SuccessResponse<Guid>>;
public class AddLaundryImagesCommandHandler
    : IRequestHandler<AddLaundryImagesCommand, SuccessResponse<Guid>>
{
    private readonly ILaundrySystemDbContext _ctx;

    public AddLaundryImagesCommandHandler(ILaundrySystemDbContext ctx) => _ctx = ctx;

    public async Task<SuccessResponse<Guid>> Handle(AddLaundryImagesCommand request, CancellationToken ct)
    {
        // 1) Room must exist
        var LaundryItems = await _ctx.LaundryItems
            .FirstOrDefaultAsync(r => r.Id == request.LaundryId && (r.IsDeleted == false || r.IsDeleted == null), ct);

        if (LaundryItems == null)
            throw new KeyNotFoundException("LaundryItems not found.");

        // If you want to auto-promote the first incoming image as Main when none exists:
        // if (!dbHasMain && incomingMainCount == 0 && images.Count > 0)
        // {
        //     var first = images[0];
        //     images[0] = first with { Category = ImageCategory.Main };
        // }

        // 3) Map and save

        LaundryItems.ItemImage = request.images.ImageURL;
        await _ctx.SaveChangesAsync(ct);

        return new SuccessResponse<Guid>(LaundryItems.Id, "Images added.");
    }
}

