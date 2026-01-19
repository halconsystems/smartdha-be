using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Orders.Queries;
using DHAFacilitationAPIs.Application.Feature.Shops.Queries;

namespace DHAFacilitationAPIs.Application.Feature.ShopVehicles.Queries;


public record GetShopVehiclesQuery : IRequest<List<ShopVehicleDTO>>;
public class GetShopVehiclesQueryHandler : IRequestHandler<GetShopVehiclesQuery, List<ShopVehicleDTO>>
{
    private readonly ILaundrySystemDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IApplicationDbContext _application;

    public GetShopVehiclesQueryHandler(ILaundrySystemDbContext context, ICurrentUserService currentUser,IApplicationDbContext application)
    {
        _context = context;
        _currentUser = currentUser;
        _application = application;
    }

    public async Task<List<ShopVehicleDTO>> Handle(GetShopVehiclesQuery request, CancellationToken ct)
    {
        var ShopVehicles =await _context.ShopVehicles
            .AsNoTracking()
            .ToListAsync();
        var shops = _context.Shops.Where(x => ShopVehicles.Select(x => x.ShopId).Contains(x.Id)).AsNoTracking().ToList();

        if(shops == null)
            throw new NotFoundException("Shops not Found.","NotFound");

        var result =ShopVehicles.Select(x => new ShopVehicleDTO
        {
            Id = x.Id,
            Name = x.Name,
            DriverUserId = x.DriverUserId,
            RegistrationNo = x.RegistrationNo,
            VehicleType = x.VehicleType,
            MapIconKey = x.MapIconKey,
            Status = x.Status,
            ShopQuery = shops
                .Select(x => new ShopQueryDTO
                {
                    Id = x.Id,
                    ShopPhoneNumber = x.ShopPhoneNumber,
                    Name = x.Name,
                    DisplayName = x.DisplayName,
                    OwnerName = x.OwnerName,
                    OwnerEmail = x.OwnerEmail,
                    OwnerPhone = x.OwnerPhone,

                }).ToList(),
        }).ToList();
        return result;
    }
}




