using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Fumigation.FemugationShopDT.Queries;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryShopDT;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.FemugationShopDT.Queries;

public record GetFemDTBYShopIdQuery(Guid ShopId) : IRequest<List<FemDTSettingDTO>>;
public class GetFemDTBYShopIdQueryHandler : IRequestHandler<GetFemDTBYShopIdQuery, List<FemDTSettingDTO>>
{
    private readonly IApplicationDbContext _context;

    public GetFemDTBYShopIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<FemDTSettingDTO>> Handle(GetFemDTBYShopIdQuery request, CancellationToken ct)
    {
        var MemberShips = await _context.ShopDTSettings
            .Where(x => x.ShopId == request.ShopId)
            .Include(x => x.Shop)
            .Include(x => x.FemDTSetting)
            .ToListAsync(ct);

        if (MemberShips is null) throw new KeyNotFoundException("Shop Discount not found.");

        var result = MemberShips.Select(x => new FemDTSettingDTO
        {
            ShopId = request.ShopId,
            Id = x.Id,
            Shop = x.Shop,
            FemDTSettings = x.FemDTSetting,
            Value = x.Value
        }).ToList();

        return result;
    }
}


