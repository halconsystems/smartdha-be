using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Feature.ShopDriver.Queries;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.ShopOwner.Queries;


public record GetAllShopOwnerQuery() : IRequest<List<ShopOwnerDTO>>;

public class GetAllShopOwnerQueryHandler : IRequestHandler<GetAllShopOwnerQuery, List<ShopOwnerDTO>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetAllShopOwnerQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<ShopOwnerDTO>> Handle(GetAllShopOwnerQuery request, CancellationToken ct)
    {
        var drivers = await _userManager.Users
            .Where(x => x.UserType == UserType.ShopOwner && x.IsDeleted == false)
            .Select(x => new ShopOwnerDTO
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


