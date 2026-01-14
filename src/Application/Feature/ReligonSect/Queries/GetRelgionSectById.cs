using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.ReligonSect.Queries;

public record GetReligonSectByIdQuery(Guid ReligonId) : IRequest<List<ReligonSectDTO>>;
public class GetReligonSectByIdQueryHandler : IRequestHandler<GetReligonSectByIdQuery, List<ReligonSectDTO>>
{
    private readonly IApplicationDbContext _context;

    public GetReligonSectByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ReligonSectDTO>> Handle(GetReligonSectByIdQuery request, CancellationToken ct)
    {
        var MemberShips = await _context.ReligonSects.Where(x => x.ReligonId == request.ReligonId)
            .AsNoTracking()
            .ToListAsync(ct);

        var result = MemberShips.Select(x => new ReligonSectDTO
        {
            Name = x.Name,
            DisplayName = x.DisplayName,
            Code = x.Code,
            ReligonId = x.ReligonId,
        }).ToList();

        return result;
    }
}

