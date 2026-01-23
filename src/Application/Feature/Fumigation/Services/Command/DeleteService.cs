using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.Services.Command;

public record DeleteServiceCommand(Guid Id) : IRequest<SuccessResponse<string>>;
public class DeleteServiceCommandHandler
    : IRequestHandler<DeleteServiceCommand, SuccessResponse<string>>
{
    private readonly IApplicationDbContext _context;

    public DeleteServiceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(DeleteServiceCommand command, CancellationToken ct)
    {
        var rowsAffected = await _context.FemServices
             .Where(x => x.Id == command.Id)
             .ExecuteDeleteAsync(ct);

        if (rowsAffected == 0)
            throw new KeyNotFoundException("Service not found.");

        return Success.Delete(command.Id.ToString());
    }

}






