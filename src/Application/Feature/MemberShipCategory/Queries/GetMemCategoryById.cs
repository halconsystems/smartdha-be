using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.MemberShipCategory.Queries;

public record GetMemberShipCategoryByIdQuery(Guid MemberShipId) : IRequest<List<MemberShipCategoryDTO>>;
public class GetMemberShipCategoryByIdQueryHandler : IRequestHandler<GetMemberShipCategoryByIdQuery, List<MemberShipCategoryDTO>>
{
    private readonly IApplicationDbContext _context;

    public GetMemberShipCategoryByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MemberShipCategoryDTO>> Handle(GetMemberShipCategoryByIdQuery request, CancellationToken ct)
    {
        var MemberShips = await _context.MemberShipCatergories.Where(x => x.MemberShipId == request.MemberShipId)
            .AsNoTracking()
            .ToListAsync(ct);

        var result = MemberShips.Select(x => new MemberShipCategoryDTO
        {
            Name = x.name,
            DisplayName = x.displayname,
            Code = x.Code,
            MemberShipId = x.MemberShipId,
        }).ToList();

        return result;
    }
}

