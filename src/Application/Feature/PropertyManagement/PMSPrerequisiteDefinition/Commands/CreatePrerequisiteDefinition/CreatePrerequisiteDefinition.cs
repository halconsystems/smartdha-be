using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands.CreatePrerequisiteDefinition;
public record CreatePrerequisiteDefinitionCommand(
    string Name,
    string Code,
    PrerequisiteType Type,
    int? MinLength,
    int? MaxLength,
    string? AllowedExtensions
) : IRequest<ApiResult<Guid>>;

public class CreatePrerequisiteDefinitionHandler : IRequestHandler<CreatePrerequisiteDefinitionCommand, ApiResult<Guid>>
{
    private readonly IPMSApplicationDbContext _db;
    public CreatePrerequisiteDefinitionHandler(IPMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<Guid>> Handle(CreatePrerequisiteDefinitionCommand r, CancellationToken ct)
    {
        var exists = await _db.Set<PrerequisiteDefinition>().AnyAsync(x => x.Code == r.Code, ct);
        if (exists) return ApiResult<Guid>.Fail("Prerequisite code already exists.");

        var def = new PrerequisiteDefinition
        {
            Name = r.Name.Trim(),
            Code = r.Code.Trim().ToUpperInvariant(),
            Type = r.Type,
            MinLength = r.MinLength,
            MaxLength = r.MaxLength,
            AllowedExtensions = r.AllowedExtensions
        };

        _db.Set<PrerequisiteDefinition>().Add(def);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(def.Id, "Prerequisite created.");
    }
}

