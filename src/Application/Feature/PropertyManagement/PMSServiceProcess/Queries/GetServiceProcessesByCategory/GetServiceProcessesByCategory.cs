using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSServiceProcess.Queries.GetServiceProcessesByCategory;
public record GetServiceProcessesByCategoryQuery(Guid CategoryId)
    : IRequest<ApiResult<List<ServiceProcessDto>>>;
public class GetServiceProcessesByCategoryHandler
    : IRequestHandler<GetServiceProcessesByCategoryQuery, ApiResult<List<ServiceProcessDto>>>
{
    private readonly IPMSApplicationDbContext _db;

    public GetServiceProcessesByCategoryHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<List<ServiceProcessDto>>> Handle(
        GetServiceProcessesByCategoryQuery r,
        CancellationToken ct)
    {
        // Validate category
        var categoryExists = await _db.Set<ServiceCategory>()
            .AnyAsync(x => x.Id == r.CategoryId && x.IsDeleted != true, ct);

        if (!categoryExists)
            return ApiResult<List<ServiceProcessDto>>.Fail("Category not found.");

        var processes = await _db.Set<ServiceProcess>()
            .Where(x =>
                x.CategoryId == r.CategoryId &&
                x.IsDeleted != true &&
                x.IsActive == true)
            .OrderBy(x => x.Name)
            .Select(x => new ServiceProcessDto(
                x.Id,
                x.CategoryId,
                x.Name,
                x.Code,

                x.IsFeeAtSubmission,
                x.IsVoucherPossible,
                x.IsFeeRequired,
                x.IsNadraVerificationRequired,
                x.IsfeeSubmit,
                x.IsInstructionAtStart,
                x.Instruction
            ))
            .ToListAsync(ct);

        return ApiResult<List<ServiceProcessDto>>.Ok(processes);
    }
}

