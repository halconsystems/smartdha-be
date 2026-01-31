using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubCategories.Queries.GetClubCategories;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.Clubs.Queries.GetClubCategories;
public record GetClubCategoriesQuery(Guid ClubId)
    : IRequest<ApiResult<List<CategoryDTO>>>;
public class GetClubCategoriesQueryHandler
    : IRequestHandler<GetClubCategoriesQuery, ApiResult<List<CategoryDTO>>>
{
    private readonly IOLMRSApplicationDbContext _ctx;

    public GetClubCategoriesQueryHandler(IOLMRSApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<ApiResult<List<CategoryDTO>>> Handle(
        GetClubCategoriesQuery request,
        CancellationToken ct)
    {
        var categories = await _ctx.ClubFacilities
            .AsNoTracking()
            .Where(x =>
                x.ClubId == request.ClubId &&
                x.IsAvailable
            )
            .Select(x => new CategoryDTO
            {
                Id = x.Facility.ClubCategory.Id,
                Name = x.Facility.ClubCategory.DisplayName,
                Code=x.Facility.ClubCategory.Code,
                Description=x.Facility.ClubCategory.Description ?? "",
                DisplayName=x.Facility.ClubCategory.DisplayName ?? "",
            })
            .Distinct()
            .ToListAsync(ct);

        if (!categories.Any())
            return ApiResult<List<CategoryDTO>>.Fail("No categories found for this club.");

        return ApiResult<List<CategoryDTO>>.Ok(categories);
    }
}

