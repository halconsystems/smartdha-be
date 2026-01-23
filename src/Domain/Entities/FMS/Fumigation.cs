using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.FMS;

public class Fumigation : BaseAuditableEntity
{
    
    public Guid UserId { get; set; }
    public Guid FemPhaseID {  get; set; }
    public FemPhase? FemPhase { get; set; }

    public Guid FemServiceId   { get; set; }
    public FemService? FemService { get; set; }
    public Guid FemTanker {  get; set; }
    public TankerSize? TankerSize { get; set; }
    [Required]
    public string StreetNo { get; set; } = default!;
    [Required]
    public string PhoneNumber { get; set; } = default!;
    [Required]
    public string Address { get; set; } = default!;
    [Required]
    public DateOnly DateOnly { get; set; }
    [Required]
    public TimeOnly TimeOnly { get; set; }
    public decimal? Taxes { get; set; }
    public decimal? Discount { get; set; }
    public decimal? SubTotal { get; set; }
    public decimal? Total { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public decimal? AmountToCollect { get; set; }
    public decimal? CollectedAmount { get; set; }
    public string? BasketId { get; set; }
    public string? AssigndTo { get; set; }
    public string? Remarks { get; set; }
    public FemStatus FemStatus { get; set; } = FemStatus.Created;
    public DateTime? AcknowledgedAt { get; set; }
    public DateTime? AssginedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    
    [Required, MaxLength(40)] public string CaseNo { get; set; } = default!;

    public Guid ShopId { get; set; }
    public FemgutionShops? FemgutionShops { get; set; }

    public string? DriverRemarksAudioPath { get; set; }

    public ICollection<FumgationMedia> Media { get; set; } = new List<FumgationMedia>();
    public Guid AssignedBy { get; set; }
}
