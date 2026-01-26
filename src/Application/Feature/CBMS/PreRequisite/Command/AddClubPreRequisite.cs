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

public record AddClubProcessPrerequisiteCommand(
    Guid ProcessId,
    Guid PrerequisiteDefinitionId,
    bool IsRequired,
    int RequiredByStepNo
) : IRequest<ApiResult<Guid>>;

public class AddClubProcessPrerequisiteCommandHandler : IRequestHandler<AddClubProcessPrerequisiteCommand, ApiResult<Guid>>
{
    private readonly IOLMRSApplicationDbContext _db;
    public AddClubProcessPrerequisiteCommandHandler(IOLMRSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<Guid>> Handle(AddClubProcessPrerequisiteCommand r, CancellationToken ct)
    {
        var processExists = await _db.Set<ClubServiceProcess>().AnyAsync(x => x.Id == r.ProcessId, ct);
        if (!processExists) return ApiResult<Guid>.Fail("Process not found.");

        var preExists = await _db.Set<ClubPrerequisiteDefinitions>().AnyAsync(x => x.Id == r.PrerequisiteDefinitionId, ct);
        if (!preExists) return ApiResult<Guid>.Fail("Prerequisite definition not found.");

        var dup = await _db.Set<ClubProcessPrerequisite>()
            .AnyAsync(x => x.ProcessId == r.ProcessId && x.PrerequisiteDefinitionId == r.PrerequisiteDefinitionId, ct);

        if (dup) return ApiResult<Guid>.Fail("Prerequisite already attached to this process.");

        var entity = new ClubProcessPrerequisite
        {
            ProcessId = r.ProcessId,
            PrerequisiteDefinitionId = r.PrerequisiteDefinitionId,
            IsRequired = r.IsRequired,
            RequiredByStepNo = r.RequiredByStepNo
        };

        _db.Set<ClubProcessPrerequisite>().Add(entity);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(entity.Id, "Process prerequisite added.");
    }
}


