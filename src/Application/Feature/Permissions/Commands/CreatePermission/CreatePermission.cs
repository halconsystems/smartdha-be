using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.Permissions.Commands.CreatePermission;
public record CreatePermissionCommand: IRequest<SuccessResponse<Guid>>
{
    public Guid SubModuleId { get; set; }
    public string Value { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}

public class CreatePermissionCommandHandler : IRequestHandler<CreatePermissionCommand, SuccessResponse<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreatePermissionCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<Guid>> Handle(CreatePermissionCommand request, CancellationToken ct)
    {
        var perm = new AppPermission
        {
            Id = Guid.NewGuid(),
            SubModuleId = request.SubModuleId,
            Value = request.Value,
            DisplayName = request.DisplayName
        };

        _context.AppPermissions.Add(perm);
        await _context.SaveChangesAsync(ct);
        return new SuccessResponse<Guid>(perm.Id,"Permission created successfully!");
    }
}

