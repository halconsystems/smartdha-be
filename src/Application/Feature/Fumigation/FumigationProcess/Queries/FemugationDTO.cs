using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities.FMS;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.FumigationProcess.Queries;

public class FemugationDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid FemPhaseID { get; set; }
    public FemPhase? FemPhase { get; set; }
    public Guid FemServiceId { get; set; }
    public FemService? FemService { get; set; }
    public Guid FemTanker { get; set; }
    public TankerSize? TankerSize { get; set; }
    public string? StreetNo { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public DateOnly DateOnly { get; set; }
    public TimeOnly TimeOnly { get; set; }
    public string? Taxes { get; set; }
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
    public DateTime? CreatedAt { get; set; }
    public string? CaseNo { get; set; }
    public string? ShopName { get; set; }
    public string? TankSize { get; set; }
    public Guid ShopId { get; set; }
    public FemgutionShops? FemgutionShops { get; set; }
    public string? DriverRemarksAudioPath { get; set; }


    public ICollection<FumgationMedia> Media { get; set; } = new List<FumgationMedia>();
    public Guid AssignedBy { get; set; }
    public bool? IsActive {  get; set; }
    public Dictionary<string, string>? Images {  get; set; }
}
