using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSServiceProcess.Queries;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubServices.Queries;

public record GetClubServiceProvessByIdQuery(Guid ProcessId)
    : IRequest<ApiResult<ClubServiceProcessDTO>>;

public class GetClubServiceProvessByIdQueryHandler
    : IRequestHandler<GetClubServiceProvessByIdQuery, ApiResult<ClubServiceProcessDTO>>
{
    private readonly IOLMRSApplicationDbContext _db;

    public GetClubServiceProvessByIdQueryHandler(IOLMRSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<ClubServiceProcessDTO>> Handle(
        GetClubServiceProvessByIdQuery r,
        CancellationToken ct)
    {
        var process = await _db.Set<ClubServiceProcess>()
            .FirstOrDefaultAsync(x =>
                x.Id == r.ProcessId, ct);
        if(process == null) return ApiResult<ClubServiceProcessDTO>.Fail("process not found.");

        var clubserviceImage = await _db.Set<Domain.Entities.CBMS.ClubServiceImages>()
             .FirstOrDefaultAsync(x => x.ServiceId == process.Id && x.Category == Domain.Enums.ImageCategory.Main, ct);



        var result = new ClubServiceProcessDTO(
                process.Id,
                process.CategoryId,
                process.Name,
                process.Code,
                process.Description,
                clubserviceImage?.ImageURL,
                process.Price,
                process.FoodType,
                process.IsAvailable,
                process.IsPriceVisible,
                process.Action,
                process.ActionName,
                process.ActionType,
                process.IsFeeAtSubmission,
                process.IsVoucherPossible,
                process.IsFeeRequired,
                process.IsfeeSubmit,
                process.IsInstructionAtStart,
                process.IsButton,
                process.Instruction,
                process.IsActive,
                process.IsDeleted
            );


        return ApiResult<ClubServiceProcessDTO>.Ok(result);
    }
}

