using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.PMS.Commands.AddCase;

public record AddCaseCommand(
    int AppID,
    string PlotPK,
    string MemPK,
    int ProcessID,
    int PriorityID

) : IRequest<SuccessResponse<string>>;

public class AddCaseCommandHandler
    : IRequestHandler<AddCaseCommand, SuccessResponse<string>>
{
    
    private readonly IProcedureService _sp;
    private readonly ICurrentUserService _current;

    public AddCaseCommandHandler( IProcedureService sp, ICurrentUserService current)
    {
          _sp = sp;
        _current = current;
    }

    public async Task<SuccessResponse<string>> Handle(
        AddCaseCommand request,
        CancellationToken cancellationToken)
    {

        var userId = _current.UserId;

        if (userId == Guid.Empty)
            throw new UnauthorizedAccessException("User not identified");

        var parameters = new DynamicParameters();
        parameters.Add("@user_ID", userId, DbType.Guid);
        parameters.Add("@AppID",request.AppID,DbType.Int32);
        parameters.Add("@PlotPK", request.PlotPK, DbType.String, size: 20);
        parameters.Add("@MemPK", request.MemPK, DbType.String, size: 20);
        parameters.Add("@ProcessID", request.ProcessID, DbType.Int32);
        parameters.Add("@PriorityID",request.PriorityID, DbType.Int32);
        parameters.Add("@msg", dbType: DbType.String,size:150, direction:ParameterDirection.Output);



        await _sp.ExecuteAsync("USP_AddCase",
            parameters,
            cancellationToken,
            "PMSConnection");

        string message = parameters.Get<string>("@msg") ?? "No message";

        return new SuccessResponse<string>(message);


    }
}
