using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetAllDrivers;
public record GetAllDriversQuery() : IRequest<List<DriverListDto>>;

public class GetAllDriversQueryHandler : IRequestHandler<GetAllDriversQuery, List<DriverListDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetAllDriversQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<DriverListDto>> Handle(GetAllDriversQuery request, CancellationToken ct)
    {
        var drivers = await _userManager.Users
            .Where(x => x.UserType == UserType.Driver && x.IsDeleted == false)
            .Select(x => new DriverListDto
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

