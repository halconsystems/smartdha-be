using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class SMSLog : BaseAuditableEntity
{
    public string Status { get; set; } = default!;
    public string? MessageId { get; set; }
    public string? CountrySupported { get; set; }
    public string? CountryCode { get; set; }
    public string? CountryIso { get; set; }
    public string? NetworkName { get; set; }
    public string? ReceiverNumber { get; set; }
    public string? ErrorFilter { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorDescription { get; set; }
    public string? ChargedBalance { get; set; }
    public string? ChargingUnit { get; set; }
    public string? SentMessage { get; set; }
    public bool IsSmsSent { get; set; } // 
}

