using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.Clubs.Queries;

public record GetClubDetailById(Guid ClubId)
    : IRequest<ApiResult<ClubDTO>>;

public class GetClubDetailByIdHandler
    : IRequestHandler<GetClubDetailById, ApiResult<ClubDTO>>
{
    private readonly ICBMSApplicationDbContext _ctx;       // Clubs DbContext
    private readonly IFileStorageService _fileStorageService;

    public GetClubDetailByIdHandler(
       ICBMSApplicationDbContext ctx,
       IFileStorageService fileStorageService)
    {
        _ctx = ctx;
        _fileStorageService = fileStorageService;
    }

    public async Task<ApiResult<ClubDTO>> Handle(GetClubDetailById request, CancellationToken ct)
    {
        var club = await _ctx.Clubs
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.Id == request.ClubId, ct);

        if (club == null)
            return ApiResult<ClubDTO>.Fail("Club not found.");

        var clubCategories = await _ctx.ClubFacilities
         .AsNoTracking()
         .Where(x => x.ClubId == request.ClubId && x.IsAvailable)
         .Select(x => new
         {
             CategoryId = x.Facility.ClubCategory.Id,
             CategoryName = x.Facility.ClubCategory.DisplayName
         })
         .Distinct()
         .ToListAsync(ct);

        var clubImages = await _ctx.ClubImages
        .AsNoTracking()
        .Where(x => x.ClubId == request.ClubId)
        .ToListAsync(ct);


        var imagePath = clubImages
    .FirstOrDefault(x => x.Category == ImageCategory.Main)
    ?.ImageURL;

        var bannerPaths = clubImages
            .Where(x => x.Category != ImageCategory.Main)
            .Select(x => x.ImageURL)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        var result = new ClubDTO
        {
            Id = club.Id,
            Name = club.Name,
            Location = club.Location,
            Description = club.Description,
            ContactNumber = club.ContactNumber,

            // 🔥 HIGHLIGHTS = CATEGORY NAMES
            highlights = clubCategories
                .Select(x => x.CategoryName)
                .ToList(),

            Categories = clubCategories
                .Select(x => new HighlightDTO
                {
                    Id = x.CategoryId,
                    Name = x.CategoryName
                })
                .ToList(),

            imageUrl = imagePath == null
                ? null
                : _fileStorageService.GetPublicUrl(imagePath),

            bannerIamges = bannerPaths
                .Select(path => _fileStorageService.GetPublicUrl(path))
                .ToList()
        };


        return ApiResult<ClubDTO>.Ok(result);
    }

}



