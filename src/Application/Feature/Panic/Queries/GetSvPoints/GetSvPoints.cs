using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetSvPoints;
public record GetSvPointsQuery() : IRequest<List<SvPointDto>>;
public class GetSvPointsQueryHandler
    : IRequestHandler<GetSvPointsQuery, List<SvPointDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSvPointsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SvPointDto>> Handle(GetSvPointsQuery request, CancellationToken ct)
    {
        return await _context.SvPoints
            .Where(p => p.IsActive==true)
            .Select(p => new SvPointDto
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                Description=p.Description,
                Address=p.Address,
                IsActive=p.IsActive
            })
            .ToListAsync(ct);
    }
}
