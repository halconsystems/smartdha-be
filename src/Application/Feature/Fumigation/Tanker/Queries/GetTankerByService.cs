using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.Tanker.Queries;
public record GetTankerByServiceQuery(Guid ServiceId)
    : IRequest<List<TankerDTO>>;
public class GetTankerByServiceQueryHandler
    : IRequestHandler<GetTankerByServiceQuery, List<TankerDTO>>
{
    private readonly IApplicationDbContext _context;

    public GetTankerByServiceQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TankerDTO>> Handle(
        GetTankerByServiceQuery request,
        CancellationToken ct)
    {
        var tanker = await _context.TankerSizes
            .AsNoTracking()
            .Where(x => x.FemServiceId == request.ServiceId)
            .ToListAsync(ct);

        if (!tanker.Any())
            throw new KeyNotFoundException("No tanker sizes found for this service.");

        return tanker.Select(x => new TankerDTO
        {
            Id = x.Id,
            Name = x.Name,
            DisplayName = x.DisplayName,
            Code = x.Code,
            Price = x.Price,
            IsActive = x.IsActive,
            ServiceId = x.FemServiceId.ToString(),
            ServiceName = x.FemService?.Name
        }).ToList();
    }
}

