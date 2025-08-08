using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.RoomCategories.Queries.GetRoomCategoryById;
public record GetRoomCategoryByIdQuery(Guid Id) : IRequest<Domain.Entities.RoomCategory?>;

public class GetRoomCategoryByIdQueryHandler : IRequestHandler<GetRoomCategoryByIdQuery, Domain.Entities.RoomCategory?>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    public GetRoomCategoryByIdQueryHandler(IOLMRSApplicationDbContext ctx) => _ctx = ctx;

    public async Task<Domain.Entities.RoomCategory?> Handle(GetRoomCategoryByIdQuery request, CancellationToken ct)
    {
        return await _ctx.RoomCategories.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id && (x.IsDeleted == false || x.IsDeleted == null), ct);
    }
}
