using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Commands.DeleteServiceProcess;
public record DeleteServiceProcessCommand(Guid Id)
    : IRequest<ApiResult<bool>>;

public class DeleteServiceProcessHandler
    : IRequestHandler<DeleteServiceProcessCommand, ApiResult<bool>>
{
    private readonly IPMSApplicationDbContext _db;

    public DeleteServiceProcessHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(DeleteServiceProcessCommand r, CancellationToken ct)
    {
        var entity = await _db.Set<ServiceProcess>()
            .FirstOrDefaultAsync(x => x.Id == r.Id, ct);

        if (entity == null)
            return ApiResult<bool>.Fail("Process not found.");

        // Optional safety check:
        // Prevent deletion if cases already exist
        var hasCases = await _db.Set<PropertyCase>()
            .AnyAsync(x => x.ProcessId == r.Id, ct);

        if (hasCases)
            return ApiResult<bool>.Fail("Process cannot be deleted because cases already exist.");

        entity.IsDeleted = true;
        entity.IsActive = false;

        await _db.SaveChangesAsync(ct);

        return ApiResult<bool>.Ok(true, "Process deleted (soft delete).");
    }
}

