using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryShopDT;

public record GetShopDTSettingByIdQuery(Guid Id) : IRequest<ShopDTdto>;
public class GetShopDTSettingByIdQueryHandler : IRequestHandler<GetShopDTSettingByIdQuery, ShopDTdto>
{
    private readonly ILaundrySystemDbContext _context;

    public GetShopDTSettingByIdQueryHandler(ILaundrySystemDbContext context)
    {
        _context = context;
    }

    public async Task<ShopDTdto> Handle(GetShopDTSettingByIdQuery request, CancellationToken ct)
    {
        var MemberShips = await _context.ShopDTSettings
            .Where(x => x.Id == request.Id)
            .Include(x => x.Shop)
            .Include(x => x.OrderDTSetting)
            .FirstOrDefaultAsync(ct);

        if (MemberShips is null) throw new KeyNotFoundException("Shop Discount not found.");

        var result = new ShopDTdto
        {
            Id = MemberShips.Id,
            Shop = MemberShips.Shop,
            OrderDTSetting = MemberShips.OrderDTSetting,
            Value = MemberShips.Value
        };

        return result;
    }
}

