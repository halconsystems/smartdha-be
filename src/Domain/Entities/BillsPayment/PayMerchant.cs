using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.BillsPayment;
public class PayMerchant : BaseAuditableEntity
{
    // e.g. DHA_CLUB_MAIN, SUNSET_CLUB, SCHOOL_PHASE8
    public string Code { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string Gateway { get; set; } = "PAYFAST";
    public string MerchantId { get; set; } = default!;
    public string SmartPayMerchantId { get; set; } = default!;
    // Encrypted value (never plain)
    public string SecureKeyEncrypted { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
