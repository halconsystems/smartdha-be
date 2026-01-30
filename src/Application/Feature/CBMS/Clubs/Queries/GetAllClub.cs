using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.Clubs.Queries;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.Clubs.Queries;

public record GetAllClubQuery()
    : IRequest<ApiResult<List<ClubDTO>>>;

public class GetAllClubQueryHandler
    : IRequestHandler<GetAllClubQuery, ApiResult<List<ClubDTO>>>
{
    private readonly IOLMRSApplicationDbContext _ctx;       // Clubs DbContext
    private readonly IApplicationDbContext _appCtx;         // Auth/Assignments DbContext
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _fileStorageService;

    public GetAllClubQueryHandler(
       IOLMRSApplicationDbContext ctx,
       IApplicationDbContext appCtx,
       ICurrentUserService currentUser,
       IMapper mapper,
       IFileStorageService fileStorageService)
    {
        _ctx = ctx;
        _appCtx = appCtx;
        _currentUser = currentUser;
        _mapper = mapper;
        _fileStorageService = fileStorageService;
    }

    public async Task<ApiResult<List<ClubDTO>>> Handle(GetAllClubQuery request, CancellationToken ct)
    {

        var clubs = await _ctx.Clubs
        .AsNoTracking()
        .ToListAsync(ct);

        if (!clubs.Any())
            return ApiResult<List<ClubDTO>>.Fail("Club not found.");

        var clubIds = clubs.Select(c => c.Id).ToList();


        var clubImages = _ctx.ClubImages.Where(x => clubs.Select(c => c.Id).Contains(x.ClubId))
            .AsNoTracking()
            .ToList();

        var clubCategories = await _ctx.ClubFacilities
        .AsNoTracking()
        .Where(x => clubIds.Contains(x.ClubId) && x.IsAvailable)
        .Select(x => new
        {
            x.ClubId,
            CategoryId = x.Facility.ClubCategory.Id,
            CategoryName = x.Facility.ClubCategory.DisplayName
        })
        .Distinct()
        .ToListAsync(ct);




        var result = clubs.Select(c =>
        {
            var imagePath = clubImages
                .FirstOrDefault(x => x.ClubId == c.Id && x.Category == ImageCategory.Main)
                ?.ImageURL;

            var bannerPaths = clubImages
                .Where(x => x.ClubId == c.Id && x.Category != ImageCategory.Main)
                .Select(x => x.ImageURL)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            var categories = clubCategories
                .Where(x => x.ClubId == c.Id)
                .ToList();

            return new ClubDTO
            {
                Id = c.Id,
                Name = c.Name,
                Location = c.Location,
                Description = c.Description,
                ContactNumber = c.ContactNumber,

                // 🔥 HIGHLIGHTS = CATEGORY NAMES
                highlights = categories
                    .Select(x => x.CategoryName)
                    .ToList(),

                Categories = categories
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
        }).ToList();



        return ApiResult<List<ClubDTO>>.Ok(result);
    }

}



