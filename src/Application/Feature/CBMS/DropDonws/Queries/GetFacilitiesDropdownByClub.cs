using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.DropDonws.Queries;
public record GetFacilitiesDropdownByClubQuery(Guid ClubId)
    : IRequest<List<DropdownDto>>;
public class GetFacilitiesDropdownByClubHandler
    : IRequestHandler<GetFacilitiesDropdownByClubQuery, List<DropdownDto>>
{
    private readonly ICBMSApplicationDbContext _db;

    public GetFacilitiesDropdownByClubHandler(ICBMSApplicationDbContext db)
        => _db = db;

    public async Task<List<DropdownDto>> Handle(
        GetFacilitiesDropdownByClubQuery request,
        CancellationToken ct)
    {
        return await _db.ClubFacilities
            .AsNoTracking()
            .Include(x => x.Facility)
            .Where(x =>
                x.ClubId == request.ClubId &&
                x.IsDeleted != true &&
                x.Facility.IsActive == true)
            .OrderBy(x => x.Facility.DisplayName)
            .Select(x => new DropdownDto(
                x.Facility.Id,
                x.Facility.DisplayName))
            .Distinct()
            .ToListAsync(ct);
    }
}

