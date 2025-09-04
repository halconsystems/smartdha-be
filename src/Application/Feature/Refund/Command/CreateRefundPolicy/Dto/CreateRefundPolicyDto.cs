using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Refund.Command.CreateRefundPolicy.Dto;
public class CreateRefundPolicyDto
{
    [Required]
    public Guid ClubId { get; set; }

    [Required]
    public int HoursBeforeCheckIn { get; set; }   // rule trigger

    [Required]
    [Range(0, 100)]
    public decimal RefundPercent { get; set; }   // % refund

    public bool RefundDeposit { get; set; } = false;

    [Required]
    public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow;
    public DateTime EffectiveTo { get; set; }
}

