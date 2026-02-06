using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryShopDT;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.FemugationShopDT.Queries;

public record GetFemShopDTbyIdQuery(Guid Id) : IRequest<FemDTSettingDTO>;
public class GetFemShopDTbyIdQueryHandler : IRequestHandler<GetFemShopDTbyIdQuery, FemDTSettingDTO>
{
    private readonly IApplicationDbContext _context;

    public GetFemShopDTbyIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<FemDTSettingDTO> Handle(GetFemShopDTbyIdQuery request, CancellationToken ct)
    {
        var MemberShips = await _context.ShopDTSettings
            .Where(x => x.Id == request.Id)
            .Include(x => x.Shop)
            .Include(x => x.FemDTSetting)
            .FirstOrDefaultAsync(ct);

        if (MemberShips is null) throw new KeyNotFoundException("Shop Discount not found.");

        var result = new FemDTSettingDTO
        {
            Id = MemberShips.Id,
            Shop = MemberShips.Shop,
            FemDTSettings = MemberShips.FemDTSetting,
            Value = MemberShips.Value
        };

        return result;
    }
}

