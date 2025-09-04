using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Dropdown.Queries.GetDropdown;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using static System.Reflection.Metadata.BlobBuilder;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.Clubs.Queries.GetClubById;
public record GetClubByIdQuery(Guid Id) : IRequest<SuccessResponse<ClubDto>>;

public class GetClubByIdQueryHandler: IRequestHandler<GetClubByIdQuery, SuccessResponse<ClubDto>>
{
    private readonly IOLMRSApplicationDbContext _ctx;       // Clubs DbContext
    private readonly IApplicationDbContext _appCtx;         // Auth/Assignments DbContext
    private readonly ICurrentUserService _currentUser;

    public GetClubByIdQueryHandler(
        IOLMRSApplicationDbContext ctx,
        IApplicationDbContext appCtx,
        ICurrentUserService currentUser)
    {
        _ctx = ctx;
        _appCtx = appCtx;
        _currentUser = currentUser;
    }

    public async Task<SuccessResponse<ClubDto>> Handle(GetClubByIdQuery request, CancellationToken ct)
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

        IQueryable<Club> query = _ctx.Clubs
            .AsNoTracking()
            .Where(c => c.IsActive == true && c.IsDeleted != true);

        if (!isSuperAdmin)
        {
            // 2️⃣ Normal users: restrict to assigned clubs
            var assignedClubIds = await _appCtx.UserClubAssignments
                .Where(uca => uca.UserId == userId)
                .Select(uca => uca.ClubId)
                .ToListAsync(ct);

            query = query.Where(c => assignedClubIds.Contains(c.Id));
        }

        // 3️⃣ Apply ID filter
        var dto = await query
            .Where(c => c.Id == request.Id)
            .Select(c => new ClubDto(
                c.Id,
                c.Name,
                c.Description,
                c.Location,
                c.ContactNumber,
                c.IsActive,
                c.IsDeleted
            ))
            .FirstOrDefaultAsync(ct);

        if (dto is null)
            throw new NotFoundException(request.Id.ToString() +" Not Found!");

        return Success.Ok(dto);
    }
}

