using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSMessageTemplate.Queries.GetMessageTemplateById;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSMessageTemplate.Queries.GetMessageTemplatesByModule;
public record GetMessageTemplatesByModuleQuery(Guid ModuleId)
    : IRequest<ApiResult<List<MessageTemplateDto>>>;
public class GetMessageTemplatesByModuleHandler
    : IRequestHandler<GetMessageTemplatesByModuleQuery, ApiResult<List<MessageTemplateDto>>>
{
    private readonly IPMSApplicationDbContext _db;

    public GetMessageTemplatesByModuleHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<List<MessageTemplateDto>>> Handle(
        GetMessageTemplatesByModuleQuery r,
        CancellationToken ct)
    {
        var list = await _db.Set<MessageTemplate>()
            .AsNoTracking()
            .Where(x =>
                x.ModuleId == r.ModuleId &&
                x.IsDeleted !=true)
            .OrderBy(x => x.Code)
            .Select(x => new MessageTemplateDto(
                x.Id,
                x.ModuleId,
                x.Code,
                x.Title,
                x.Body,
                x.AllowSms,
                x.AllowNotification,
                x.SmsMaxLength
            ))
            .ToListAsync(ct);

        return ApiResult<List<MessageTemplateDto>>.Ok(list);
    }
}

