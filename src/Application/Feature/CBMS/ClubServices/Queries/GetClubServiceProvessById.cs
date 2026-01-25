using System;
using System.Collections.Generic;
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
            .Where(x =>
                x.Id == r.ProcessId)
            .Select(x => new ClubServiceProcessDTO(
                x.Id,
                x.CategoryId,
                x.Name,
                x.Code,
                x.Description,
                x.IsFeeAtSubmission,
                x.IsVoucherPossible,
                x.IsFeeRequired,
                x.IsfeeSubmit,
                x.IsInstructionAtStart,
                x.IsButton,
                x.Instruction,
                x.IsActive,
                x.IsDeleted
            ))
            .FirstOrDefaultAsync(ct);

        if (process == null)
            return ApiResult<ClubServiceProcessDTO>.Fail("Process not found.");

        return ApiResult<ClubServiceProcessDTO>.Ok(process);
    }
}

