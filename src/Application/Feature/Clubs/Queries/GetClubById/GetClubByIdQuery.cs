using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.Clubs.Queries.GetClubById;
public record GetClubByIdQuery(Guid Id) : IRequest<Club?>;



public class GetClubByIdQueryHandler : IRequestHandler<GetClubByIdQuery, Club?>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    private readonly IMapper _mapper;
    public GetClubByIdQueryHandler(IOLMRSApplicationDbContext ctx, IMapper mapper)
    {
        _ctx = ctx;
        _mapper = mapper;
    }
    public async Task<Club?> Handle(GetClubByIdQuery request, CancellationToken ct)
    {
        return await _ctx.Clubs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id && (x.IsDeleted == null || x.IsDeleted == false), ct);
    }
}

