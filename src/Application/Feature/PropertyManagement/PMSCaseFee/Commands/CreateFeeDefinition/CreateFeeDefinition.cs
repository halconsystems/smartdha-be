using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Commands.CreateFeeDefinition;
public record CreateFeeDefinitionCommand(
    Guid ProcessId,
    FeeType FeeType,
    decimal? FixedAmount,
    AreaUnit? AreaUnit,
    bool AllowOverride,
    DateTime? EffectiveFrom,
    DateTime? EffectiveTo,
    string? Notes
) : IRequest<ApiResult<Guid>>;
public class CreateFeeDefinitionHandler
    : IRequestHandler<CreateFeeDefinitionCommand, ApiResult<Guid>>
{
    private readonly IPMSApplicationDbContext _db;
    public CreateFeeDefinitionHandler(IPMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<Guid>> Handle(CreateFeeDefinitionCommand r, CancellationToken ct)
    {
        var exists = await _db.Set<FeeDefinition>()
            .AnyAsync(x => x.ProcessId == r.ProcessId, ct);

        if (exists)
            return ApiResult<Guid>.Fail("Fee definition already exists for this process.");

        var entity = new FeeDefinition
        {
            ProcessId = r.ProcessId,
            FeeType = r.FeeType,
            FixedAmount = r.FixedAmount,
            AreaUnit = r.AreaUnit,
            AllowOverride = r.AllowOverride,
            EffectiveFrom = r.EffectiveFrom,
            EffectiveTo = r.EffectiveTo,
            Notes = r.Notes
        };

        _db.Set<FeeDefinition>().Add(entity);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(entity.Id, "Fee definition created.");
    }
}

