using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.PMS.Queries.GetProcesses;
public record GetProcessTypesQuery(
    int UserId,
    AppType AppID
) : IRequest<SuccessResponse<List<ProcessTypeDto>>>;

public class GetProcessTypesQueryHandler
    : IRequestHandler<GetProcessTypesQuery, SuccessResponse<List<ProcessTypeDto>>>
{
    private readonly IProcedureService _sp;

    public GetProcessTypesQueryHandler(IProcedureService sp)
    {
        _sp = sp;
    }

    public async Task<SuccessResponse<List<ProcessTypeDto>>> Handle(
        GetProcessTypesQuery request,
        CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@user_ID", request.UserId, DbType.Int32);
        parameters.Add("@AppID", request.AppID, DbType.Int32);
        parameters.Add("@msg", dbType: DbType.String, size: 150, direction: ParameterDirection.Output);

        var (outParams, data) = await _sp.ExecuteWithListAsync<ProcessTypeDto>(
           "USP_SelectProcess",
           parameters,
           cancellationToken,
           "PMSConnection");

        string message = outParams.Get<string>("@msg") ?? "Success";

        return new SuccessResponse<List<ProcessTypeDto>>(data, message);
    }
}
