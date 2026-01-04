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

namespace DHAFacilitationAPIs.Application.Feature.User.Commands.SetDriverStatus;
public record SetDriverStatusCommand(Guid DriverId, bool IsActive) : IRequest<Unit>;

public class SetDriverStatusCommandHandler
    : IRequestHandler<SetDriverStatusCommand, Unit>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _context;

    public SetDriverStatusCommandHandler(
        UserManager<ApplicationUser> userManager,
        IApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<Unit> Handle(SetDriverStatusCommand request, CancellationToken ct)
    {
        var driver = await _userManager.Users
            .FirstOrDefaultAsync(x => x.Id == request.DriverId.ToString()
                && x.UserType == UserType.Driver
                && !x.IsDeleted, ct)
            ?? throw new NotFoundException("Driver not found.");

        driver.IsActive = request.IsActive;

        await _userManager.UpdateAsync(driver);
        await _context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}

