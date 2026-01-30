using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSDirectorate.Queries.GetDirectorates;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubCategories.Queries;
public record ClubCategoriesDTO(Guid Id,string Name, string Code,string Description, string DisplayName, bool? IsActive, bool? IsDeleted);
public record GetClubCategoriesQuery() : IRequest<ApiResult<List<ClubCategoriesDTO>>>;

public class GetClubCategoriesQueryHandler : IRequestHandler<GetClubCategoriesQuery, ApiResult<List<ClubCategoriesDTO>>>
{
    private readonly IOLMRSApplicationDbContext _db;
    public GetClubCategoriesQueryHandler(IOLMRSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<List<ClubCategoriesDTO>>> Handle(GetClubCategoriesQuery request, CancellationToken ct)
    {
        var list = await _db.Set<Domain.Entities.CBMS.ClubCategory>()
             .Where(x => x.IsActive == true && x.IsDeleted == false)
            .OrderBy(x => x.Name)
            .Select(x => new ClubCategoriesDTO(x.Id,x.Name, x.Code,x.Description ?? "",x.DisplayName, x.IsActive,x.IsDeleted))
            .ToListAsync(ct);

        return ApiResult<List<ClubCategoriesDTO>>.Ok(list);
    }
}


