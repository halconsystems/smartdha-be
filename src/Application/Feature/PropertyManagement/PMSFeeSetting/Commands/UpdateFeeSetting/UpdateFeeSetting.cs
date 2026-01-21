using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSFeeSetting.Commands.UpdateFeeSetting;
public record UpdateFeeSettingCommand(
    Guid FeeSettingId,
    string DisplayName,
    decimal Amount
) : IRequest<ApiResult<bool>>;
public class UpdateFeeSettingHandler
    : IRequestHandler<UpdateFeeSettingCommand, ApiResult<bool>>
{
    private readonly IPMSApplicationDbContext _db;

    public UpdateFeeSettingHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        UpdateFeeSettingCommand r,
        CancellationToken ct)
    {
        var entity = await _db.Set<FeeSetting>()
            .FirstOrDefaultAsync(x =>
                x.Id == r.FeeSettingId &&
                x.IsDeleted != true, ct);

        if (entity == null)
            return ApiResult<bool>.Fail("Fee setting not found.");

        entity.DisplayName = r.DisplayName.Trim();
        entity.Amount = r.Amount;

        await _db.SaveChangesAsync(ct);
        return ApiResult<bool>.Ok(true, "Fee setting updated.");
    }
}

