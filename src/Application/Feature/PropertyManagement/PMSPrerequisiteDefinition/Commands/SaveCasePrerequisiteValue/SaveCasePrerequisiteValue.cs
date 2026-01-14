using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands.SaveCasePrerequisiteValue;
public record SaveCasePrerequisiteValueCommand(
    Guid CaseId,
    Guid PrerequisiteDefinitionId,
    string? ValueText,
    decimal? ValueNumber,
    DateTime? ValueDate,
    bool? ValueBool
) : IRequest<ApiResult<bool>>;

public class SaveCasePrerequisiteValueHandler : IRequestHandler<SaveCasePrerequisiteValueCommand, ApiResult<bool>>
{
    private readonly IPMSApplicationDbContext _db;
    public SaveCasePrerequisiteValueHandler(IPMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<bool>> Handle(SaveCasePrerequisiteValueCommand r, CancellationToken ct)
    {
        var exists = await _db.Set<PropertyCase>().AnyAsync(x => x.Id == r.CaseId, ct);
        if (!exists) return ApiResult<bool>.Fail("Case not found.");

        var defExists = await _db.Set<PrerequisiteDefinition>().AnyAsync(x => x.Id == r.PrerequisiteDefinitionId, ct);
        if (!defExists) return ApiResult<bool>.Fail("Prerequisite definition not found.");

        var row = await _db.Set<CasePrerequisiteValue>()
            .FirstOrDefaultAsync(x => x.CaseId == r.CaseId && x.PrerequisiteDefinitionId == r.PrerequisiteDefinitionId, ct);

        if (row == null)
        {
            row = new CasePrerequisiteValue
            {
                CaseId = r.CaseId,
                PrerequisiteDefinitionId = r.PrerequisiteDefinitionId
            };
            _db.Set<CasePrerequisiteValue>().Add(row);
        }

        row.ValueText = r.ValueText;
        row.ValueNumber = r.ValueNumber;
        row.ValueDate = r.ValueDate;
        row.ValueBool = r.ValueBool;

        await _db.SaveChangesAsync(ct);
        return ApiResult<bool>.Ok(true, "Prerequisite saved.");
    }
}

