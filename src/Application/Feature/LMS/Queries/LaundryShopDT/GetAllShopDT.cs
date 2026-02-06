using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryService;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryShopDT;

public record GetAllShopDTQuery : IRequest<List<ShopDTdto>>;
public class GetAllShopDTQueryHandler : IRequestHandler<GetAllShopDTQuery, List<ShopDTdto>>
{
    private readonly ILaundrySystemDbContext _context;

    public GetAllShopDTQueryHandler(ILaundrySystemDbContext context)
    {
        _context = context;
    }

    public async Task<List<ShopDTdto>> Handle(GetAllShopDTQuery request, CancellationToken ct)
    {
        var MemberShips = await _context.ShopDTSettings
            .Include(x => x.Shop)
            .Include(x => x.OrderDTSetting)
            .AsNoTracking()
            .ToListAsync(ct);
        

        var result = MemberShips.Select(x => new ShopDTdto
        {
            Id = x.Id,
            Shop = x.Shop,
            OrderDTSetting = x.OrderDTSetting,
            Value = x.Value,
        }).ToList();

        return result;
    }
}

