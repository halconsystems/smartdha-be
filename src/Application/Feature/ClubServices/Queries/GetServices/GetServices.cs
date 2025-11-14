using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.ClubServices.Queries.GetServices;
public record GetServicesQuery(Guid ClubId, ServiceType ServiceType)
    : IRequest<SuccessResponse<List<ServiceDto>>>;

public class GetServicesQueryHandler
    : IRequestHandler<GetServicesQuery, SuccessResponse<List<ServiceDto>>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    private readonly IMapper _mapper;
    private readonly IApplicationDbContext _appCtx;
    private readonly ICurrentUserService _currentUser;

    public GetServicesQueryHandler(IOLMRSApplicationDbContext ctx, IMapper mapper, IApplicationDbContext appCtx, ICurrentUserService currentUser)
        => (_ctx, _mapper, _appCtx, _currentUser) = (ctx, mapper, appCtx, currentUser);

    public async Task<SuccessResponse<List<ServiceDto>>> Handle(GetServicesQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId.ToString();
        if (string.IsNullOrEmpty(userId))
            throw new UnAuthorizedException("Invalid user context.");

        // Get user roles
        var roles = await _appCtx.AppUserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role.Name)
            .ToListAsync(ct);

        bool isSuperAdmin = roles.Contains("SuperAdministrator");

        //  If user is NOT superadmin → validate club assignment
        if (!isSuperAdmin)
        {
            bool isUserAssignedToClub = await _appCtx.UserClubAssignments
                .AnyAsync(uca => uca.UserId == userId && uca.ClubId == request.ClubId, ct);

            if (!isUserAssignedToClub)
                throw new UnAuthorizedException("You are not assigned to this club.");
        }

        var q = _ctx.Services
            .AsNoTracking()
            .Where(x =>
                (x.IsDeleted == null || x.IsDeleted == false) &&
                x.ServiceType == request.ServiceType &&
                x.ClubId == request.ClubId);


        var items = await q
            .OrderBy(x => x.Name)
            .ProjectTo<ServiceDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return Success.Ok(items);
    }
}

