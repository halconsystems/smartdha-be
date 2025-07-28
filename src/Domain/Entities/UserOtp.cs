using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class UserOtp : BaseAuditableEntity
{
    public Guid UserId { get; set; }
    public string CNIC { get; set; } = default!;
    public string MobileNo { get; set; } = default!;
    public string OtpCode { get; set; } = default!;
    public string SentMessage { get; set; } = default!;
    public bool IsVerified { get; set; } = false;
    public DateTime ExpiresAt { get; set; }
}
