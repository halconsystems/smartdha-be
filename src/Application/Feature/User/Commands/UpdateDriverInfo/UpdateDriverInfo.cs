using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.User.Commands.UpdateDriverInfo;
public record UpdateDriverInfoCommand(
    Guid DriverId,
    string Name,
    string MobileNo,
    string Email
) : IRequest<Unit>;
public class UpdateDriverInfoCommandHandler
    : IRequestHandler<UpdateDriverInfoCommand, Unit>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _context;

    public UpdateDriverInfoCommandHandler(
        UserManager<ApplicationUser> userManager,
        IApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<Unit> Handle(UpdateDriverInfoCommand request, CancellationToken ct)
    {
        var driver = await _userManager.Users
            .FirstOrDefaultAsync(x => x.Id == request.DriverId.ToString()
                                      && x.UserType == UserType.Driver
                                      && !x.IsDeleted, ct)
            ?? throw new NotFoundException("Driver not found.");

        driver.Name = request.Name;
        driver.MobileNo = request.MobileNo;
        driver.RegisteredMobileNo = request.MobileNo;
        driver.Email = request.Email;
        driver.NormalizedEmail = request.Email.ToUpper();
        

        await _userManager.UpdateAsync(driver);
        await _context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}

