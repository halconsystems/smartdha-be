using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Room.Queries.GetRoomImages;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Grounds.Command.GroundImages.Queries;


public class GetGroundImagesQuery : IRequest<SuccessResponse<List<GroundImagesDTO>>>
{
    [Required]
    public Guid GroundId { get; set; }
}
public class GetRoomImagesQueryHandler : IRequestHandler<GetGroundImagesQuery, SuccessResponse<List<GroundImagesDTO>>>
{
    private readonly IOLMRSApplicationDbContext _ctx;

    public GetRoomImagesQueryHandler(IOLMRSApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<SuccessResponse<List<GroundImagesDTO>>> Handle(GetGroundImagesQuery request, CancellationToken ct)
    {
        var images = await _ctx.GroundImages
            .Where(x => x.GroundId == request.GroundId && x.IsDeleted != true)
            .Select(x => new GroundImagesDTO
            {
                Id = x.Id,
                GroudId = x.GroundId,
                ImageURL = x.ImageURL,
                ImageExtension = x.ImageExtension,
                ImageName = x.ImageName,
                Description = x.Description,
                Category = x.Category
            })
            .ToListAsync(ct);

        return Success.Ok(images, "Ground images fetched successfully.");
    }
}

