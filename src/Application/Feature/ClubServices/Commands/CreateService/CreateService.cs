using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.ClubServices.Commands.CreateService;
public record CreateServiceCommand(string Name, string? Description, ServiceType ServiceType, Guid ClubId)
    : IRequest<SuccessResponse<string>>;

public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, SuccessResponse<string>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    private readonly IApplicationDbContext _appCtx;
    private readonly ICurrentUserService _currentUser;
    public CreateServiceCommandHandler(IOLMRSApplicationDbContext ctx, IApplicationDbContext appCtx, ICurrentUserService currentUser) 
        => (_ctx, _appCtx, _currentUser) = (ctx, appCtx, currentUser);

    public async Task<SuccessResponse<string>> Handle(CreateServiceCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId.ToString();
        if (string.IsNullOrEmpty(userId))
            throw new UnAuthorizedException("Invalid user context.");

        //  Get current roles
        var roles = await _appCtx.AppUserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role.Name)
            .ToListAsync(ct);

        bool isSuperAdmin = roles.Contains("SuperAdministrator");

        //  If user is NOT superadmin → validate club assignment
        if (!isSuperAdmin)
        {
            bool assignedToClub = await _appCtx.UserClubAssignments
                .AnyAsync(uca => uca.UserId == userId && uca.ClubId == request.ClubId, ct);

            if (!assignedToClub)
                throw new UnAuthorizedException("You are not assigned to this club and cannot create residence types for it.");
        }

        var entity = new Services
        {
            Name = request.Name,
            Description = request.Description,
            ClubId = request.ClubId,
            ServiceType = request.ServiceType,
            IsActive = true,
            IsDeleted = false,
            Created = DateTime.Now
        };
        _ctx.Services.Add(entity);
        await _ctx.SaveChangesAsync(ct);
        return Success.Created(entity.Id.ToString());
    }
}
