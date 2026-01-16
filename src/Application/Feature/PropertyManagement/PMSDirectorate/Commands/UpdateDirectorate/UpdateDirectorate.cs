using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSDirectorate.Commands.UpdateDirectorate;
public record UpdateDirectorateCommand(
    Guid Id,
    string Name,
    string Code
) : IRequest<ApiResult<bool>>;
public class UpdateDirectorateHandler
    : IRequestHandler<UpdateDirectorateCommand, ApiResult<bool>>
{
    private readonly IPMSApplicationDbContext _db;
    public UpdateDirectorateHandler(IPMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<bool>> Handle(
        UpdateDirectorateCommand request,
        CancellationToken ct)
    {
        var entity = await _db.Set<Directorate>()
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity == null)
            return ApiResult<bool>.Fail("Directorate not found.");

        var codeExists = await _db.Set<Directorate>()
            .AnyAsync(x => x.Code == request.Code && x.Id != request.Id, ct);

        if (codeExists)
            return ApiResult<bool>.Fail("Directorate code already exists.");

        entity.Name = request.Name.Trim();
        entity.Code = request.Code.Trim().ToUpperInvariant();

        await _db.SaveChangesAsync(ct);
        return ApiResult<bool>.Ok(true, "Directorate updated.");
    }
}

