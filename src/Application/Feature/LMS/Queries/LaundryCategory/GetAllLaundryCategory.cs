using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryPackaging;

namespace DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryCategory;

public record GetAllLaundryCategoryQuery : IRequest<List<LaundryCategoryDTO>>;
public class GetAllLaundryCategoryQueryHandler : IRequestHandler<GetAllLaundryCategoryQuery, List<LaundryCategoryDTO>>
{
    private readonly ILaundrySystemDbContext _context;

    public GetAllLaundryCategoryQueryHandler(ILaundrySystemDbContext context)
    {
        _context = context;
    }

    public async Task<List<LaundryCategoryDTO>> Handle(GetAllLaundryCategoryQuery request, CancellationToken ct)
    {
        var MemberShips = await _context.LaundryCategories
            .AsNoTracking()
            .ToListAsync(ct);

        var result = MemberShips.Select(x => new LaundryCategoryDTO
        {
            Name = x.Name,
            DisplayName = x.DisplayName,
            Code = x.Code,
        }).ToList();

        return result;
    }
}



