using System;
using System.ComponentModel.DataAnnotations.Schema;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Refunds.Dtos;

public class GetRefundRequestDto
{
    public Guid Id { get; set; }
    public Guid ReservationId { get; set; }
    public DateTime RequestedAt { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal AmountPaid { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal RefundableAmount { get; set; }
    public RefundStatus Status { get; set; }
    public string? Notes { get; set; }
}
