using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSDirectorate.Commands.DeleteDirectorate;
public record DeleteDirectorateCommand(Guid Id)
    : IRequest<ApiResult<bool>>;

public class DeleteDirectorateHandler
    : IRequestHandler<DeleteDirectorateCommand, ApiResult<bool>>
{
    private readonly IPMSApplicationDbContext _db;
    public DeleteDirectorateHandler(IPMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<bool>> Handle(
        DeleteDirectorateCommand request,
        CancellationToken ct)
    {
        var entity = await _db.Set<Directorate>()
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity == null)
            return ApiResult<bool>.Fail("Directorate not found.");

        // Optional safety check: prevent delete if used in workflow
        var inUse = await _db.Set<ProcessStep>()
            .AnyAsync(x => x.DirectorateId == request.Id, ct);

        if (inUse)
            return ApiResult<bool>.Fail(
                "Directorate is in use in workflow steps and cannot be deleted.");

        entity.IsDeleted = true;
        entity.IsActive = false;

        await _db.SaveChangesAsync(ct);
        return ApiResult<bool>.Ok(true, "Directorate deleted.");
    }
}

