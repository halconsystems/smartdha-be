using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetAllDrivers;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.ShopDriver.Queries;

public record GetAllShopDriversQuery() : IRequest<List<ShopDriverListDTO>>;

public class GetAllShopDriversQueryHandler : IRequestHandler<GetAllShopDriversQuery, List<ShopDriverListDTO>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetAllShopDriversQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<ShopDriverListDTO>> Handle(GetAllShopDriversQuery request, CancellationToken ct)
    {
        var drivers = await _userManager.Users
            .Where(x => x.UserType == UserType.ShopDriver && x.IsDeleted == false)
            .Select(x => new ShopDriverListDTO
            {
                Id = Guid.Parse(x.Id),
                Name = x.Name,
                Email = x.Email!,
                CNIC = x.CNIC!,
                MobileNo = x.MobileNo!,
                IsActive = x.IsActive
            })
            .ToListAsync(ct);


        return drivers;
    }
}


