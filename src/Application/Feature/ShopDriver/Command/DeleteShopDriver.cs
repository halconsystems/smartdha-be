using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.ShopDriver.Command;

using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

public record DeleteShopDriverCommand(Guid DriverId) : IRequest<Unit>;
public class DeleteShopDriverCommandHandler
    : IRequestHandler<DeleteShopDriverCommand, Unit>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _context;

    public DeleteShopDriverCommandHandler(
        UserManager<ApplicationUser> userManager,
        IApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<Unit> Handle(DeleteShopDriverCommand request, CancellationToken ct)
    {
        var driver = await _userManager.Users
            .FirstOrDefaultAsync(x => x.Id == request.DriverId.ToString()
                && x.UserType == UserType.ShopDriver
                && !x.IsDeleted, ct)
            ?? throw new NotFoundException("Driver not found.");

        driver.IsDeleted = true;
        driver.IsActive = false;

        await _userManager.UpdateAsync(driver);
        await _context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}

