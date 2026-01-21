using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSServiceProcess.Queries;
public record ServiceProcessDto(
    Guid Id,
    Guid CategoryId,
    string Name,
    string Code,

    bool IsFeeAtSubmission,
    bool IsVoucherPossible,
    bool IsFeeRequired,
    bool IsNadraVerificationRequired,
    bool IsfeeSubmit,
    bool IsInstructionAtStart,
    string? Instruction
);

