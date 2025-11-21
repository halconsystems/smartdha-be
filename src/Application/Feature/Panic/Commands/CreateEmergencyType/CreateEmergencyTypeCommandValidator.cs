using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.CreateEmergencyType;
public class CreateEmergencyTypeCommandValidator
    : AbstractValidator<CreateEmergencyTypeCommand>
{
    public CreateEmergencyTypeCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.HelplineNumber).MaximumLength(30);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

