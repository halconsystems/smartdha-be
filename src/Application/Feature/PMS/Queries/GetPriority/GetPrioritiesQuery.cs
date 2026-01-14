using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.PMS.Queries.GetPriority;
public record GetPrioritiesQuery(
    Guid UserId,
    int AppID,
    int ProcessID
) : IRequest<SuccessResponse<List<PriorityDto>>>;

public class GetPrioritiesQueryHandler
    : IRequestHandler<GetPrioritiesQuery, SuccessResponse<List<PriorityDto>>>
{
    private readonly IProcedureService _sp;

    public GetPrioritiesQueryHandler(IProcedureService sp)
    {
        _sp = sp;
    }

    public async Task<SuccessResponse<List<PriorityDto>>> Handle(
        GetPrioritiesQuery request,
        CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@user_ID", request.UserId, DbType.Guid);
        parameters.Add("@AppID", request.AppID, DbType.Int32);
        parameters.Add("@ProcessID", request.ProcessID, DbType.Int32);
        parameters.Add(
            "@msg",
            dbType: DbType.String,
            size: 150,
            direction: ParameterDirection.Output
        );

        var (outParams, priorities) =
            await _sp.ExecuteWithListAsync<PriorityDto>(
                "USP_SelectPriority",
                parameters,
                cancellationToken,
                "PMSConnection"
            );

        var message = outParams.Get<string>("@msg")
                      ?? "Priorities fetched successfully";

        return new SuccessResponse<List<PriorityDto>>(priorities, message);
    }
}

