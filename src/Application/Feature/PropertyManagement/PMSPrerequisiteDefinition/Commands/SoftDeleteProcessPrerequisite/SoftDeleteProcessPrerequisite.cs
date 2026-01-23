using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands.SoftDeleteProcessPrerequisite;
public record SoftDeleteProcessPrerequisiteCommand(
    Guid ProcessPrerequisiteId
) : IRequest<ApiResult<bool>>;
public class SoftDeleteProcessPrerequisiteHandler
    : IRequestHandler<SoftDeleteProcessPrerequisiteCommand, ApiResult<bool>>
{
    private readonly IPMSApplicationDbContext _db;

    public SoftDeleteProcessPrerequisiteHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        SoftDeleteProcessPrerequisiteCommand r,
        CancellationToken ct)
    {
        var entity = await _db.Set<ProcessPrerequisite>()
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

