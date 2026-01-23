using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Fumigation.Services.Queries;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.Phases.Queries;

public record GetPhaseByIdQuery(Guid Id) : IRequest<PhaseDTO>;
public class GetPhaseByIdQueryHandler : IRequestHandler<GetPhaseByIdQuery, PhaseDTO>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetPhaseByIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PhaseDTO> Handle(GetPhaseByIdQuery request, CancellationToken ct)
    {
        var phase = await _context.FemPhases
           .FirstOrDefaultAsync(x => x.Id == request.Id);


        if (phase == null)
            throw new KeyNotFoundException("Phases Not Found.");


        var result = new PhaseDTO
        {
            Id = phase.Id,
            Name = phase.Name,
            DisplayName = phase.DisplayName,
            Code =  phase.Code,
            IsActive = phase.IsActive,
        };

        return result;
    }
}








