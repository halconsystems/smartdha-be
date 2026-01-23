using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Fumigation.Phases.Queries;
using DHAFacilitationAPIs.Application.Feature.GroundReservations.Queries;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.Services.Queries;

public record GetAllServicesQuery : IRequest<List<ServiceDTO>>;
public class GetAllServicesQueryHandler : IRequestHandler<GetAllServicesQuery, List<ServiceDTO>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAllServicesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<ServiceDTO>> Handle(GetAllServicesQuery request, CancellationToken ct)
    {
        var Phases = await _context.FemServices
            .AsNoTracking()
            .ToListAsync(ct);


        if (Phases == null)
            throw new KeyNotFoundException("Phases Not Found.");


        var result = Phases.Select(x => new ServiceDTO
        {
            Id = x.Id,
            Name = x.Name,
            DisplayName = x.DisplayName,
            Code = x.Code,
            IsActive = x.IsActive
        }).ToList();

        return result;
    }
}






