using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSDirectorate.Queries.GetDirectorates;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSServiceCategory.Queries.GetServiceCategories;
public record GetServiceCategoriesQuery() : IRequest<ApiResult<List<IdNameDto>>>;

public class GetServiceCategoriesHandler : IRequestHandler<GetServiceCategoriesQuery, ApiResult<List<IdNameDto>>>
{
    private readonly IPMSApplicationDbContext _db;
    public GetServiceCategoriesHandler(IPMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<List<IdNameDto>>> Handle(GetServiceCategoriesQuery request, CancellationToken ct)
    {
        var list = await _db.Set<ServiceCategory>()
             .Where(x=>x.IsActive==true &&x.IsDeleted==false)
            .OrderBy(x => x.Name)
            .Select(x => new IdNameDto(x.Id, x.Name, x.Code,""))
            .ToListAsync(ct);

        return ApiResult<List<IdNameDto>>.Ok(list);
    }
}

