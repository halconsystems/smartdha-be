using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubServices.Queries;


public record ClubServiceProcessDTO(
    Guid Id,
    Guid CategoryId,
    string Name,
    string Code,
    string? Description,

    bool IsFeeAtSubmission,
    bool IsVoucherPossible,
    bool IsFeeRequired,
    bool IsfeeSubmit,
    bool IsInstructionAtStart,
    bool IsButton,
    string? Instruction,
    bool? IsActive,
    bool? IsDelete
);
