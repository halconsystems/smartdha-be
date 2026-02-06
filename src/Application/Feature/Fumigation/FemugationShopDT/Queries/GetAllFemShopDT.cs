using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryShopDT;
using Microsoft.AspNetCore.Hosting;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.FemugationShopDT.Queries;

internal class GetAllFemShopDT
{
}
public record GetAllFemShopDTQuery : IRequest<List<FemDTSettingDTO>>;
public class GetAllFemShopDTQueryHandler : IRequestHandler<GetAllFemShopDTQuery, List<FemDTSettingDTO>>
{
    private readonly IApplicationDbContext _context;

    public GetAllFemShopDTQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<FemDTSettingDTO>> Handle(GetAllFemShopDTQuery request, CancellationToken ct)
    {
        var MemberShips = await _context.ShopDTSettings
            .Include(x => x.Shop)
            .Include(x => x.FemDTSetting)
            .AsNoTracking()
            .ToListAsync(ct);


        var result = MemberShips.Select(x => new FemDTSettingDTO
        {
            Id = x.Id,
            Shop = x.Shop,
            FemDTSettings = x.FemDTSetting,
            Value = x.Value,
        }).ToList();

        return result;
    }
}

