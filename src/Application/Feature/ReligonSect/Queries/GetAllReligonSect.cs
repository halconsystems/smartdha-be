using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.ReligonSect.Queries;

public record GetAllReligonSectCategoryQuery : IRequest<List<ReligonSectDTO>>;
public class GetAllReligonCategoryQueryHandler : IRequestHandler<GetAllReligonSectCategoryQuery, List<ReligonSectDTO>>
{
    private readonly IApplicationDbContext _context;

    public GetAllReligonCategoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ReligonSectDTO>> Handle(GetAllReligonSectCategoryQuery request, CancellationToken ct)
    {
        var Religions = await _context.ReligonSects
            .AsNoTracking()
            .ToListAsync(ct);

        var result = Religions.Select(x => new ReligonSectDTO
        {
            Name = x.Name,
            DisplayName = x.DisplayName,
            Code = x.Code,
            ReligonId = x.ReligonId
        }).ToList();

        return result;
    }
}


