using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetAllDrivers;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetDriverById;
public record GetDriverByIdQuery(Guid DriverId) : IRequest<DriverListDto>;
public class GetDriverByIdQueryHandler
    : IRequestHandler<GetDriverByIdQuery, DriverListDto>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetDriverByIdQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<DriverListDto> Handle(GetDriverByIdQuery request, CancellationToken ct)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(x => x.Id == request.DriverId.ToString()
                && x.UserType == UserType.Driver
                && !x.IsDeleted,
                ct);

        if (user == null) throw new NotFoundException("Driver not found.");

        return new DriverListDto
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

