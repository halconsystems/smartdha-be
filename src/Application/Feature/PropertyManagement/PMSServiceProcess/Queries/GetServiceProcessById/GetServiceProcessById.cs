using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSServiceProcess.Queries.GetServiceProcessById;
public record GetServiceProcessByIdQuery(Guid ProcessId)
    : IRequest<ApiResult<ServiceProcessDto>>;

public class GetServiceProcessByIdHandler
    : IRequestHandler<GetServiceProcessByIdQuery, ApiResult<ServiceProcessDto>>
{
    private readonly IPMSApplicationDbContext _db;

    public GetServiceProcessByIdHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<ServiceProcessDto>> Handle(
        GetServiceProcessByIdQuery r,
        CancellationToken ct)
    {
        var process = await _db.Set<ServiceProcess>()
            .Where(x =>
                x.Id == r.ProcessId &&
                x.IsDeleted != true &&
                x.IsActive == true)
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
            .FirstOrDefaultAsync(ct);

        if (process == null)
            return ApiResult<ServiceProcessDto>.Fail("Process not found.");

        return ApiResult<ServiceProcessDto>.Ok(process);
    }
}
