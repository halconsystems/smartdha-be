using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Feature.ShopDriver.Queries;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;

using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.ShopOwner.Queries;

public record GetShopOwnerByIdQuery(Guid OwnerId) : IRequest<ShopOwnerDTO>;
public class GetShopOwnerByIdQueryHandler
    : IRequestHandler<GetShopOwnerByIdQuery, ShopOwnerDTO>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetShopOwnerByIdQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ShopOwnerDTO> Handle(GetShopOwnerByIdQuery request, CancellationToken ct)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(x => x.Id == request.OwnerId.ToString()
                && x.UserType == UserType.ShopOwner
                && !x.IsDeleted,
                ct);

        if (user == null) throw new NotFoundException("Owner not found.");

        return new ShopOwnerDTO
        {
            Id = Guid.Parse(user.Id),
            Name = user.Name,
            Email = user.Email!,
            CNIC = user.CNIC!,
            MobileNo = user.MobileNo!,
            IsActive = user.IsActive
        };
    }
}

