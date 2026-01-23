using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.Phases.Command;

public record DeleteFemPhaseCommand(Guid Id) : IRequest<SuccessResponse<string>>;
public class DeletePhaseCommandHandler
    : IRequestHandler<DeleteFemPhaseCommand, SuccessResponse<string>>
{
    private readonly IApplicationDbContext _context;

    public DeletePhaseCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(DeleteFemPhaseCommand command, CancellationToken ct)
    {
        var rowsAffected = await _context.FemPhases
             .Where(x => x.Id == command.Id)
             .ExecuteDeleteAsync(ct);

        if (rowsAffected == 0)
            throw new KeyNotFoundException("phase not found.");

        return Success.Delete(command.Id.ToString());
    }

}






