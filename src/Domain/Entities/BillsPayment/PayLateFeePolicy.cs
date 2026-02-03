using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.Payment;

namespace DHAFacilitationAPIs.Domain.Entities.BillsPayment;
public class PayLateFeePolicy : BaseAuditableEntity
{
    // PMS / CLUB / SCHOOL / DHA_SECURITY
    public string SourceSystem { get; set; } = default!;

    // Default due date offset from bill creation
    public int DueAfterDays { get; set; }

    // Bill expires after this many days (0 = never expire)
    public int ExpireAfterDays { get; set; }
    // Grace period before late fee applies
    public int GraceDays { get; set; }
    // NONE / FIXED / PER_DAY
    public LateFeeType LateFeeType { get; set; }
    // Used if FIXED
    public decimal FixedLateFee { get; set; }
    // Used if PER_DAY
    public decimal PerDayLateFee { get; set; }
}

