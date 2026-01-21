using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Enums.PMS;
public class CaseFeeReceipt : BaseAuditableEntity
{
    public Guid CaseId { get; set; }
    public Guid FeeDefinitionId { get; set; }   // FK to CaseFee
    public Guid? FeeOptionId { get; set; }   // FK to CaseFee
    public decimal? PaidAmount { get; set; }
    public decimal? TotalPayable {  get; set; }
    public decimal? NADRA_FEE { get; set; } = 0;
    public decimal SERVICE_FEE { get; set; }= 0;

    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Online;
    public string? GatewayTransactionId { get; set; } // IPG ref
    public string? BankRefNo { get; set; }

    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
}
