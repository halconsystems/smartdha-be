using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands.DeleteProcessPrerequisite;
public record DeleteProcessPrerequisiteCommand
(
    Guid ProcessId,
    Guid PrerequisiteDefinitionId,
    bool DeleteDefinitionIfUnused
) : IRequest<ApiResult<bool>>;
public class DeleteProcessPrerequisiteHandler
    : IRequestHandler<DeleteProcessPrerequisiteCommand, ApiResult<bool>>
{
    private readonly IPMSApplicationDbContext _db;

    public DeleteProcessPrerequisiteHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        DeleteProcessPrerequisiteCommand r,
        CancellationToken ct)
    {
        // 1️⃣ Find mapping
        var mapping = await _db.Set<ProcessPrerequisite>()
            .FirstOrDefaultAsync(x =>
                x.ProcessId == r.ProcessId &&
                x.PrerequisiteDefinitionId == r.PrerequisiteDefinitionId,
                ct);

        if (mapping == null)
            return ApiResult<bool>.Fail("Prerequisite mapping not found.");

        _db.Set<ProcessPrerequisite>().Remove(mapping);

        // 2️⃣ Optionally delete definition if unused
        if (r.DeleteDefinitionIfUnused)
        {
            var stillUsed = await _db.Set<ProcessPrerequisite>()
                .AnyAsync(x =>
                    x.PrerequisiteDefinitionId == r.PrerequisiteDefinitionId &&
                    x.ProcessId != r.ProcessId,
                    ct);

            if (!stillUsed)
            {
                var definition = await _db.Set<PrerequisiteDefinition>()
                    .FirstOrDefaultAsync(x => x.Id == r.PrerequisiteDefinitionId, ct);

                if (definition != null)
                    _db.Set<PrerequisiteDefinition>().Remove(definition);
            }
        }

        await _db.SaveChangesAsync(ct);

        return ApiResult<bool>.Ok(true, "Prerequisite removed successfully.");
    }
}

