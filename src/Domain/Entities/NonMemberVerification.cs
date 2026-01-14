using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DHAFacilitationAPIs.Domain.Entities;
public class NonMemberVerification : BaseAuditableEntity
{
    [Required]
    public string UserId { get; set; } = default!;

    [Required]
    public string CNIC { get; set; } = default!;

    public string Remarks { get; set; } = default!;
    public VerificationStatus Status { get; set; } = VerificationStatus.Pending;
    public DateTime? ApprovedAt { get; set; } = default!;
    public string? ApprovedBy { get; set; } = default!;// Admin UserId

    [Required]
    public HomeType HomeType { get; set; } = default!;
    [Required]
    public PropertyType PropertyType { get; set; } = default!;

    [Required]
    public Domain.Enums.ResidenceTypes ResidenceType { get; set; } = default!;

    [Required]
    public ResidenceStatus ResidenceStatus { get; set; } = default!;

    public string? PhaseNo { get; set; }
    public string? LaneNo { get; set; }
    public string? PlotNo { get; set; }
    public string? Floors { get; set; }

    public string? TenantOwnerName { get; set; }
    public string? TenantOwnerContact { get; set; }
    public DateTime? TenantOwnerAgreemenrStartDate { get; set; }
    public DateTime? TenantOwnerAgreemenrEndDate { get; set; }

}
