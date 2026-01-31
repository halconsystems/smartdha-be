using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Command;
public record DeleteFaciltityCommand(Guid Id)
    : IRequest<ApiResult<bool>>;
public class DeleteFaciltityCommandHandler
    : IRequestHandler<DeleteFaciltityCommand, ApiResult<bool>>
{
    private readonly IOLMRSApplicationDbContext _db;

    public DeleteFaciltityCommandHandler(IOLMRSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        DeleteFaciltityCommand request,
        CancellationToken ct)
    {
        var entity = await _db.Set<Domain.Entities.CBMS.Facility>()
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity == null)
            return ApiResult<bool>.Fail("Facility not found.");

        entity.IsDeleted = true;
        entity.IsActive = false;

        await _db.SaveChangesAsync(ct);

        return ApiResult<bool>.Ok(true, "Facility deleted successfully.");
    }
}
