using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Fumigation.Tanker.Queries;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.FumigationProcess.Queries;

public record GetAllFemugationListQuery : IRequest<List<FemugationDTO>>;
public class GetAllFemugationListQueryHandler : IRequestHandler<GetAllFemugationListQuery, List<FemugationDTO>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAllFemugationListQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<FemugationDTO>> Handle(GetAllFemugationListQuery request, CancellationToken ct)
    {
        var tanker = await _context.Fumigations
            .Include(x => x.FemService)
            .Include(x => x.FemPhase)
            .Include(x => x.TankerSize)
            .Include(x => x.FemgutionShops)
            .AsNoTracking()
            .ToListAsync(ct);

        var tankSize = await _context.TankerSizes
            .Where(x => tanker.Select(x => x.FemTanker).Contains(x.Id))
            .ToListAsync(ct);


        if (tanker == null)
            throw new KeyNotFoundException("Phases Not Found.");


        var result = tanker.Select(x => new FemugationDTO
        {
            Id = x.Id,
            UserId = x.UserId,
            FemPhaseID = x.FemPhaseID,
            CaseNo = x.CaseNo,
            CreatedAt = x.Created,
            FemStatus = x.FemStatus,
            Total = x.Total,
            SubTotal = x.SubTotal,
            ShopName = x.FemgutionShops?.Name,
            TankSize = tankSize?.FirstOrDefault(t => t.Id == x.FemTanker)?.Name,
            IsActive = x.IsActive,
            Address = x.Address,
            StreetNo = x.StreetNo,
            PhoneNumber = x.PhoneNumber
        }).ToList();

        return result;
    }
}







