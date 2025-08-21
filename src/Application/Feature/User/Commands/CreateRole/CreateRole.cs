using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DHAFacilitationAPIs.Application.Feature.User.Commands.CreateRole;
public record CreateRoleCommand(string Name, bool IsSystemRole)
    : IRequest<SuccessResponse<AppRole>>;

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, SuccessResponse<AppRole>>
{
    private readonly IApplicationDbContext _context;

    public CreateRoleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<AppRole>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = new AppRole
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            IsSystemRole = request.IsSystemRole
        };

        _context.AppRoles.Add(role);
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<AppRole>(role, "Role created successfully");
    }
}

