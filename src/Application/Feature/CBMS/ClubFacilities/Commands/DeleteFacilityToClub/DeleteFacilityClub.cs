using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Commands.DeleteFacilityToClub;

public record DeleteFacilityClubCommand(Guid Id)
    : IRequest<ApiResult<bool>>;
public class DeleteFacilityClubCommandHandler
    : IRequestHandler<DeleteFacilityClubCommand, ApiResult<bool>>
{
    private readonly ICBMSApplicationDbContext _db;

    public DeleteFacilityClubCommandHandler(ICBMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        DeleteFacilityClubCommand request,
        CancellationToken ct)
    {
        var entity = await _db.Set<Domain.Entities.CBMS.ClubFacility>()
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity == null)
            return ApiResult<bool>.Fail("Club Facility not found.");

        entity.IsDeleted = true;
        entity.IsActive = false;

        await _db.SaveChangesAsync(ct);

        return ApiResult<bool>.Ok(true, "Club Facility deleted successfully.");
    }
}
