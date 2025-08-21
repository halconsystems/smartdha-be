using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.ApprovalProcess.Queries.GetProcessStepsByProcessId;

public record GetProcessStepsByProcessIdQuery(int ProcessID) : IRequest<SuccessResponse<List<ProcessStepDto>>>;

public class GetProcessStepsByProcessIdQueryHandler: IRequestHandler<GetProcessStepsByProcessIdQuery, SuccessResponse<List<ProcessStepDto>>>
{
    private readonly IProcedureService _procedureService;
    public GetProcessStepsByProcessIdQueryHandler(IProcedureService procedureService)
    {
        _procedureService = procedureService;
    }

    public async Task<SuccessResponse<List<ProcessStepDto>>> Handle(GetProcessStepsByProcessIdQuery request, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@ProcessID", request.ProcessID, DbType.Int32);

        var (_, result) = await _procedureService.ExecuteWithListAsync<ProcessStepDto>(
            "USP_GetProcessStepsByProcessId", parameters, cancellationToken);

        return new SuccessResponse<List<ProcessStepDto>>(result, "Process steps fetched successfully.");
    }
}
