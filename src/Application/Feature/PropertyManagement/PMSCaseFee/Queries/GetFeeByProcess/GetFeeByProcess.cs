using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Queries.GetFeeByProcess;

public record FeeSlabDto(
    decimal FromArea,
    decimal ToArea,
    decimal Amount
);

public record FeeSetupDto(
    Guid FeeDefinitionId,
    FeeType FeeType,
    decimal? FixedAmount,
    AreaUnit? AreaUnit,
    bool AllowOverride,
    List<FeeSlabDto> Slabs
);


public record GetFeeByProcessQuery(Guid ProcessId)
    : IRequest<ApiResult<FeeSetupDto>>;

public class GetFeeByProcessHandler
    : IRequestHandler<GetFeeByProcessQuery, ApiResult<FeeSetupDto>>
{
    private readonly IPMSApplicationDbContext _db;

    public GetFeeByProcessHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<FeeSetupDto>> Handle(
        GetFeeByProcessQuery request,
        CancellationToken ct)
    {
        var fee = await _db.Set<FeeDefinition>()
            .Include(x => x.Process)
            .FirstOrDefaultAsync(
                x => x.ProcessId == request.ProcessId &&
                     (x.EffectiveFrom == null || x.EffectiveFrom <= DateTime.UtcNow) &&
                     (x.EffectiveTo == null || x.EffectiveTo >= DateTime.UtcNow),
                ct);

        if (fee == null)
            return ApiResult<FeeSetupDto>.Fail("No fee configuration found.");

        var slabs = await _db.Set<FeeSlab>()
            .Where(x => x.FeeDefinitionId == fee.Id)
            .OrderBy(x => x.FromArea)
            .Select(x => new FeeSlabDto(
                x.FromArea,
                x.ToArea,
                x.Amount
            ))
            .ToListAsync(ct);

        var dto = new FeeSetupDto(
            fee.Id,
            fee.FeeType,
            fee.FixedAmount,
            fee.AreaUnit,
            fee.AllowOverride,
            slabs
        );

        return ApiResult<FeeSetupDto>.Ok(dto);
    }
}


