using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.AddProcessStep.Queries.GetStepsByProcess;
public record StepDto(Guid Id, int StepNo, string Name, Guid DirectorateId, bool RequiresVoucher, bool RequiresPaymentBeforeNext);
public record StepInfoDto(Guid Id, int StepNo, string Name, Guid DirectorateId ,string DirectorateName, bool RequiresVoucher, bool RequiresPaymentBeforeNext);

public record GetStepsByProcessQuery(Guid ProcessId) : IRequest<ApiResult<List<StepInfoDto>>>;

public class GetStepsByProcessHandler : IRequestHandler<GetStepsByProcessQuery, ApiResult<List<StepInfoDto>>>
{
    private readonly IPMSApplicationDbContext _db;
    public GetStepsByProcessHandler(IPMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<List<StepInfoDto>>> Handle(GetStepsByProcessQuery q, CancellationToken ct)
    {
        var list = await _db.Set<ProcessStep>()
            .Where(x => x.ProcessId == q.ProcessId && x.IsActive==true && x.IsDeleted !=true)
            .OrderBy(x => x.StepNo)
            .Select(x => new StepInfoDto(x.Id, x.StepNo, x.Name, x.DirectorateId,x.Directorate.Name ,x.RequiresVoucher, x.RequiresPaymentBeforeNext))
            .ToListAsync(ct);

        return ApiResult<List<StepInfoDto>>.Ok(list);
    }
}

