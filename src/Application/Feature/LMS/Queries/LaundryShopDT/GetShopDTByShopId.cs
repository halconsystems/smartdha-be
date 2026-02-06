using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryShopDT;


public record GetShopDTByShopIdQuery(Guid ShopId) : IRequest<List<ShopDTdto>>;
public class GetShopDTByShopIdQueryHandler : IRequestHandler<GetShopDTByShopIdQuery, List<ShopDTdto>>
{
    private readonly ILaundrySystemDbContext _context;

    public GetShopDTByShopIdQueryHandler(ILaundrySystemDbContext context)
    {
        _context = context;
    }

    public async Task<List<ShopDTdto>> Handle(GetShopDTByShopIdQuery request, CancellationToken ct)
    {
        var MemberShips = await _context.ShopDTSettings
            .Where(x => x.ShopId == request.ShopId)
            .Include(x => x.Shop)
            .Include(x => x.OrderDTSetting)
            .ToListAsync(ct);

        if (MemberShips is null) throw new KeyNotFoundException("Shop Discount not found.");

        var result = MemberShips.Select(x => new ShopDTdto
        {
            ShopId = request.ShopId,
            Id = x.Id,
            Shop = x.Shop,
            OrderDTSetting = x.OrderDTSetting,
            Value = x.Value
        }).ToList();

        return result;
    }
}


