using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Domain.Entities.PMS;
public class CaseMessageLog : BaseAuditableEntity
{
    public Guid CaseId { get; set; }
    public PropertyCase Case { get; set; } = default!;

    public Guid TemplateId { get; set; }
    public MessageTemplate Template { get; set; } = default!;

    // 🔐 Sender (system / officer)
    public string? SentByUserId { get; set; }

    // 👤 Recipient
    public string? RecipientUserId { get; set; }
    public string? RecipientMobile { get; set; }

    // 📡 Channel info
    public MessageChannel Channel { get; set; }   // Sms / Notification / Both

    // ✉️ SMS snapshot
    public bool SmsSent { get; set; }
    public string? SmsText { get; set; }

    // 🔔 Notification snapshot
    public bool NotificationSent { get; set; }
    public string? NotificationTitle { get; set; }
    public string? NotificationBody { get; set; }

    // ⚠️ Optional failure info
    public string? FailureReason { get; set; }
}

