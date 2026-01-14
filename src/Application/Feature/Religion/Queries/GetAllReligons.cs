using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.MemberShipCategory.Queries;

namespace DHAFacilitationAPIs.Application.Feature.Religion.Queries;

public record GetAllReligonCategoryQuery : IRequest<List<ReligonDTO>>;
public class GetAllReligonCategoryQueryHandler : IRequestHandler<GetAllReligonCategoryQuery, List<ReligonDTO>>
{
    private readonly IApplicationDbContext _context;

    public GetAllReligonCategoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ReligonDTO>> Handle(GetAllReligonCategoryQuery request, CancellationToken ct)
    {
        var Religions = await _context.Religions
            .AsNoTracking()
            .ToListAsync(ct);

        var result = Religions.Select(x => new ReligonDTO
        {
            Name = x.Name,
            DisplayName = x.DisplayName,
            Code = x.Code,
        }).ToList();

        return result;
    }
}

