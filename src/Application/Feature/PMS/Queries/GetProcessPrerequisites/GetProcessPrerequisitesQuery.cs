using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.PMS.Queries.GetProcessPrerequisites;
public record GetProcessPrerequisitesQuery(
    Guid UserId,
    int ProcessID
) : IRequest<SuccessResponse<List<ProcessPrerequisitesDto>>>;

public class GetProcessPrerequisitesQueryHandler
    : IRequestHandler<GetProcessPrerequisitesQuery, SuccessResponse<List<ProcessPrerequisitesDto>>>
{
    private readonly IProcedureService _sp;

    public GetProcessPrerequisitesQueryHandler(IProcedureService sp)
    {
        _sp = sp;
    }

    public async Task<SuccessResponse<List<ProcessPrerequisitesDto>>> Handle(
        GetProcessPrerequisitesQuery request,
        CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@user_ID", request.UserId, DbType.Guid);
        parameters.Add("@ProcessID", request.ProcessID, DbType.Int32);
        parameters.Add("@msg", dbType: DbType.String, size: 150, direction: ParameterDirection.Output);

        var (outParams, data) =
            await _sp.ExecuteWithListAsync<ProcessPrerequisitesDto>(
                "USP_SelectProcessPrerequisites",
                parameters,
                cancellationToken,
                "PMSConnection");

        string message = outParams.Get<string>("@msg") ?? "Success";

        return new SuccessResponse<List<ProcessPrerequisitesDto>>(data, message);
    }
}
