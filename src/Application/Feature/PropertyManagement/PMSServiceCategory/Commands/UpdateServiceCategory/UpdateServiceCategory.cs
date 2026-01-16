using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSServiceCategory.Commands.UpdateServiceCategory;
public record UpdateServiceCategoryCommand(
    Guid Id,
    string Name,
    string Code
) : IRequest<ApiResult<bool>>;
public class UpdateServiceCategoryHandler
    : IRequestHandler<UpdateServiceCategoryCommand, ApiResult<bool>>
{
    private readonly IPMSApplicationDbContext _db;

    public UpdateServiceCategoryHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        UpdateServiceCategoryCommand request,
        CancellationToken ct)
    {
        var entity = await _db.Set<ServiceCategory>()
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity == null)
            return ApiResult<bool>.Fail("Category not found.");

        var codeExists = await _db.Set<ServiceCategory>()
            .AnyAsync(x => x.Code == request.Code && x.Id != request.Id, ct);

        if (codeExists)
            return ApiResult<bool>.Fail("Category code already exists.");

        entity.Name = request.Name.Trim();
        entity.Code = request.Code.Trim().ToUpperInvariant();

        await _db.SaveChangesAsync(ct);

        return ApiResult<bool>.Ok(true, "Category updated successfully.");
    }
}
