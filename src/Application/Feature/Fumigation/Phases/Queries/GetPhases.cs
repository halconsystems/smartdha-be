using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.GroundReservations.Queries;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.Phases.Queries;

public record GetPhasesQuery : IRequest<List<PhaseDTO>>;
public class GetPhasesQueryHandler : IRequestHandler<GetPhasesQuery, List<PhaseDTO>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetPhasesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<PhaseDTO>> Handle(GetPhasesQuery request, CancellationToken ct)
    {

        var Phases = await _context.FemPhases
            .AsNoTracking()
            .ToListAsync(ct);


        if (Phases == null)
            throw new KeyNotFoundException("Phases Not Found.");


        var result = Phases.Select(x => new PhaseDTO
        {
            Id = x.Id,
            Name = x.Name,
            DisplayName = x.DisplayName,
            Code = x.Code,
            IsActive = x.IsActive,
        }).ToList();

        return result;
    }
}






