using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSDirectorate.Commands.CreateDirectorate;
public record CreateDirectorateCommand(string Name, string Code,Guid ModuleId,Guid RoleId) : IRequest<ApiResult<Guid>>;

public class CreateDirectorateHandler : IRequestHandler<CreateDirectorateCommand, ApiResult<Guid>>
{
    private readonly IPMSApplicationDbContext _db;
    public CreateDirectorateHandler(IPMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<Guid>> Handle(CreateDirectorateCommand request, CancellationToken ct)
    {
        var exists = await _db.Set<Directorate>()
            .AnyAsync(x => x.Code == request.Code, ct);

        if (exists) return ApiResult<Guid>.Fail("Directorate code already exists.");

        var entity = new Directorate
        {
            Name = request.Name.Trim(),
            Code = request.Code.Trim().ToUpperInvariant(),
            ModuleId = request.ModuleId,
            RoleId = request.RoleId
        };

        _db.Set<Directorate>().Add(entity);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(entity.Id, "Directorate created.");
    }
}
