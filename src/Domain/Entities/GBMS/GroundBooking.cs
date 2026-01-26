using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Domain.Entities.GBMS;

public class GroundBooking :BaseAuditableEntity
{
    [Required]
    public Guid UserId {  get; set; }
    public Guid GroundId    { get; set; }
    public Grounds? Grounds { get; set; }
    public string? BookingDescription { get; set; }
    public DateTime BookingDate { get; set; }
    public DateTime CheckinDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public string? TotalAmount { get; set; } 
    public string? SubTotal { get; set; } 
    public string? Taxes { get; set; }
    public string? Discount {  get; set; }
    public string? CollectedAmount { get; set; }
    public string? AmountToCollect {  get; set; }
    public string? BacketId { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public bool IsConfirm {  get; set; }
    public string BookingCode { get; set; } = default!;

    public DateOnly BookingDateOnly { get; set; }
}
