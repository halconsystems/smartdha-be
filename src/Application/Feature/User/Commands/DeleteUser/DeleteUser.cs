using DHAFacilitationAPIs.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

public record DeleteUserCommand(string UserId) : IRequest<bool>;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public DeleteUserHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user == null)
            throw new NotFoundException("User not found", request.UserId);

        user.IsDeleted = true;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }
}

