using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class FirebaseApiLog : BaseAuditableEntity
{
    public string DeviceToken { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Body { get; set; } = default!;
    public string PayloadJson { get; set; } = default!;
    public string? ResponseJson { get; set; }
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
}

