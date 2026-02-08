using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Queries.ClubFacilities;

public record ClubFacilitiesDTO(
    Guid Id,
    Guid ClubId,
    Guid FacillityId,
    string FacilityName,
    string CategoryName,
    decimal? Price,
    bool IsAvailable,
    bool IsPriceVisible,
    bool HasAction,
    string? ActionName,
    string? ActionTypes,
    FacilityActionType FacilityActionType,
    BookingMode BookingMode
);

public record GetAllClubFaciltiesQuery() : IRequest<ApiResult<List<ClubFacilitiesDTO>>>;

public class GetAllClubFaciltiesQueryHandler
    : IRequestHandler<GetAllClubFaciltiesQuery, ApiResult<List<ClubFacilitiesDTO>>>
{
    private readonly ICBMSApplicationDbContext _db;

    public GetAllClubFaciltiesQueryHandler(ICBMSApplicationDbContext db)
        => _db = db;

    public async Task<ApiResult<List<ClubFacilitiesDTO>>> Handle(
        GetAllClubFaciltiesQuery request,
        CancellationToken ct)
    {
        var list = await _db.Set<Domain.Entities.CBMS.ClubFacility>()
            .AsNoTracking()
            .Where(x => x.IsDeleted != true)
            .OrderBy(x => x.Price)
            .Select(x => new ClubFacilitiesDTO(
                x.Id,
                x.ClubId,
                x.FacilityId,
                x.Facility.Name,                 // ✅ Facility Name
                x.Facility.ClubCategory.Name,    // ✅ Category Name
                x.Price,
                x.IsAvailable,
                x.IsPriceVisible,
                x.HasAction,
                x.ActionName,
                x.ActionType,
                x.FacilityActionType,
                x.BookingMode
            ))
            .ToListAsync(ct);

        return ApiResult<List<ClubFacilitiesDTO>>.Ok(list);
    }
}



