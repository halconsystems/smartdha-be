using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities.FMS;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.FumigationProcess.Queries;



public record FemugationHistoryQuery(Guid Id) : IRequest<FemugationDTO>;
public class FemugationHistoryQueryHandler : IRequestHandler<FemugationHistoryQuery, FemugationDTO>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public FemugationHistoryQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<FemugationDTO> Handle(FemugationHistoryQuery request, CancellationToken ct)
    {
        var femugation = await _context.Fumigations
            .Include(x => x.FemService)
            .Include(x => x.FemPhase)
            .Include(x => x.TankerSize)
            .Include(x => x.FemgutionShops)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id,ct);

        if (femugation == null)
            throw new KeyNotFoundException("Femugation Not Found.");

        var femMedia = await _context.FumgationMedias
            .Where(x => x.FemugationId == request.Id)
            .AsNoTracking()
            .ToListAsync();


        var result = new FemugationDTO
        {
            Id = femugation.Id,
            UserId = femugation.UserId,
            FemPhaseID = femugation.FemPhaseID,
            FemPhase = femugation.FemPhase,
            FemServiceId = femugation.FemServiceId,
            FemService = femugation.FemService,
            FemTanker = femugation.FemTanker,
            TankerSize = femugation.TankerSize,
            FemgutionShops = femugation.FemgutionShops,
            DriverRemarksAudioPath = femugation.DriverRemarksAudioPath,
            CaseNo = femugation.CaseNo,
            CreatedAt = femugation.Created,
            FemStatus = femugation.FemStatus,
            Total = femugation.Total,
            SubTotal = femugation.SubTotal,
            ShopName = femugation.FemgutionShops?.Name,
            AssginedAt = femugation.AssginedAt,
            AcknowledgedAt = femugation.AcknowledgedAt,
            ResolvedAt = femugation.ResolvedAt,
            CancelledAt = femugation.CancelledAt,
            Media = femMedia == null ? new List<FumgationMedia>() : femMedia,
        };

        return result;
    }
}







