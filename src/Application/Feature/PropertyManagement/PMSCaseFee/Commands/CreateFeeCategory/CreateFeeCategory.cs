using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Commands.CreateFeeCategory;
public record CreateFeeCategoryCommand(
    string Code,
    string Name
) : IRequest<ApiResult<Guid>>;

public class CreateFeeCategoryHandler
    : IRequestHandler<CreateFeeCategoryCommand, ApiResult<Guid>>
{
    private readonly IPMSApplicationDbContext _db;

    public CreateFeeCategoryHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<Guid>> Handle(CreateFeeCategoryCommand r, CancellationToken ct)
    {
        var exists = await _db.Set<FeeCategory>()
            .AnyAsync(x => x.Code == r.Code, ct);

        if (exists)
            return ApiResult<Guid>.Fail("Fee category already exists.");

        var entity = new FeeCategory
        {
            Code = r.Code.Trim().ToUpperInvariant(),
            Name = r.Name.Trim()
        };

        _db.Set<FeeCategory>().Add(entity);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(entity.Id, "Fee category created.");
    }
}

