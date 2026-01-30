using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.Clubs.Queries.GetClubFacilitiesByCategory;
public record GetClubFacilitiesByCategoryQuery(
    Guid ClubId,
    Guid CategoryId
) : IRequest<List<FacilityDTO>>;
public class GetClubFacilitiesByCategoryQueryHandler
    : IRequestHandler<GetClubFacilitiesByCategoryQuery, List<FacilityDTO>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    public GetClubFacilitiesByCategoryQueryHandler(IOLMRSApplicationDbContext ctx)
    {
        _ctx = ctx;
    }
    public async Task<List<FacilityDTO>> Handle(
        GetClubFacilitiesByCategoryQuery request,
        CancellationToken ct)
    {
        return await _ctx.ClubFacilities
            .AsNoTracking()
            .Where(x =>
                x.ClubId == request.ClubId &&
                x.IsAvailable &&
                x.Facility.ClubCategoryId == request.CategoryId
            )
            .Select(x => new FacilityDTO
            {
                Id = x.Facility.Id,
                Name = x.Facility.DisplayName,
                Price = x.Price,
                IsAvailable = x.IsAvailable,
                HasAction = x.HasAction,
                ActionName = x.ActionName,
                ImageUrl = x.Facility.ImageURL,
            })
            .ToListAsync(ct);
    }
}

