using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.Tanker.Command;


public record DeleteTankerCommand(Guid Id) : IRequest<SuccessResponse<string>>;
public class DeleteTankerCommandHandler
    : IRequestHandler<DeleteTankerCommand, SuccessResponse<string>>
{
    private readonly IApplicationDbContext _context;

    public DeleteTankerCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(DeleteTankerCommand command, CancellationToken ct)
    {
        var rowsAffected = await _context.TankerSizes
             .Where(x => x.Id == command.Id)
             .ExecuteDeleteAsync(ct);

        if (rowsAffected == 0)
            throw new KeyNotFoundException("Size not found.");

        return Success.Delete(command.Id.ToString());
    }

}






