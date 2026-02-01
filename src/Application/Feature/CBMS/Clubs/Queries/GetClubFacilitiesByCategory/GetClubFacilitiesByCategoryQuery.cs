using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.Clubs.Queries.GetClubFacilitiesByCategory;
public record GetClubFacilitiesByCategoryQuery(
    Guid ClubId,
    Guid CategoryId
) : IRequest<List<FacilityDTO>>;
public class GetClubFacilitiesByCategoryHandler
    : IRequestHandler<GetClubFacilitiesByCategoryQuery, List<FacilityDTO>>
{
    private readonly ICBMSApplicationDbContext _ctx;
    private readonly IFileStorageService _fileStorageService;

    public GetClubFacilitiesByCategoryHandler(
        ICBMSApplicationDbContext ctx,
        IFileStorageService fileStorageService)
    {
        _ctx = ctx;
        _fileStorageService = fileStorageService;
    }

    public async Task<List<FacilityDTO>> Handle(
        GetClubFacilitiesByCategoryQuery request,
        CancellationToken ct)
    {
        var raw = await _ctx.ClubFacilities
            .AsNoTracking()
            .Where(x =>
                x.ClubId == request.ClubId &&
                x.IsAvailable &&
                x.Facility.ClubCategoryId == request.CategoryId)
            .Select(x => new
            {
                FacilityId = x.Facility.Id,
                x.Facility.DisplayName,
                x.Price,
                x.IsAvailable,
                x.IsPriceVisible,
                x.HasAction,
                x.BookingMode,
                x.FacilityActionType,

                Image = _ctx.FacilitiesImages
                    .Where(img =>
                        img.FacilityId == x.FacilityId &&
                        img.Category == ImageCategory.Main &&
                        img.IsActive==true &&
                        img.IsDeleted != true)
                    .Select(img => img.ImageURL)
                    .FirstOrDefault()
            })
            .ToListAsync(ct);

        var result = raw.Select(x =>
        {

            var actionTypeEnum = ResolveActionName(x.FacilityActionType);

            return new FacilityDTO
            {
                Id = x.FacilityId,
                Name = x.DisplayName,

                ImageUrl = x.Image != null
                    ? _fileStorageService.GetPublicUrl(x.Image)
                    : null,

                BookingMode = x.BookingMode,

                Price = x.IsPriceVisible ? x.Price : null,
                IsPriceVisible = x.IsPriceVisible,

                IsAvailable = x.IsAvailable,
                HasAction = x.HasAction,

                DisplayName = actionTypeEnum,
                ActionTypeEnum = x.FacilityActionType
            };
        }).ToList();

        return result;
    }

    // -----------------------------
    // ACTION RESOLUTION
    // -----------------------------
    
    private static string? ResolveActionName(FacilityActionType actionType)
    {
        return actionType switch
        {
            FacilityActionType.Book => "Book Now",
            FacilityActionType.Reserve => "Reserve Now",
            FacilityActionType.ContactUs => "Contact Us",
            FacilityActionType.ViewDetails => "View Details",
            _ => null
        };
    }


}


