using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.DropDonws.Queries;
public record GetFacilitiesDropdownByCategoryQuery(Guid CategoryId)
    : IRequest<List<DropdownDto>>;
public class GetFacilitiesDropdownByCategoryHandler
    : IRequestHandler<GetFacilitiesDropdownByCategoryQuery, List<DropdownDto>>
{
    private readonly ICBMSApplicationDbContext _db;

    public GetFacilitiesDropdownByCategoryHandler(ICBMSApplicationDbContext db)
        => _db = db;

    public async Task<List<DropdownDto>> Handle(
        GetFacilitiesDropdownByCategoryQuery request,
        CancellationToken ct)
    {
        return await _db.Facilities
            .AsNoTracking()
            .Where(x =>
                x.ClubCategoryId == request.CategoryId &&
                x.IsDeleted != true &&
                x.IsActive == true)
            .OrderBy(x => x.DisplayName)
            .Select(x => new DropdownDto(
                x.Id,
                x.DisplayName
            ))
            .ToListAsync(ct);
    }
}

