using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Common.Dto;
public class CreatePaymentBillRequest
{
    public string SourceSystem { get; set; } = default!; // PMS / CLUB / SCHOOL

    public Guid SourceVoucherId { get; set; }
    public string SourceVoucherNo { get; set; } = default!;

    public string Title { get; set; } = default!;

    public string EntityType { get; set; } = default!; // PROPERTY / CLUB
    public Guid? EntityId { get; set; }
    public string? EntityName { get; set; }

    public string UserId { get; set; } = default!;

    public decimal TotalAmount { get; set; }

    public DateTime? DueDate { get; set; }
}

