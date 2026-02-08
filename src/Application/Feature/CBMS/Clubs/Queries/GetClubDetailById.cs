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
        .FirstOrDefaultAsync(x => x.Id == request.ClubId && x.IsActive==true && x.IsDeleted !=true, ct);

        if (club == null)
            return ApiResult<ClubDTO>.Fail("Club not found.");

        var clubCategories = await _ctx.ClubFacilities
    .AsNoTracking()
    .Where(x =>
        x.ClubId == request.ClubId &&
        x.IsAvailable   ==true &&
        x.IsActive == true &&
        x.IsDeleted != true &&
        x.Facility.ClubCategory.IsActive == true &&
        x.Facility.ClubCategory.IsDeleted != true
    )
    .Select(x => x.Facility.ClubCategory)
    .GroupBy(c => c.Id)
    .Select(g => new
    {
        CategoryId = g.Key,
        CategoryName = g.First().DisplayName,
        OrderNo = g.First().OrderNo ?? int.MaxValue // 🔥 handle null orders
    })
    .OrderBy(x => x.OrderNo) // ✅ ORDER BY CATEGORY ORDER
    .ToListAsync(ct);

        //var clubCategories = await _ctx.ClubFacilities
        // .AsNoTracking()
        // .Where(x => x.ClubId == request.ClubId && x.IsAvailable && x.IsActive == true && x.IsDeleted != true)
        // .Select(x => new
        // {
        //     CategoryId = x.Facility.ClubCategory.Id,
        //     CategoryName = x.Facility.ClubCategory.DisplayName,
        //     OrderNo = g.First().OrderNo ?? int.MaxValue // 🔥 handle null orders
        // })
        // .Distinct()
        // .ToListAsync(ct);

        var clubImages = await _ctx.ClubImages
        .AsNoTracking()
        .Where(x => x.ClubId == request.ClubId && x.IsActive == true && x.IsDeleted != true)
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



