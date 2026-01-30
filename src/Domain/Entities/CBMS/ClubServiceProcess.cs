using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;

public class ClubServiceProcess : BaseAuditableEntity
{

    // FK -> Category
    public Guid CategoryId { get; set; }
    public ClubCategories Category { get; set; } = default!;

    // Example: "Solar Panel Installation"
    [Required, MaxLength(150)]
    public string Name { get; set; } = default!;

    [Required, MaxLength(30)]
    public string Code { get; set; } = default!;
    public string? ImageURL { get; set; }

    public string? Description { get; set; }
    public FoodType? FoodType { get; set; }
    public string? Price { get; set; }
    public bool? IsAvailable { get; set; }
    public bool? IsPriceVisible { get; set; }
    public bool? Action { get; set; }
    public string? ActionName { get; set; }
    public string? ActionType { get; set; }

    // If user must pay immediately on submit
    public bool IsFeeRequired { get; set; } = false;       // fee exists for this process
    public bool IsFeeAtSubmission { get; set; } = false;   // if true → fee required before submit

    // If finance voucher can be generated at some step
    public bool IsVoucherPossible { get; set; } = false;
    public bool IsfeeSubmit { get; set; } = false;
    public bool IsInstructionAtStart { get; set; } = false;
    public bool IsButton { get; set; } = false;
    public string? Instruction { get; set; } = default!;

}
