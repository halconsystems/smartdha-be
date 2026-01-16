using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands.DeletePrerequisiteDefinition;
public record DeletePrerequisiteDefinitionCommand(Guid Id)
    : IRequest<ApiResult<bool>>;
public class DeletePrerequisiteDefinitionHandler
    : IRequestHandler<DeletePrerequisiteDefinitionCommand, ApiResult<bool>>
{
    private readonly IPMSApplicationDbContext _db;
    public DeletePrerequisiteDefinitionHandler(IPMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<bool>> Handle(
    DeletePrerequisiteDefinitionCommand r,
    CancellationToken ct)
    {
        var entity = await _db.Set<PrerequisiteDefinition>()
            .FirstOrDefaultAsync(x => x.Id == r.Id, ct);

        if (entity == null)
            return ApiResult<bool>.Fail("Prerequisite not found.");

        // Prevent delete if already attached to a process
        var isUsed = await _db.Set<ProcessPrerequisite>()
            .AnyAsync(x => x.PrerequisiteDefinitionId == r.Id, ct);

        if (isUsed)
            return ApiResult<bool>.Fail(
                "Prerequisite is already attached to a process and cannot be deleted.");

        // ✅ SOFT DELETE (EXPLICIT & CLEAR)
        entity.IsActive = false;
        entity.IsDeleted = true;

        await _db.SaveChangesAsync(ct);

        return ApiResult<bool>.Ok(true, "Prerequisite deleted successfully.");
    }

}
