using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.MemberShipCategory.Queries;

public record GetAllMemberShipCategoryQuery : IRequest<List<MemberShipCategoryDTO>>;
public class GetAllMemberShipCategoryQueryHandler : IRequestHandler<GetAllMemberShipCategoryQuery, List<MemberShipCategoryDTO>>
{
    private readonly IApplicationDbContext _context;

    public GetAllMemberShipCategoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MemberShipCategoryDTO>> Handle(GetAllMemberShipCategoryQuery request, CancellationToken ct)
    {
        var MemberShips = await _context.MemberShipCatergories
            .AsNoTracking()
            .ToListAsync(ct);

        var result = MemberShips.Select(x => new MemberShipCategoryDTO
        {
            Id = x.Id,
            Name = x.name,
            DisplayName = x.displayname,
            Code = x.Code,
            MemberShipId = x.MemberShipId,
        }).ToList();

        return result;
    }
}

