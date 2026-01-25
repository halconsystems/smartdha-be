using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;

public class ClubPrerequisiteOptions : BaseAuditableEntity
{
    public Guid PrerequisiteDefinitionId { get; set; }
    public PrerequisiteDefinition PrerequisiteDefinition { get; set; } = default!;

    // Display text shown in UI (e.g. "5 KW", "10 KW")
    [Required, MaxLength(200)]
    public string Label { get; set; } = default!;

    // Value sent to backend (e.g. "5", "10", "A")
    [Required, MaxLength(100)]
    public string Value { get; set; } = default!;

    // Optional ordering
    public int SortOrder { get; set; } = 0;

    // If you want to disable an option without deleting
    public bool IsDisabled { get; set; } = false;
}
