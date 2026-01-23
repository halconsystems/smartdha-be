using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.FemigationDT.Command;


public record DeleteFMDTCommand(Guid Id) : IRequest<SuccessResponse<string>>;
public class DeleteFMDTCommandHandler
    : IRequestHandler<DeleteFMDTCommand, SuccessResponse<string>>
{
    private readonly IApplicationDbContext _context;

    public DeleteFMDTCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(DeleteFMDTCommand command, CancellationToken ct)
    {
        var rowsAffected = await _context.FemDTSettings
             .Where(x => x.Id == command.Id)
             .ExecuteDeleteAsync(ct);

        if (rowsAffected == 0)
            throw new KeyNotFoundException("Discount/Tax not found.");

        return Success.Delete(command.Id.ToString());
    }

}






