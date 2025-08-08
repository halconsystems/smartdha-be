using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.RoomCategories.Queries.GetRoomCategoryById;
public record GetRoomCategoryByIdQuery(Guid Id)
    : IRequest<SuccessResponse<RoomCategoryDto>>;

public class GetRoomCategoryByIdQueryHandler
    : IRequestHandler<GetRoomCategoryByIdQuery, SuccessResponse<RoomCategoryDto>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    private readonly IMapper _mapper;

    public GetRoomCategoryByIdQueryHandler(IOLMRSApplicationDbContext ctx, IMapper mapper)
    {
        _ctx = ctx;
        _mapper = mapper;
    }

    public async Task<SuccessResponse<RoomCategoryDto>> Handle(GetRoomCategoryByIdQuery request, CancellationToken ct)
    {
        var dto = await _ctx.RoomCategories
            .AsNoTracking()
            .Where(x => x.Id == request.Id && (x.IsDeleted == false || x.IsDeleted == null))
            .ProjectTo<RoomCategoryDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);

        if (dto is null)
            throw new NotFoundException(nameof(RoomCategories), request.Id.ToString());

        return Success.Ok(dto);
        // or: return Success.Ok(dto); if you have the helper
    }
}
