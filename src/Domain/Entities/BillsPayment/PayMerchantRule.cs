using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.BillsPayment;
public class PayMerchantRule : BaseAuditableEntity
{
    // PMS | CLUB | SCHOOL
    public string SourceSystem { get; set; } = default!;
    // CLUB | SCHOOL | PROPERTY
    public string EntityType { get; set; } = default!;
    // ClubId / SchoolId (nullable = fallback rule)
    public Guid? EntityId { get; set; }
    // Optional: Gym, Banquet, Membership
    public string? CategoryCode { get; set; }
    public string MerchantCode { get; set; } = default!;
    public int Priority { get; set; } = 0;
}

