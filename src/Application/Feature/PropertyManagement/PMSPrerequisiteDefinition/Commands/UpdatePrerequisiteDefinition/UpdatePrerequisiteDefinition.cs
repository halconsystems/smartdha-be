using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands.UpdatePrerequisiteDefinition;
public record UpdatePrerequisiteDefinitionCommand(
    Guid Id,
    string Name,
    string Code,
    PrerequisiteType Type,
    int? MinLength,
    int? MaxLength,
    string? AllowedExtensions
) : IRequest<ApiResult<bool>>;
public class UpdatePrerequisiteDefinitionHandler
    : IRequestHandler<UpdatePrerequisiteDefinitionCommand, ApiResult<bool>>
{
    private readonly IPMSApplicationDbContext _db;
    public UpdatePrerequisiteDefinitionHandler(IPMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<bool>> Handle(UpdatePrerequisiteDefinitionCommand r, CancellationToken ct)
    {
        var entity = await _db.Set<PrerequisiteDefinition>()
            .FirstOrDefaultAsync(x => x.Id == r.Id, ct);

        if (entity == null)
            return ApiResult<bool>.Fail("Prerequisite not found.");

        // Code uniqueness check (excluding current)
        var codeExists = await _db.Set<PrerequisiteDefinition>()
            .AnyAsync(x => x.Code == r.Code && x.Id != r.Id, ct);

        if (codeExists)
            return ApiResult<bool>.Fail("Prerequisite code already exists.");

        entity.Name = r.Name.Trim();
        entity.Code = r.Code.Trim().ToUpperInvariant();
        entity.Type = r.Type;
        entity.MinLength = r.MinLength;
        entity.MaxLength = r.MaxLength;
        entity.AllowedExtensions = r.AllowedExtensions;

        await _db.SaveChangesAsync(ct);
        return ApiResult<bool>.Ok(true, "Prerequisite updated.");
    }
}
