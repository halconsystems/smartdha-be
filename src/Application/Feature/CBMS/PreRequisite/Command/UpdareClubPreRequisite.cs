using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.PreRequisite.Command;

public record UpdateClubProcessPrerequisiteCommand
(
    Guid ProcessId,
    Guid PrerequisiteDefinitionId,

    // Definition updates
    string Name,
    PrerequisiteType Type,
    int? MinLength,
    int? MaxLength,
    string? AllowedExtensions,

    // Mapping updates
    bool IsRequired,
    int RequiredByStepNo
) : IRequest<ApiResult<bool>>;
public class UpdateClubProcessPrerequisiteHandler
    : IRequestHandler<UpdateClubProcessPrerequisiteCommand, ApiResult<bool>>
{
    private readonly IOLMRSApplicationDbContext _db;

    public UpdateClubProcessPrerequisiteHandler(IOLMRSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        UpdateClubProcessPrerequisiteCommand r,
        CancellationToken ct)
    {
        // 1️⃣ Load definition
        var definition = await _db.Set<ClubPrerequisiteDefinitions>()
            .FirstOrDefaultAsync(x => x.Id == r.PrerequisiteDefinitionId, ct);

        if (definition == null)
            return ApiResult<bool>.Fail("Prerequisite definition not found.");

        // 2️⃣ Load mapping
        var mapping = await _db.Set<ClubProcessPrerequisite>()
            .FirstOrDefaultAsync(x =>
                x.ProcessId == r.ProcessId &&
                x.PrerequisiteDefinitionId == r.PrerequisiteDefinitionId,
                ct);

        if (mapping == null)
            return ApiResult<bool>.Fail("Prerequisite is not attached to this process.");

        // 3️⃣ Update definition (shared data)
        definition.Name = r.Name.Trim();
        definition.Type = r.Type;
        definition.MinLength = r.MinLength;
        definition.MaxLength = r.MaxLength;
        definition.AllowedExtensions = r.AllowedExtensions;

        // 4️⃣ Update mapping (process-specific)
        mapping.IsRequired = r.IsRequired;
        mapping.RequiredByStepNo = r.RequiredByStepNo;

        await _db.SaveChangesAsync(ct);

        return ApiResult<bool>.Ok(true, "Prerequisite updated successfully.");
    }
}


