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
             .FirstOrDefaultAsync(ct);

        if (rowsAffected == null)
            throw new KeyNotFoundException("phase not found.");

        rowsAffected.IsDeleted = true;
        rowsAffected.IsActive = false;

        return Success.Delete(command.Id.ToString());
    }

}






