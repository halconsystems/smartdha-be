using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.User.Commands.ActivateDeactivateUser;
public record ActivateDeactivateUserCommand(string UserId, bool IsActive) : IRequest<bool>;

public class ActivateDeactivateUserHandler : IRequestHandler<ActivateDeactivateUserCommand, bool>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ActivateDeactivateUserHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> Handle(ActivateDeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user == null)
            throw new NotFoundException("User not found", request.UserId);

        user.IsActive = request.IsActive;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }
}

