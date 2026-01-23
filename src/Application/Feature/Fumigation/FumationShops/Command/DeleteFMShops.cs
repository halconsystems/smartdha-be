using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.FumationShops.Command;


public record DeleteFMShopsCommand(Guid Id) : IRequest<SuccessResponse<string>>;
public class DeleteFMShopsCommandHandler
    : IRequestHandler<DeleteFMShopsCommand, SuccessResponse<string>>
{
    private readonly IApplicationDbContext _context;

    public DeleteFMShopsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(DeleteFMShopsCommand command, CancellationToken ct)
    {
        var rowsAffected = await _context.FemgutionShops
             .Where(x => x.Id == command.Id)
             .ExecuteDeleteAsync(ct);

        if (rowsAffected == 0)
            throw new KeyNotFoundException("Shop not found.");

        return Success.Delete(command.Id.ToString());
    }

}






