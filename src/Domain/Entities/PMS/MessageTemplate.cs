using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.PMS;
public class MessageTemplate : BaseAuditableEntity
{
    public string Code { get; set; } = default!;     // CASE_REJECTED, CASE_APPROVED
    public string Title { get; set; } = default!;    // For notifications
    public string Body { get; set; } = default!;     // Text with {{Placeholders}}

    public bool AllowSms { get; set; }
    public bool AllowNotification { get; set; }

    public int SmsMaxLength { get; set; } = 160;
    // 🔑 MODULE SCOPING
    public Guid ModuleId { get; set; }
}

