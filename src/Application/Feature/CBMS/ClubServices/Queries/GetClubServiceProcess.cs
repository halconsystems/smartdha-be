using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSDirectorate.Queries.GetDirectorates;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSServiceProcess.Queries;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubServices.Queries;

public record GetClubServiceProcessQuery() : IRequest<ApiResult<List<ClubServiceProcessDTO>>>;

public class GetClubServiceProcessQueryHandler : IRequestHandler<GetClubServiceProcessQuery, ApiResult<List<ClubServiceProcessDTO>>>
{
    private readonly IOLMRSApplicationDbContext _db;
    public GetClubServiceProcessQueryHandler(IOLMRSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<List<ClubServiceProcessDTO>>> Handle(GetClubServiceProcessQuery request, CancellationToken ct)
    {

        var processes = await _db.Set<ClubServiceProcess>()
            .OrderBy(x => x.Name).ToListAsync(ct);

        var clubserviceImage = await _db.Set<Domain.Entities.CBMS.ClubServiceImages>()
             .Where(x => processes.Select(c => c.Id).Contains(x.ServiceId) && x.Category == Domain.Enums.ImageCategory.Main).ToListAsync(ct);

        var result = processes.Select(x =>
        {
            var imagePath = clubserviceImage?
                .FirstOrDefault(y => y.ServiceId == x.Id)?
                .ImageURL;

            return new ClubServiceProcessDTO(
                 x.Id,
                x.CategoryId,
                x.Name,
                x.Code,
                x.Description,
                imagePath,
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

