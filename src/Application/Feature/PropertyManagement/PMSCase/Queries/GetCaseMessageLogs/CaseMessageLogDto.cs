using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Queries.GetCaseMessageLogs;
public class CaseMessageLogDto
{
    public Guid MessageLogId { get; set; }

    public Guid CaseId { get; set; }

    // 📄 Template
    public Guid TemplateId { get; set; }
    public string TemplateCode { get; set; } = default!;
    public string TemplateTitle { get; set; } = default!;

    // 👤 Sender
    public string? SentByUserId { get; set; }
    public string? SentByUserName { get; set; }

    // 👥 Recipient
    public string? RecipientUserId { get; set; }
    public string? RecipientUserName { get; set; }
    public string? RecipientMobile { get; set; }

    // 📡 Channel
    public MessageChannel Channel { get; set; }

    // ✉️ SMS
    public bool SmsSent { get; set; }
    public string? SmsText { get; set; }

    // 🔔 Notification
    public bool NotificationSent { get; set; }
    public string? NotificationTitle { get; set; }
    public string? NotificationBody { get; set; }

    // ⚠️ Failure
    public string? FailureReason { get; set; }

    // 🕒 Audit
    public DateTime SentAt { get; set; }
}

