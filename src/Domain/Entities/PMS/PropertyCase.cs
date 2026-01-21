using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Domain.Entities.PMS;
public class PropertyCase : BaseAuditableEntity
{
    public Guid UserPropertyId { get; set; }
    public UserProperty UserProperty { get; set; } = default!;

    public Guid ProcessId { get; set; }
    public ServiceProcess Process { get; set; } = default!;

    public CaseStatus Status { get; set; } = CaseStatus.Draft;

    // Current workflow location
    public int CurrentStepNo { get; set; } = 0;

    public Guid? CurrentStepId { get; set; }
    public ProcessStep? CurrentStep { get; set; }

    // Case tracking number (human-friendly)
    [Required, MaxLength(30)]
    public string CaseNo { get; set; } = default!;

    // Applicant info (simple; later you can link to AppUser)
    [MaxLength(150)]
    public string? ApplicantName { get; set; }

    [MaxLength(20)]
    public string? ApplicantCnic { get; set; }

    [MaxLength(20)]
    public string? ApplicantMobile { get; set; }

    // Remarks at submission
    [MaxLength(500)]
    public string? ApplicantRemarks { get; set; }
    public Guid? CurrentModuleId { get; set; }
    public Guid DirectorateId { get; set; }
    public Directorate Directorate { get; set; } = default!;
    public string? CurrentAssignedUserId { get; set; } // internal runtime owner
}

