using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Fumigation.Services.Queries;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.Tanker.Queries;

public record GetAllTankerQuery : IRequest<List<TankerDTO>>;
public class GetAllTankerQueryHandler : IRequestHandler<GetAllTankerQuery, List<TankerDTO>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAllTankerQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<TankerDTO>> Handle(GetAllTankerQuery request, CancellationToken ct)
    {
        var tanker = await _context.TankerSizes
            .AsNoTracking()
            .ToListAsync(ct);


        if (tanker == null)
            throw new KeyNotFoundException("Phases Not Found.");


        var result = tanker.Select(x => new TankerDTO
        {
            Id = x.Id,
            Name = x.Name,
            DisplayName = x.DisplayName,
            Code = x.Code,
            Price = x.Price,
            IsActive = x.IsActive
        }).ToList();

        return result;
    }
}






