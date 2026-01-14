using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSServiceCategory.Commands.CreateServiceCategory;
public record CreateServiceCategoryCommand(string Name, string Code) : IRequest<ApiResult<Guid>>;

public class CreateServiceCategoryHandler : IRequestHandler<CreateServiceCategoryCommand, ApiResult<Guid>>
{
    private readonly IPMSApplicationDbContext _db;
    public CreateServiceCategoryHandler(IPMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<Guid>> Handle(CreateServiceCategoryCommand request, CancellationToken ct)
    {
        var exists = await _db.Set<ServiceCategory>()
            .AnyAsync(x => x.Code == request.Code, ct);

        if (exists) return ApiResult<Guid>.Fail("Category code already exists.");

        var entity = new ServiceCategory
        {
            Name = request.Name.Trim(),
            Code = request.Code.Trim().ToUpperInvariant()
        };

        _db.Set<ServiceCategory>().Add(entity);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(entity.Id, "Category created.");
    }
}
