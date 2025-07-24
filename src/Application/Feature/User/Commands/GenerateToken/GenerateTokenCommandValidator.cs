using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.User.Commands.GenerateToken;
public class GenerateTokenCommandValidator : AbstractValidator<GenerateTokenCommand>
{
    public GenerateTokenCommandValidator()
    {
        RuleFor(v => v.Email)
        .NotEmpty()
        .MaximumLength(200);

        RuleFor(v => v.Password)
        .NotEmpty()
        .MinimumLength(8)
        .MaximumLength(30);
    }
}
