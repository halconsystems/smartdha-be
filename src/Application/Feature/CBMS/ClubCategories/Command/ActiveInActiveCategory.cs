using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubCategories.Command;

public record ActiveDeleteCategoryCommand(Guid Id,bool Active)
    : IRequest<ApiResult<bool>>;
public class ActiveDeleteCategoryCommandHandler
    : IRequestHandler<ActiveDeleteCategoryCommand, ApiResult<bool>>
{
    private readonly IOLMRSApplicationDbContext _db;

    public ActiveDeleteCategoryCommandHandler(IOLMRSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        ActiveDeleteCategoryCommand request,
        CancellationToken ct)
    {
        var entity = await _db.Set<Domain.Entities.CBMS.ClubServiceCategory>()
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity == null)
            return ApiResult<bool>.Fail("Club Category not found.");

        entity.IsActive = request.Active;


        await _db.SaveChangesAsync(ct);

        return ApiResult<bool>.Ok(true, "Club Category deleted successfully.");
    }
}
