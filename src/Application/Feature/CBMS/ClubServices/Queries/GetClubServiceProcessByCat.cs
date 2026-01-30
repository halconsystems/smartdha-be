using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.Grounds.Queries;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSServiceProcess.Queries;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Entities.GBMS;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubServices.Queries;

public record GetClubServiceProcessByCatQuery(Guid CategoryId)
    : IRequest<ApiResult<List<ClubServiceProcessDTO>>>;
public class GetClubServiceProcessByCatQueryHandler
    : IRequestHandler<GetClubServiceProcessByCatQuery, ApiResult<List<ClubServiceProcessDTO>>>
{
    private readonly IOLMRSApplicationDbContext _db;
    private readonly IFileStorageService _fileStorage;

    public GetClubServiceProcessByCatQueryHandler(IOLMRSApplicationDbContext db,IFileStorageService fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    public async Task<ApiResult<List<ClubServiceProcessDTO>>> Handle(
        GetClubServiceProcessByCatQuery r,
        CancellationToken ct)
    {
        // Validate category
        var categoryExists = await _db.Set<Domain.Entities.CBMS.ClubCategories>()
            .AnyAsync(x => x.Id == r.CategoryId && x.IsDeleted != true, ct);


        if (!categoryExists)
            return ApiResult<List<ClubServiceProcessDTO>>.Fail("Category not found.");

        var processes = await _db.Set<ClubServiceProcess>()
            .Where(x =>
                x.CategoryId == r.CategoryId)
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

        var clubserviceImage = await _db.Set<Domain.Entities.CBMS.ClubServiceImages>()
            .Where(x => processes.Select(c => c.Id).Contains(x.ServiceId) && x.Category == Domain.Enums.ImageCategory.Main).ToListAsync(ct);


        var result = processes.Select(x =>
        {
            var imagePath = clubserviceImage?
                .FirstOrDefault(y => y.ServiceId == x.Id)?
                .ImageURL;
            var publicURL = imagePath != null ? _fileStorage.GetPublicUrl(imagePath) : null;

            return new ClubServiceProcessDTO(
                 x.Id,
                x.CategoryId,
                x.Name,
                x.Code,
                x.Description,
                publicURL,
                x.Price,
                x.FoodType,
                x.IsAvailable,
                x.IsPriceVisible,
                x.Action,
                x.ActionName,
                x.ActionType,
                x.IsFeeAtSubmission,
                x.IsVoucherPossible,
                x.IsFeeRequired,
                x.IsfeeSubmit,
                x.IsInstructionAtStart,
                x.IsButton,
                x.Instruction,

                x.IsActive,
                x.IsDeleted
            );
        }).ToList();

        return ApiResult<List<ClubServiceProcessDTO>>.Ok(result);
    }
}

