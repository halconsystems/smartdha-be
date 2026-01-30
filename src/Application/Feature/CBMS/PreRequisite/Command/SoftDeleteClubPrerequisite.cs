using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.PreRequisite.Command;


public record SoftDeleteClubProcessPrerequisiteCommand(
    Guid ProcessPrerequisiteId
) : IRequest<ApiResult<bool>>;
public class SoftDeleteClubProcessPrerequisiteCommandHandler
    : IRequestHandler<SoftDeleteClubProcessPrerequisiteCommand, ApiResult<bool>>
{
    private readonly IOLMRSApplicationDbContext _db;

    public SoftDeleteClubProcessPrerequisiteCommandHandler(IOLMRSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        SoftDeleteClubProcessPrerequisiteCommand r,
        CancellationToken ct)
    {
        var entity = await _db.Set<ClubProcessPrerequisite>()
            .FirstOrDefaultAsync(x =>
                x.Id == r.ProcessPrerequisiteId &&
                x.IsDeleted != true,
                ct);

        if (entity == null)
            return ApiResult<bool>.Fail("Process prerequisite not found.");

        // ✅ Manual soft delete (NO Remove / RemoveRange)
        entity.IsActive = false;
        entity.IsDeleted = true;

        await _db.SaveChangesAsync(ct);

        return ApiResult<bool>.Ok(true, "Process prerequisite deleted.");
    }
}

