using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubServices.Queries;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.Clubs.Queries;



public record GetClubDetailById(Guid ClubId)
    : IRequest<ApiResult<ClubDTO>>;

public class GetClubDetailByIdHandler
    : IRequestHandler<GetClubDetailById, ApiResult<ClubDTO>>
{
    private readonly IOLMRSApplicationDbContext _ctx;       // Clubs DbContext
    private readonly IApplicationDbContext _appCtx;         // Auth/Assignments DbContext
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _fileStorageService;

    public GetClubDetailByIdHandler(
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

    public async Task<ApiResult<ClubDTO>> Handle(GetClubDetailById request, CancellationToken ct)
    {

        var clubs = await _ctx.Clubs
            .FirstOrDefaultAsync(x => x.Id == request.ClubId, ct);

        if (clubs == null)
            return ApiResult<ClubDTO>.Fail("Club not found.");

        var clubsCategory = await _ctx.ClubCategories
            .Where(x => x.ClubId == clubs.Id)
            .AsNoTracking()
            .ToListAsync(ct);

        if(clubsCategory == null)
            return ApiResult<ClubDTO>.Fail("Club Category not found.");

        var clubServices = await _ctx.ClubProcess
            .Where(x => clubsCategory.Select(c => c.Id).Contains(x.CategoryId))
            .AsNoTracking()
            .ToListAsync(ct);

        var clubImages = _ctx.ClubImages.Where(x => x.ClubId == clubs.Id)
            .AsNoTracking()
            .ToList();


        var imagePath = clubImages?
     .FirstOrDefault(x => x.ClubId == request.ClubId && x.Category == ImageCategory.Main)
     ?.ImageURL;

        var bannerPaths = clubImages?
            .Where(x => x.ClubId == request.ClubId && x.Category != ImageCategory.Main)
            .Select(x => x.ImageURL)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        var clubCategoryIds = clubsCategory?
            .Where(c => c.ClubId == clubs.Id)
            .Select(c => c.Id)
            .ToHashSet()
            ?? new HashSet<Guid>();


        var result = new ClubDTO
        {
            Id = request.ClubId,
            Name = clubs.Name,
            Location = clubs.Location,

            highlights = clubsCategory != null
                ? clubsCategory
                    .Where(x => x.ClubId == request.ClubId)
                    .Select(x => new HighlightDTO
                    {
                        Id = x.Id,
                        Name = x.Name
                    }).ToList()
                : new List<HighlightDTO>(),

            imageUrl = imagePath == null
                ? null
                : _fileStorageService.GetPublicUrl(imagePath),

            bannerIamges = bannerPaths?
                .Select(path => _fileStorageService.GetPublicUrl(path))
                .ToList() ?? new List<string>(),

            Description = clubs.Description,
            ContactNumber = clubs.ContactNumber,

        };



        return ApiResult<ClubDTO>.Ok(result);
    }

}



