using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.ResidenceType.Queries;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.CreateResidenceType.Queries.GetResidenceTypes;
public record GetResidenceTypesQuery(ClubType ClubType) : IRequest<SuccessResponse<List<ResidenceTypeDto>>>;

public class GetResidenceTypesQueryHandler
    : IRequestHandler<GetResidenceTypesQuery, SuccessResponse<List<ResidenceTypeDto>>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    private readonly IMapper _mapper;

    public GetResidenceTypesQueryHandler(IOLMRSApplicationDbContext ctx, IMapper mapper)
    {
        _ctx = ctx;
        _mapper = mapper;
    }

    public async Task<SuccessResponse<List<ResidenceTypeDto>>> Handle(GetResidenceTypesQuery request, CancellationToken ct)
    {
        var q = _ctx.ResidenceTypes
            .AsNoTracking();


        var list = await q
            .Where(x => (x.IsDeleted == null || x.IsDeleted == false) &&
                x.Rooms.Any(r => r.Club.ClubType == request.ClubType))
            .OrderBy(x => x.Name)
            .ProjectTo<ResidenceTypeDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return Success.Ok(list);
        // If you have the helper: return Success.Ok(list);
    }
}
