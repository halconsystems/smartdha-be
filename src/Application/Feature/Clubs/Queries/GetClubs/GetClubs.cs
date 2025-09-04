using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.Clubs.Queries.GetClubs;
public record GetClubsQuery()
    : IRequest<SuccessResponse<List<ClubDto>>>;

public class GetClubsQueryHandler
    : IRequestHandler<GetClubsQuery, SuccessResponse<List<ClubDto>>>
{
    private readonly IOLMRSApplicationDbContext _ctx;       // Clubs DbContext
    private readonly IApplicationDbContext _appCtx;         // Auth/Assignments DbContext
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public GetClubsQueryHandler(
       IOLMRSApplicationDbContext ctx,
       IApplicationDbContext appCtx,
       ICurrentUserService currentUser,
       IMapper mapper)
    {
        _ctx = ctx;
        _appCtx = appCtx;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<SuccessResponse<List<ClubDto>>> Handle(GetClubsQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId.ToString();
        if (string.IsNullOrEmpty(userId))
            throw new UnAuthorizedException("Invalid user context.");

        // 1️⃣ Get current roles
        var roles = await _appCtx.AppUserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role.Name)
            .ToListAsync(ct);

        bool isSuperAdmin = roles.Contains("SuperAdministrator");

        // 2️⃣ Base query
        IQueryable<Club> query = _ctx.Clubs
            .AsNoTracking()
            .Where(c => c.IsDeleted == null || c.IsDeleted == false)
            .OrderBy(c => c.Name);

        // 3️⃣ Restrict to assigned clubs if not superadmin
        if (!isSuperAdmin)
        {
            var assignedClubIds = await _appCtx.UserClubAssignments
                .Where(uca => uca.UserId == userId)
                .Select(uca => uca.ClubId)
                .ToListAsync(ct);

            query = query.Where(c => assignedClubIds.Contains(c.Id));
        }

        // 4️⃣ Project to DTO
        var clubs = await query
            .ProjectTo<ClubDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return Success.Ok(clubs);
    }

}


