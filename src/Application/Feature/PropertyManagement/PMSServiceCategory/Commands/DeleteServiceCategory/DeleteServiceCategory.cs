using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSServiceCategory.Commands.DeleteServiceCategory;
public record DeleteServiceCategoryCommand(Guid Id)
    : IRequest<ApiResult<bool>>;
public class DeleteServiceCategoryHandler
    : IRequestHandler<DeleteServiceCategoryCommand, ApiResult<bool>>
{
    private readonly IPMSApplicationDbContext _db;

    public DeleteServiceCategoryHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        DeleteServiceCategoryCommand request,
        CancellationToken ct)
    {
        var entity = await _db.Set<ServiceCategory>()
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity == null)
            return ApiResult<bool>.Fail("Category not found.");

        entity.IsDeleted = true;
        entity.IsActive = false;

        await _db.SaveChangesAsync(ct);

        return ApiResult<bool>.Ok(true, "Category deleted successfully.");
    }
}
