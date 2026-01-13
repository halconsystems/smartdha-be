using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.PMS.Commands.AddCaseAttachment;
public record AddCaseAttachmentCommand(
    Guid UserId,
    int CaseId,
    int PrerequisitesID,
    string FilePath  //  saved file path
) : IRequest<SuccessResponse<string>>;
public class AddCaseAttachmentCommandHandler
    : IRequestHandler<AddCaseAttachmentCommand, SuccessResponse<string>>
{
    private readonly IProcedureService _sp;

    public AddCaseAttachmentCommandHandler(IProcedureService sp)
    {
        _sp = sp;
    }

    public async Task<SuccessResponse<string>> Handle(
        AddCaseAttachmentCommand request,
        CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@userID", request.UserId, DbType.Guid);
        parameters.Add("@caseID", request.CaseId, DbType.Int32);
        parameters.Add("@PrerequisitesID", request.PrerequisitesID, DbType.Int32);
        parameters.Add("@FilePath", request.FilePath, DbType.String);
        parameters.Add("@msg", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

        await _sp.ExecuteAsync(
            "USP_AddCaseAttachments",
            parameters,
            cancellationToken,
            "PMSConnection"
        );

        var message = parameters.Get<string>("@msg") ?? "Attachment added";

        return new SuccessResponse<string>(message);
    }
}

