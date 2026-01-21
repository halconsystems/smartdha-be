using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSFeeSetting.Commands.DeleteFeeSetting;
public record DeleteFeeSettingCommand(Guid FeeSettingId)
    : IRequest<ApiResult<bool>>;

public class DeleteFeeSettingHandler
    : IRequestHandler<DeleteFeeSettingCommand, ApiResult<bool>>
{
    private readonly IPMSApplicationDbContext _db;

    public DeleteFeeSettingHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }
    public async Task<ApiResult<bool>> Handle(
        DeleteFeeSettingCommand r,
        CancellationToken ct)
    {
        var entity = await _db.Set<FeeSetting>()
            .FirstOrDefaultAsync(x =>
                x.Id == r.FeeSettingId &&
                x.IsDeleted != true, ct);

        if (entity == null)
            return ApiResult<bool>.Fail("Fee setting not found.");

        entity.IsActive = false;
        entity.IsDeleted = true;

        await _db.SaveChangesAsync(ct);
        return ApiResult<bool>.Ok(true, "Fee setting deleted.");
    }
}

