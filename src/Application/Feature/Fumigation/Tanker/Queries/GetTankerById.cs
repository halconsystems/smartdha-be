using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Fumigation.Services.Queries;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.Tanker.Queries;

public record GetTankerByIdQuery(Guid Id) : IRequest<TankerDTO>;
public class GetTankerByIdQueryHandler : IRequestHandler<GetTankerByIdQuery, TankerDTO>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetTankerByIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<TankerDTO> Handle(GetTankerByIdQuery request, CancellationToken ct)
    {
        var tanker = await _context.TankerSizes
           .FirstOrDefaultAsync(x => x.Id == request.Id);
        var FemServices = await _context.FemServices
         .AsNoTracking()
         .ToListAsync(ct);

        if (tanker == null)
            throw new KeyNotFoundException("Service Not Found.");


        var result = new TankerDTO
        {
            Id = tanker.Id,
            Name = tanker.Name,
            DisplayName = tanker.DisplayName,
            Code = tanker.Code,
            Price = tanker.Price,
            IsActive = tanker.IsActive,
            ServiceId= tanker.FemServiceId.ToString(),
            ServiceName = FemServices.FirstOrDefault(y => y.Id == tanker.FemServiceId)?.Name
        };

        return result;
    }
}








