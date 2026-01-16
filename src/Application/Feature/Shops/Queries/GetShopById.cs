using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities.LMS;

namespace DHAFacilitationAPIs.Application.Feature.Shops.Queries;

public class GetShopById
{
}

public record GetShopByIdQuery(Guid Id) : IRequest<ShopQueryDTO>;
public class GetShopByIdQueryHandler : IRequestHandler<GetShopByIdQuery, ShopQueryDTO>
{
    private readonly ILaundrySystemDbContext _context;

    public GetShopByIdQueryHandler(ILaundrySystemDbContext context)
    {
        _context = context;
    }

    public async Task<ShopQueryDTO> Handle(GetShopByIdQuery request, CancellationToken ct)
    {
        var shops = await _context.Shops
            .Where(x => x.Id == request.Id)
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        if (shops == null) 
            throw new KeyNotFoundException("Shop not found.");

        var result = new ShopQueryDTO
        {
            Id = shops.Id,
            Name = shops.Name,
            DisplayName = shops.DisplayName,
            Code = shops.Code,
            Address = shops.Address,
            Latitude = shops.Latitude,
            Longitude = shops.Longitude,
            OwnerName = shops.OwnerName,
            OwnerEmail = shops.OwnerEmail,
            OwnerPhone = shops.OwnerPhone,
            ShopPhoneNumber = shops.ShopPhoneNumber,

        };

        return result;
    }
}



