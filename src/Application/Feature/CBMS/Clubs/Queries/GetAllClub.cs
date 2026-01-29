using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubServices.Queries;
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

        if(clubs == null)
            return ApiResult<List<ClubDTO>>.Fail("Club not found.");

        var clubsCategory = await _ctx.ClubCategories
            .Where(x => clubs.Select(c => c.Id).Contains(x.ClubId))
            .AsNoTracking()
            .ToListAsync(ct);


        var clubImages = _ctx.ClubImages.Where(x => clubs.Select(c => c.Id).Contains(x.ClubId))
            .AsNoTracking()
            .ToList();



        var result = clubs.Select(c =>
        {
            var imagePath = clubImages?
                .FirstOrDefault(x => x.ClubId == c.Id && x.Category == ImageCategory.Main)
                ?.ImageURL;

            var bannerPaths = clubImages?
                .Where(x => x.ClubId == c.Id && x.Category != ImageCategory.Main)
                .Select(x => x.ImageURL)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            return new ClubDTO
            {
                Id = c.Id,
                Name = c.Name,
                Location = c.Location,

                highlights = clubsCategory != null
                ? clubsCategory
                    .Where(x => x.ClubId == c.Id)
                    .Select(x => x.Name)
                    .ToList()
                : new List<string>(),

                //highlights = clubsCategory != null
                //    ? clubsCategory
                //        .Where(x => x.ClubId == c.Id)
                //        .Select(x => x.Name)
                //        .ToList()
                //    : new List<string>(),

                imageUrl = imagePath == null
                    ? null
                    : _fileStorageService.GetPublicUrl(imagePath),

                bannerIamges = bannerPaths == null
                    ? new List<string>()
                    : bannerPaths
                        .Select(path => _fileStorageService.GetPublicUrl(path))
                        .ToList(),
                Description = c.Description,
                ContactNumber = c.ContactNumber,
                Categories = clubsCategory != null
                ? clubsCategory
                .Where(x => x.ClubId == c.Id)
                .Select(x => new HighlightDTO
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList()
                : new List<HighlightDTO>()

            };
        }).ToList();


        return ApiResult<List<ClubDTO>>.Ok(result);
    }

}



