using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands.AddProcessPrerequisite;
public record AddProcessPrerequisiteCommand(
    Guid ProcessId,
    Guid PrerequisiteDefinitionId,
    bool IsRequired,
    int RequiredByStepNo
) : IRequest<ApiResult<Guid>>;

public class AddProcessPrerequisiteHandler : IRequestHandler<AddProcessPrerequisiteCommand, ApiResult<Guid>>
{
    private readonly IPMSApplicationDbContext _db;
    private readonly ICurrentUserService _currentUserService;
    public AddProcessPrerequisiteHandler(IPMSApplicationDbContext db, ICurrentUserService currentUserService)
    {
        _db = db;
        _currentUserService = currentUserService;
    }
    public async Task<ApiResult<Guid>> Handle(AddProcessPrerequisiteCommand r, CancellationToken ct)
    {
        var processExists = await _db.Set<ServiceProcess>().AnyAsync(x => x.Id == r.ProcessId, ct);
        if (!processExists) return ApiResult<Guid>.Fail("Process not found.");

        var preExists = await _db.Set<PrerequisiteDefinition>().AnyAsync(x => x.Id == r.PrerequisiteDefinitionId, ct);
        if (!preExists) return ApiResult<Guid>.Fail("Prerequisite definition not found.");

        var dup = await _db.Set<ProcessPrerequisite>()
            .AnyAsync(x => x.ProcessId == r.ProcessId && x.PrerequisiteDefinitionId == r.PrerequisiteDefinitionId, ct);
        if (dup)
        {
            var updateProcess = await _db.Set<ProcessPrerequisite>()
             .FirstOrDefaultAsync(x => x.ProcessId == r.ProcessId && x.PrerequisiteDefinitionId == r.PrerequisiteDefinitionId, ct);
            if (updateProcess == null)
            {
                return ApiResult<Guid>.Fail("Prerequisite definition not found.");
            }

            updateProcess.IsDeleted = false;
            updateProcess.IsActive = true;
            updateProcess.LastModified = DateTime.Now;
            updateProcess.LastModifiedBy = _currentUserService.UserId.ToString();

            _db.Set<ProcessPrerequisite>().Add(updateProcess);
            await _db.SaveChangesAsync(ct);

            return ApiResult<Guid>.Ok(updateProcess.Id, "Process prerequisite added.");

        }
        else
        {
            var entity = new ProcessPrerequisite
            {
                ProcessId = r.ProcessId,
                PrerequisiteDefinitionId = r.PrerequisiteDefinitionId,
                IsRequired = r.IsRequired,
                RequiredByStepNo = r.RequiredByStepNo
            };

            _db.Set<ProcessPrerequisite>().Add(entity);
            await _db.SaveChangesAsync(ct);

            return ApiResult<Guid>.Ok(entity.Id, "Process prerequisite added.");
        }
    }
}

