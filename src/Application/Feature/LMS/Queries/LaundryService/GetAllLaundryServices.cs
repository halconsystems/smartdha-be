using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.MemberShipCategory.Queries;

namespace DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryService;

public record GetAllLaundryServicesQuery : IRequest<List<LaundryServiceDTO>>;
public class GetAllLaundryServicesQueryHandler : IRequestHandler<GetAllLaundryServicesQuery, List<LaundryServiceDTO>>
{
    private readonly ILaundrySystemDbContext _context;

    public GetAllLaundryServicesQueryHandler(ILaundrySystemDbContext context)
    {
        _context = context;
    }

    public async Task<List<LaundryServiceDTO>> Handle(GetAllLaundryServicesQuery request, CancellationToken ct)
    {
        var MemberShips = await _context.LaundryServices
            .AsNoTracking()
            .ToListAsync(ct);

        var result = MemberShips.Select(x => new LaundryServiceDTO
        {
            Id = x.Id,
            Name = x.Name,
            DisplayName = x.DisplayName,
            Code = x.Code,
        }).ToList();

        return result;
    }
}

