using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Queries.GetFeeByProcess;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FeeDefination.Queries;

public record GetClubFeeDefinationByIdQuery(Guid FeeDefinitionId)
    : IRequest<ApiResult<ClubFeeSetupDTO>>;
public class GetClubFeeDefinationByIdQueryHandler
    : IRequestHandler<GetClubFeeDefinationByIdQuery, ApiResult<ClubFeeSetupDTO>>
{
    private readonly IPMSApplicationDbContext _db;
    public GetClubFeeDefinationByIdQueryHandler(IPMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<ClubFeeSetupDTO>> Handle(GetClubFeeDefinationByIdQuery r, CancellationToken ct)
    {
        var fee = await _db.Set<ClubFeeDefinition>()
            .FirstOrDefaultAsync(x => x.Id == r.FeeDefinitionId, ct);

        if (fee == null)
            return ApiResult<ClubFeeSetupDTO>.Fail("Fee definition not found.");

        var slabs = await _db.Set<ClubFeeSlab>()
            .Where(x => x.FeeDefinitionId == fee.Id)
            .OrderBy(x => x.FromArea)
            .Select(x => new ClubFeeSlabDto(x.FromArea, x.ToArea, x.Amount))
            .ToListAsync(ct);

        return ApiResult<ClubFeeSetupDTO>.Ok(new ClubFeeSetupDTO(
            fee.Id,
            fee.FeeType,
            fee.FixedAmount,
            fee.AreaUnit,
            fee.AllowOverride,
            slabs
        ));
    }
}


