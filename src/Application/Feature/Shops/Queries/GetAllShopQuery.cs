using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryPackaging;

namespace DHAFacilitationAPIs.Application.Feature.Shops.Queries;

public record GetAllShopQueryQuery : IRequest<List<ShopQueryDTO>>;
public class GetAllShopQueryQueryHandler : IRequestHandler<GetAllShopQueryQuery, List<ShopQueryDTO>>
{
    private readonly ILaundrySystemDbContext _context;

    public GetAllShopQueryQueryHandler(ILaundrySystemDbContext context)
    {
        _context = context;
    }

    public async Task<List<ShopQueryDTO>> Handle(GetAllShopQueryQuery request, CancellationToken ct)
    {
        var shops = await _context.Shops
            .AsNoTracking()
            .ToListAsync(ct);

        var result = shops.Select(x => new ShopQueryDTO
        {
            Id = x.Id,   
            Name = x.Name,
            DisplayName = x.DisplayName,
            Code = x.Code,
            Address = x.Address,
            Latitude = x.Latitude,
            Longitude = x.Longitude,    
            OwnerName = x.OwnerName,
            OwnerEmail = x.OwnerEmail,
            OwnerPhone = x.OwnerPhone,
            ShopPhoneNumber = x.ShopPhoneNumber,
        }).ToList();

        return result;
    }
}



