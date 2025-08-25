using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.Clubs.Queries.GetClubs;
public record GetClubsQuery()
    : IRequest<SuccessResponse<List<ClubDto>>>;

public class GetClubsQueryHandler
    : IRequestHandler<GetClubsQuery, SuccessResponse<List<ClubDto>>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    private readonly IMapper _mapper;

    public GetClubsQueryHandler(IOLMRSApplicationDbContext ctx, IMapper mapper)
    {
        _ctx = ctx;
        _mapper = mapper;
    }

    public async Task<SuccessResponse<List<ClubDto>>> Handle(GetClubsQuery request, CancellationToken ct)
    {
        var q = _ctx.Clubs.AsNoTracking();


        var clubs = await q
            .Where(x =>x.IsDeleted == null || x.IsDeleted == false)
            .OrderBy(x => x.Name)
            .ProjectTo<ClubDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return Success.Ok(clubs);
    }
}


