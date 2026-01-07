using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class WebhookCallbackLog : BaseAuditableEntity
{
    public string Source { get; set; } = default!;   // e.g. "SmartPay", "Zindigi", "JazzCash"
    public string Endpoint { get; set; } = default!; // called URL path

    public string HttpMethod { get; set; } = default!;
    public string ContentType { get; set; } = default!;

    public string Payload { get; set; } = default!;  // 🔥 RAW JSON / FORM / ANY DATA
    public string? Headers { get; set; }              // serialized headers

    public string? ClientIp { get; set; }
    public bool IsProcessed { get; set; } = false;

    public string? Remarks { get; set; }
}

