using System;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Refunds.Dtos;

public class GetRefundRequestDto
{
    public Guid Id { get; set; }
    public Guid ReservationId { get; set; }
    public DateTime RequestedAt { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal AmountRefunded { get; set; }
    public RefundStatus Status { get; set; }
    public string? Notes { get; set; }
}
