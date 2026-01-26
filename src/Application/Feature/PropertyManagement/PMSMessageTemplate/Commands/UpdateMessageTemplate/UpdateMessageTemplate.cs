using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSMessageTemplate.Commands.UpdateMessageTemplate;
public record UpdateMessageTemplateCommand(
    Guid Id,
    string Title,
    string Body,
    bool AllowSms,
    bool AllowNotification,
    int SmsMaxLength
) : IRequest<ApiResult<bool>>;
public class UpdateMessageTemplateHandler
    : IRequestHandler<UpdateMessageTemplateCommand, ApiResult<bool>>
{
    private readonly IPMSApplicationDbContext _db;

    public UpdateMessageTemplateHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        UpdateMessageTemplateCommand r,
        CancellationToken ct)
    {
        var template = await _db.Set<MessageTemplate>()
            .FirstOrDefaultAsync(x => x.Id == r.Id && x.IsDeleted!=true, ct);

        if (template == null)
            return ApiResult<bool>.Fail("Template not found.");

        template.Title = r.Title.Trim();
        template.Body = r.Body.Trim();
        template.AllowSms = r.AllowSms;
        template.AllowNotification = r.AllowNotification;
        template.SmsMaxLength = r.SmsMaxLength;

        await _db.SaveChangesAsync(ct);
        return ApiResult<bool>.Ok(true, "Template updated.");
    }
}

