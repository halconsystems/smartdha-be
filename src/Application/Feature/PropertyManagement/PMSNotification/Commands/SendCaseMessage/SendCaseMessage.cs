using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSNotification.Commands.SendCaseMessage;

public record SendCaseMessageCommand
(
    Guid CaseId,
    MessageChannel Channel,
    Guid TemplateId
) : IRequest<ApiResult<bool>>;

public class SendCaseMessageHandler
    : IRequestHandler<SendCaseMessageCommand, ApiResult<bool>>
{
    private readonly IPMSApplicationDbContext _pmsDb;
    private readonly IApplicationDbContext _appDb;
    private readonly ISmsService _sms;
    private readonly INotificationService _notify;


    public SendCaseMessageHandler(
        IPMSApplicationDbContext pmsDb,
        IApplicationDbContext appDb,
        ISmsService sms,
        INotificationService notify)
    {
        _pmsDb = pmsDb;
        _appDb = appDb;
        _sms = sms;
        _notify = notify;
    }

    public async Task<ApiResult<bool>> Handle(
        SendCaseMessageCommand r,
        CancellationToken ct)
    {
        // 1️⃣ Load case
        var c = await _pmsDb.Set<PropertyCase>()
            .Include(x => x.Process)
            .FirstOrDefaultAsync(x => x.Id == r.CaseId, ct);
        if (c == null)
            return ApiResult<bool>.Fail("Case not found.");

        UserDevices? getToken = null;

        if (!string.IsNullOrWhiteSpace(c.CreatedBy) &&
            Guid.TryParse(c.CreatedBy, out var createdByUserId))
        {
            getToken = await _pmsDb.Set<UserDevices>()
                .FirstOrDefaultAsync(x => x.UserId == createdByUserId, ct);
        }

        if (getToken == null)
            return ApiResult<bool>.Fail("User not found.");
        if(getToken.FCMToken ==null)
            return ApiResult<bool>.Fail("User Token found.");

        // 2️⃣ Load applicant user
        var user = await _appDb.Set<ApplicationUser>()
            .FirstOrDefaultAsync(x => x.CNIC == c.ApplicantCnic, ct);

        if (user == null)
            return ApiResult<bool>.Fail("Applicant user not found.");

        // 3️⃣ Load template (MODULE-SAFE)
        var template = await _appDb.Set<MessageTemplate>()
            .FirstOrDefaultAsync(x =>
                x.Id == r.TemplateId &&
                x.ModuleId == c.CurrentModuleId &&
                x.IsActive ==true &&
                x.IsDeleted !=true,
                ct);

        if (template == null)
            return ApiResult<bool>.Fail("Template not found for this module.");

        // 4️⃣ Build placeholders (SYSTEM CONTROLLED)
        //var placeholders = new Dictionary<string, string>
        //{
        //    ["CaseNo"] = c.CaseNo,
        //    ["Process"] = c.Process.Name
        //};

        //string Resolve(string text)
        //{
        //    foreach (var kv in placeholders)
        //        text = text.Replace($"{{{{{kv.Key}}}}}", kv.Value);

        //    return text;
        //}

        var title = template.Title;
        var message = template.Body;

        var mobile = user.RegisteredMobileNo;

        // 5️⃣ SEND SMS
        if ((r.Channel == MessageChannel.Sms || r.Channel == MessageChannel.Both)
            && template.AllowSms)
        {
            if (string.IsNullOrWhiteSpace(mobile))
                return ApiResult<bool>.Fail("User mobile number not found.");

            if (message.Length > template.SmsMaxLength)
                return ApiResult<bool>.Fail(
                    $"SMS exceeds {template.SmsMaxLength} characters.");

            await _sms.SendSmsAsync(mobile, message, ct);
        }

        // 6️⃣ SEND NOTIFICATION
        if ((r.Channel == MessageChannel.Notification || r.Channel == MessageChannel.Both)
            && template.AllowNotification)
        {
            await _notify.SendFirebasePMSNotificationAsync(
                getToken.FCMToken,
                title,
                message,
                ct);
        }

        return ApiResult<bool>.Ok(true, "Message sent successfully.");
    }
}


