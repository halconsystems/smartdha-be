using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubCategories.Command;

public record DeleteClubCategoryCommand(Guid Id)
    : IRequest<ApiResult<bool>>;
public class DeleteClubCategoryCommandHandler
    : IRequestHandler<DeleteClubCategoryCommand, ApiResult<bool>>
{
    private readonly IOLMRSApplicationDbContext _db;

    public DeleteClubCategoryCommandHandler(IOLMRSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        DeleteClubCategoryCommand request,
        CancellationToken ct)
    {
        var entity = await _db.Set<Domain.Entities.CBMS.ClubCategory>()
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity == null)
            return ApiResult<bool>.Fail("Club Category not found.");

        entity.IsDeleted = true;
        entity.IsActive = false;

        await _db.SaveChangesAsync(ct);

        return ApiResult<bool>.Ok(true, "Club Category deleted successfully.");
    }
}
