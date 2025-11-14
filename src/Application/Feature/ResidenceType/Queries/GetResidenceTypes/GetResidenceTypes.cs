using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.ResidenceType.Queries;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DHAFacilitationAPIs.Application.Feature.CreateResidenceType.Queries.GetResidenceTypes;
public record GetResidenceTypesQuery(Guid ClubId, ClubType ClubType) : IRequest<SuccessResponse<List<ResidenceTypeDto>>>;

public class GetResidenceTypesQueryHandler
    : IRequestHandler<GetResidenceTypesQuery, SuccessResponse<List<ResidenceTypeDto>>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    private readonly IMapper _mapper;
    private readonly IApplicationDbContext _appCtx;
    private readonly ICurrentUserService _currentUser;

    public GetResidenceTypesQueryHandler(IOLMRSApplicationDbContext ctx, IMapper mapper, IApplicationDbContext appCtx, ICurrentUserService currentUser)
    {
        _ctx = ctx;
        _mapper = mapper;
        _appCtx = appCtx;
        _currentUser = currentUser;
    }

    public async Task<SuccessResponse<List<ResidenceTypeDto>>> Handle(GetResidenceTypesQuery request, CancellationToken ct)
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

        // Fetch only residence types for this club & club type
        var query = _ctx.ResidenceTypes.AsNoTracking()
            .Where(x =>
                (x.IsDeleted == null || x.IsDeleted == false) &&
                x.ClubType == request.ClubType &&
                x.ClubId == request.ClubId);

        var list = await query
            .OrderBy(x => x.Name)
            .ProjectTo<ResidenceTypeDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return Success.Ok(list);
        // If you have the helper: return Success.Ok(list);
    }
}
