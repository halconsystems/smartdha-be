using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSMessageTemplate.Commands.PMSMessageTemplate;
public record CreateMessageTemplateCommand(
    Guid ModuleId,
    string Code,
    string Title,
    string Body,
    bool AllowSms,
    bool AllowNotification,
    int SmsMaxLength
) : IRequest<ApiResult<Guid>>;
public class CreateMessageTemplateHandler
    : IRequestHandler<CreateMessageTemplateCommand, ApiResult<Guid>>
{
    private readonly IPMSApplicationDbContext _db;

    public CreateMessageTemplateHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<Guid>> Handle(
        CreateMessageTemplateCommand r,
        CancellationToken ct)
    {
        var exists = await _db.Set<MessageTemplate>()
            .AnyAsync(x =>
                x.ModuleId == r.ModuleId &&
                x.Code == r.Code &&
                x.IsDeleted !=true,
                ct);

        if (exists)
            return ApiResult<Guid>.Fail("Template code already exists for this module.");

        var entity = new MessageTemplate
        {
            ModuleId = r.ModuleId,
            Code = r.Code.Trim().ToUpperInvariant(),
            Title = r.Title.Trim(),
            Body = r.Body.Trim(),
            AllowSms = r.AllowSms,
            AllowNotification = r.AllowNotification,
            SmsMaxLength = r.SmsMaxLength
        };

        _db.Set<MessageTemplate>().Add(entity);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(entity.Id, "Template created.");
    }
}

