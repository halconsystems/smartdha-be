using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSFeeSetting.Queries.GetFeeSettings;

public record FeeSettingDto(
    Guid Id,
    string Code,
    string DisplayName,
    decimal Amount
);
public record GetFeeSettingsQuery()
    : IRequest<ApiResult<List<FeeSettingDto>>>;

public class GetFeeSettingsHandler
    : IRequestHandler<GetFeeSettingsQuery, ApiResult<List<FeeSettingDto>>>
{
    private readonly IPMSApplicationDbContext _db;

    public GetFeeSettingsHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<List<FeeSettingDto>>> Handle(
        GetFeeSettingsQuery r,
        CancellationToken ct)
    {
        var list = await _db.Set<FeeSetting>()
            .Where(x => x.IsDeleted != true && x.IsActive == true)
            .OrderBy(x => x.DisplayName)
            .Select(x => new FeeSettingDto(
                x.Id,
                x.Code,
                x.DisplayName,
                x.Amount))
            .ToListAsync(ct);

        return ApiResult<List<FeeSettingDto>>.Ok(list);
    }
}


