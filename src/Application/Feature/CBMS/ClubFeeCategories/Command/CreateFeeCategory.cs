using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubFeeCategories.Command;

public record CreateClubFeeCategoryCommand(
    string Code,
    string Name
) : IRequest<ApiResult<Guid>>;

public class CreateClubFeeCategoryCommandHandler
    : IRequestHandler<CreateClubFeeCategoryCommand, ApiResult<Guid>>
{
    private readonly IOLMRSApplicationDbContext _db;

    public CreateClubFeeCategoryCommandHandler(IOLMRSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<Guid>> Handle(CreateClubFeeCategoryCommand r, CancellationToken ct)
    {
        var exists = await _db.Set<ClubFeeCategory>()
            .AnyAsync(x => x.Code == r.Code, ct);

        if (exists)
            return ApiResult<Guid>.Fail("Fee category already exists.");

        var entity = new ClubFeeCategory
        {
            Code = r.Code.Trim().ToUpperInvariant(),
            Name = r.Name.Trim()
        };

        _db.Set<ClubFeeCategory>().Add(entity);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(entity.Id, "Fee category created.");
    }
}


