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

public record GetClubServiceProcessByCatQuery(Guid CategoryId)
    : IRequest<ApiResult<List<ClubServiceProcessDTO>>>;
public class GetClubServiceProcessByCatQueryHandler
    : IRequestHandler<GetClubServiceProcessByCatQuery, ApiResult<List<ClubServiceProcessDTO>>>
{
    private readonly IOLMRSApplicationDbContext _db;

    public GetClubServiceProcessByCatQueryHandler(IOLMRSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<List<ClubServiceProcessDTO>>> Handle(
        GetClubServiceProcessByCatQuery r,
        CancellationToken ct)
    {
        // Validate category
        var categoryExists = await _db.Set<ServiceCategory>()
            .AnyAsync(x => x.Id == r.CategoryId && x.IsDeleted != true, ct);

        if (!categoryExists)
            return ApiResult<List<ClubServiceProcessDTO>>.Fail("Category not found.");

        var processes = await _db.Set<ClubServiceProcess>()
            .Where(x =>
                x.CategoryId == r.CategoryId)
            .OrderBy(x => x.Name)
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
            .ToListAsync(ct);

        return ApiResult<List<ClubServiceProcessDTO>>.Ok(processes);
    }
}

