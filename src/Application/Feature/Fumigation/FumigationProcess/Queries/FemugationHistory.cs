using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Entities.FMS;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.FumigationProcess.Queries;



public record FemugationHistoryQuery(Guid Id) : IRequest<FemugationDTO>;
public class FemugationHistoryQueryHandler : IRequestHandler<FemugationHistoryQuery, FemugationDTO>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileStorageService _file;

    public FemugationHistoryQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, IFileStorageService file)
    {
        _context = context;
        _currentUser = currentUser;
        _file = file;
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

        var shop = await _context.FemgutionShops
            .FirstOrDefaultAsync(x => x.Id == femugation.ShopId, ct);

        var tankSize = await _context.TankerSizes
            .Where(x => x.Id == femugation.FemTanker)
            .FirstOrDefaultAsync(ct);

        var femMedia = await _context.FumgationMedias
            .Where(x => x.FemugationId == request.Id)
            .AsNoTracking()
            .ToListAsync();

        var imageURL = femMedia
            .Select(x => new
            {
                x.FilePath,
                x.Caption
            })
            .ToList();

            Dictionary<string, string> publicUrls = imageURL
            .Where(img => !string.IsNullOrEmpty(img.FilePath))
            .ToDictionary(
                img => img.Caption ?? string.Empty,
                img => _file.GetPublicUrl(img.FilePath)
            );

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
            FemgutionShops = shop,
            DriverRemarksAudioPath = femugation.DriverRemarksAudioPath,
            CaseNo = femugation.CaseNo,
            CreatedAt = femugation.Created,
            FemStatus = femugation.FemStatus,
            Total = femugation.Total,
            SubTotal = femugation.SubTotal,
            ShopName = shop?.Name,
            AssginedAt = femugation.AssginedAt,
            AcknowledgedAt = femugation.AcknowledgedAt,
            ResolvedAt = femugation.ResolvedAt,
            CancelledAt = femugation.CancelledAt,
            Media = femMedia == null ? new List<FumgationMedia>() : femMedia,
            Images = publicUrls == null ? new Dictionary<string, string>() : publicUrls,
            TankSize = tankSize?.Name,
            Taxes = femugation.Taxes.ToString(),
            Discount = femugation.Discount,
            PhoneNumber = femugation.PhoneNumber,
            StreetNo = femugation.StreetNo,
            DateOnly = femugation.DateOnly,
            TimeOnly = femugation.TimeOnly,
            AmountToCollect = femugation.AmountToCollect,
            CollectedAmount = femugation.CollectedAmount,
            PaymentMethod = femugation.PaymentMethod,
            Address = femugation.Address,
    };

        return result;
    }
}







