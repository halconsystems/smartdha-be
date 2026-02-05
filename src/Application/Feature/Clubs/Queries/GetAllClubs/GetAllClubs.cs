using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.CBMS.Clubs.Queries;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Clubs.Queries.GetAllClubs;

public record GetAllClubsDetailQuery
    : IRequest<ApiResult<List<ClubDTO>>>;

public class GetAllClubsDetailQueryHandler
    : IRequestHandler<GetAllClubsDetailQuery, ApiResult<List<ClubDTO>>>
{
    private readonly ICBMSApplicationDbContext _ctx;
    private readonly IFileStorageService _fileStorageService;

    public GetAllClubsDetailQueryHandler(
        ICBMSApplicationDbContext ctx,
        IFileStorageService fileStorageService)
    {
        _ctx = ctx;
        _fileStorageService = fileStorageService;
    }

    public async Task<ApiResult<List<ClubDTO>>> Handle(
        GetAllClubsDetailQuery request,
        CancellationToken ct)
    {
        // 1️⃣ All active clubs
        var clubs = await _ctx.Clubs
            .AsNoTracking()
            .Where(x => x.IsActive == true && x.IsDeleted != true)
            .ToListAsync(ct);

        if (!clubs.Any())
            return ApiResult<List<ClubDTO>>.Ok(new List<ClubDTO>());

        var clubIds = clubs.Select(x => x.Id).ToList();

        // 2️⃣ All categories per club
        var clubCategories = await _ctx.ClubFacilities
            .AsNoTracking()
            .Where(x =>
                clubIds.Contains(x.ClubId) &&
                x.IsAvailable &&
                x.IsActive == true &&
                x.IsDeleted != true)
            .Select(x => new
            {
                x.ClubId,
                CategoryId = x.Facility.ClubCategory.Id,
                CategoryName = x.Facility.ClubCategory.DisplayName
            })
            .Distinct()
            .ToListAsync(ct);

        // 3️⃣ All images per club
        var clubImages = await _ctx.ClubImages
            .AsNoTracking()
            .Where(x =>
                clubIds.Contains(x.ClubId) &&
                x.IsActive == true &&
                x.IsDeleted != true)
            .ToListAsync(ct);

        // 4️⃣ Map result
        var result = clubs.Select(club =>
        {
            var images = clubImages.Where(i => i.ClubId == club.Id).ToList();

            var mainImagePath = images
                .FirstOrDefault(x => x.Category == ImageCategory.Main)
                ?.ImageURL;

            var bannerPaths = images
                .Where(x => x.Category != ImageCategory.Main)
                .Select(x => x.ImageURL)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            var categories = clubCategories
                .Where(x => x.ClubId == club.Id)
                .ToList();

            return new ClubDTO
            {
                Id = club.Id,
                Name = club.Name,
                Location = club.Location,
                Description = club.Description,
                ContactNumber = club.ContactNumber,

                // 🔥 Highlights (names only)
                highlights = categories
                    .Select(x => x.CategoryName)
                    .ToList(),

                // 📂 Categories with Id
                Categories = categories
                    .Select(x => new HighlightDTO
                    {
                        Id = x.CategoryId,
                        Name = x.CategoryName
                    })
                    .ToList(),

                // 🖼 Main Image
                imageUrl = mainImagePath == null
                    ? null
                    : _fileStorageService.GetPublicUrl(mainImagePath),

                // 🧱 Banner Images
                bannerIamges = bannerPaths
                    .Select(p => _fileStorageService.GetPublicUrl(p))
                    .ToList()
            };
        }).ToList();

        return ApiResult<List<ClubDTO>>.Ok(result);
    }
}

