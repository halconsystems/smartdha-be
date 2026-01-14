using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.MemberShip.Queries;

public record GetAllMemberShipQuery : IRequest<List<MemberShipDTO>>;
public class GetAllMemberShipQueryHandler : IRequestHandler<GetAllMemberShipQuery, List<MemberShipDTO>>
{
    private readonly IApplicationDbContext _context;

    public GetAllMemberShipQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MemberShipDTO>> Handle(GetAllMemberShipQuery request, CancellationToken ct)
    {
        var MemberShips = await _context.MemberShips
            .AsNoTracking()
            .ToListAsync(ct);

        var result = MemberShips.Select(x => new MemberShipDTO
        {
            Name = x.Name,
            DisplayName = x.DisplayName,
            Code = x.Code,
        }).ToList();

        return result;
    }
}

