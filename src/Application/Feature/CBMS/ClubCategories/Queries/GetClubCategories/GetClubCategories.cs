using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubCategories.Queries.GetClubCategories;
public record GetClubCategoriesQuery(Guid ClubId)
    : IRequest<List<CategoryDTO>>;
public class GetClubCategoriesQueryHandler
    : IRequestHandler<GetClubCategoriesQuery, List<CategoryDTO>>
{
    private readonly ICBMSApplicationDbContext _ctx;

    public GetClubCategoriesQueryHandler(ICBMSApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<List<CategoryDTO>> Handle(
        GetClubCategoriesQuery request,
        CancellationToken ct)
    {
        return await _ctx.ClubFacilities
            .AsNoTracking()
            .Where(x => x.ClubId == request.ClubId && x.IsAvailable)
            .Select(x => new CategoryDTO
            {
                Id = x.Facility.ClubCategory.Id,
                Name = x.Facility.ClubCategory.DisplayName,
                Description= x.Facility.ClubCategory.Description ?? "",
                DisplayName= x.Facility.ClubCategory.DisplayName,
                Code= x.Facility.ClubCategory.Code
            })
            .Distinct()
            .ToListAsync(ct);
    }
}

