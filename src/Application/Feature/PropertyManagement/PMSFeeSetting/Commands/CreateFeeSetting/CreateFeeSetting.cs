using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSFeeSetting.Commands.CreateFeeSetting;
public record CreateFeeSettingCommand(
    string Code,
    string DisplayName,
    decimal Amount
) : IRequest<ApiResult<Guid>>;
public class CreateFeeSettingHandler
    : IRequestHandler<CreateFeeSettingCommand, ApiResult<Guid>>
{
    private readonly IPMSApplicationDbContext _db;

    public CreateFeeSettingHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<Guid>> Handle(
        CreateFeeSettingCommand r,
        CancellationToken ct)
    {
        var code = r.Code.Trim().ToUpperInvariant();

        var exists = await _db.Set<FeeSetting>()
            .AnyAsync(x => x.Code == code && x.IsDeleted != true, ct);

        if (exists)
            return ApiResult<Guid>.Fail("Fee setting already exists.");

        var entity = new FeeSetting
        {
            Code = code,
            DisplayName = r.DisplayName.Trim(),
            Amount = r.Amount,
            IsActive = true,
            IsDeleted = false
        };

        _db.Set<FeeSetting>().Add(entity);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(entity.Id, "Fee setting created.");
    }
}

