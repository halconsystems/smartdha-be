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

public record DeleteClubProcessPrerequisiteCommand
(
    Guid ProcessId,
    Guid PrerequisiteDefinitionId,
    bool DeleteDefinitionIfUnused
) : IRequest<ApiResult<bool>>;
public class DeleteClubProcessPrerequisiteHandler
    : IRequestHandler<DeleteClubProcessPrerequisiteCommand, ApiResult<bool>>
{
    private readonly IPMSApplicationDbContext _db;

    public DeleteClubProcessPrerequisiteHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        DeleteClubProcessPrerequisiteCommand r,
        CancellationToken ct)
    {
        // 1️⃣ Find mapping
        var mapping = await _db.Set<ClubProcessPrerequisite>()
            .FirstOrDefaultAsync(x =>
                x.ProcessId == r.ProcessId &&
                x.PrerequisiteDefinitionId == r.PrerequisiteDefinitionId,
                ct);

        if (mapping == null)
            return ApiResult<bool>.Fail("Prerequisite mapping not found.");

        _db.Set<ClubProcessPrerequisite>().Remove(mapping);

        // 2️⃣ Optionally delete definition if unused
        if (r.DeleteDefinitionIfUnused)
        {
            var stillUsed = await _db.Set<ClubProcessPrerequisite>()
                .AnyAsync(x =>
                    x.PrerequisiteDefinitionId == r.PrerequisiteDefinitionId &&
                    x.ProcessId != r.ProcessId,
                    ct);

            if (!stillUsed)
            {
                var definition = await _db.Set<ClubPrerequisiteDefinitions>()
                    .FirstOrDefaultAsync(x => x.Id == r.PrerequisiteDefinitionId, ct);

                if (definition != null)
                    _db.Set<ClubPrerequisiteDefinitions>().Remove(definition);
            }
        }

        await _db.SaveChangesAsync(ct);

        return ApiResult<bool>.Ok(true, "Prerequisite removed successfully.");
    }
}


