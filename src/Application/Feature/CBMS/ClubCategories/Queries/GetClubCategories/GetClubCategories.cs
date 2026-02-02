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
        return await _ctx.ClubCategories
            .AsNoTracking()
            .Where(x => x.Id == request.ClubId)
            .Select(x => new CategoryDTO
            {
                Id = x.Id,
                Name = x.Name,
                Description= x.Description ?? "",
                DisplayName= x.DisplayName,
                Code= x.Code
            })
            .Distinct()
            .ToListAsync(ct);
    }
}

