using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Room.Queries.GetRoomImages;
public class GetRoomImagesQuery : IRequest<SuccessResponse<List<RoomImageDto>>>
{
    [Required]
    public Guid RoomId { get; set; }
}
public class GetRoomImagesQueryHandler : IRequestHandler<GetRoomImagesQuery, SuccessResponse<List<RoomImageDto>>>
{
    private readonly IOLMRSApplicationDbContext _ctx;

    public GetRoomImagesQueryHandler(IOLMRSApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<SuccessResponse<List<RoomImageDto>>> Handle(GetRoomImagesQuery request, CancellationToken ct)
    {
        var images = await _ctx.RoomImages
            .Where(x => x.RoomId == request.RoomId && x.IsDeleted != true)
            .Select(x => new RoomImageDto
            {
                Id = x.Id,
                RoomId = x.RoomId,
                ImageURL = x.ImageURL,
                ImageExtension = x.ImageExtension,
                ImageName = x.ImageName,
                Description = x.Description,
                Category = x.Category
            })
            .ToListAsync(ct);

        return Success.Ok(images, "Room images fetched successfully.");
    }
}

