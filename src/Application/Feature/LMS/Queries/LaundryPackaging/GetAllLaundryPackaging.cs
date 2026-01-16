using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryService;

namespace DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryPackaging;

public record GetAllLaundryPackagingQuery : IRequest<List<LaundryPackagingDTO>>;
public class GetAllLaundryPackagingHandler : IRequestHandler<GetAllLaundryPackagingQuery, List<LaundryPackagingDTO>>
{
    private readonly ILaundrySystemDbContext _context;

    public GetAllLaundryPackagingHandler(ILaundrySystemDbContext context)
    {
        _context = context;
    }

    public async Task<List<LaundryPackagingDTO>> Handle(GetAllLaundryPackagingQuery request, CancellationToken ct)
    {
        var MemberShips = await _context.LaundryPackagings
            .AsNoTracking()
            .ToListAsync(ct);

        var result = MemberShips.Select(x => new LaundryPackagingDTO
        {
            Id = x.Id,
            Name = x.Name,
            DisplayName = x.DisplayName,
            Code = x.Code,
        }).ToList();

        return result;
    }
}


