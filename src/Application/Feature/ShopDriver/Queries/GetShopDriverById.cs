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

using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

public record GetShopDriverByIdQuery(Guid DriverId) : IRequest<ShopDriverListDTO>;
public class GetDriverByIdQueryHandler
    : IRequestHandler<GetShopDriverByIdQuery, ShopDriverListDTO>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetDriverByIdQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ShopDriverListDTO> Handle(GetShopDriverByIdQuery request, CancellationToken ct)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(x => x.Id == request.DriverId.ToString()
                && x.UserType == UserType.ShopDriver
                && !x.IsDeleted,
                ct);

        if (user == null) throw new NotFoundException("Driver not found.");

        return new ShopDriverListDTO
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

