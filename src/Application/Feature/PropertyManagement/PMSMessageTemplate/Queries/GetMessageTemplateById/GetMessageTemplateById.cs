using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSMessageTemplate.Queries.GetMessageTemplateById;

public record MessageTemplateDto(
    Guid Id,
    Guid ModuleId,
    string Code,
    string Title,
    string Body,
    bool AllowSms,
    bool AllowNotification,
    int SmsMaxLength
);

public record GetMessageTemplateByIdQuery(Guid Id)
    : IRequest<ApiResult<MessageTemplateDto>>;

public class GetMessageTemplateByIdHandler
    : IRequestHandler<GetMessageTemplateByIdQuery, ApiResult<MessageTemplateDto>>
{
    private readonly IPMSApplicationDbContext _db;

    public GetMessageTemplateByIdHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<MessageTemplateDto>> Handle(
        GetMessageTemplateByIdQuery r,
        CancellationToken ct)
    {
        var t = await _db.Set<MessageTemplate>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == r.Id && x.IsDeleted !=true, ct);

        if (t == null)
            return ApiResult<MessageTemplateDto>.Fail("Template not found.");

        return ApiResult<MessageTemplateDto>.Ok(
            new MessageTemplateDto(
                t.Id,
                t.ModuleId,
                t.Code,
                t.Title,
                t.Body,
                t.AllowSms,
                t.AllowNotification,
                t.SmsMaxLength
            ));
    }
}
