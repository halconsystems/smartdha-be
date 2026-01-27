using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Queries.GetCaseMessageLogs;



public record GetCaseMessageLogsQuery(Guid CaseId)
    : IRequest<ApiResult<List<CaseMessageLogDto>>>;
public class GetCaseMessageLogsHandler
    : IRequestHandler<GetCaseMessageLogsQuery, ApiResult<List<CaseMessageLogDto>>>
{
    private readonly IPMSApplicationDbContext _pmsDb;
    private readonly IApplicationDbContext _appDb;

    public GetCaseMessageLogsHandler(
        IPMSApplicationDbContext pmsDb,
        IApplicationDbContext appDb)
    {
        _pmsDb = pmsDb;
        _appDb = appDb;
    }

    public async Task<ApiResult<List<CaseMessageLogDto>>> Handle(
        GetCaseMessageLogsQuery r,
        CancellationToken ct)
    {
        // 1️⃣ Load logs
        var logs = await _pmsDb.Set<CaseMessageLog>()
            .AsNoTracking()
            .Include(x => x.Template)
            .Where(x => x.CaseId == r.CaseId && x.IsDeleted != true)
            .OrderByDescending(x => x.Created)
            .ToListAsync(ct);

        if (!logs.Any())
            return ApiResult<List<CaseMessageLogDto>>.Ok(new());

        // 2️⃣ Collect user IDs
        var userIds = logs
            .SelectMany(x => new[] { x.SentByUserId, x.RecipientUserId })
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToList();

        // 3️⃣ Load users
        var users = await _appDb.Set<ApplicationUser>()
            .AsNoTracking()
            .Where(x => userIds.Contains(x.Id))
            .Select(x => new { x.Id, x.Name })
            .ToListAsync(ct);

        var userLookup = users.ToDictionary(x => x.Id, x => x.Name);

        // 4️⃣ Map
        var result = logs.Select(x => new CaseMessageLogDto
        {
            MessageLogId = x.Id,
            CaseId = x.CaseId,

            TemplateId = x.TemplateId,
            TemplateCode = x.Template.Code,
            TemplateTitle = x.Template.Title,

            SentByUserId = x.SentByUserId,
            SentByUserName =
                x.SentByUserId != null && userLookup.ContainsKey(x.SentByUserId)
                    ? userLookup[x.SentByUserId]
                    : null,

            RecipientUserId = x.RecipientUserId,
            RecipientUserName =
                x.RecipientUserId != null && userLookup.ContainsKey(x.RecipientUserId)
                    ? userLookup[x.RecipientUserId]
                    : null,

            RecipientMobile = x.RecipientMobile,

            Channel = x.Channel,

            SmsSent = x.SmsSent,
            SmsText = x.SmsText,

            NotificationSent = x.NotificationSent,
            NotificationTitle = x.NotificationTitle,
            NotificationBody = x.NotificationBody,

            FailureReason = x.FailureReason,

            SentAt = x.Created
        }).ToList();

        return ApiResult<List<CaseMessageLogDto>>.Ok(result);
    }
}
