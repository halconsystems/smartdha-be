using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryServices;


public record DeleteLaundryServiceCommand(Guid Id) : IRequest<SuccessResponse<string>>;
public class DeleteLaundryServiceCommandHandler
    : IRequestHandler<DeleteLaundryServiceCommand, SuccessResponse<string>>
{
    private readonly ILaundrySystemDbContext _context;

    public DeleteLaundryServiceCommandHandler(ILaundrySystemDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(DeleteLaundryServiceCommand command, CancellationToken ct)
    {
        var rowsAffected = await _context.LaundryServices
             .Where(x => x.Id == command.Id)
             .ExecuteDeleteAsync(ct);

        if (rowsAffected == 0)
            throw new KeyNotFoundException("Laundry Category not found.");

        return Success.Delete(command.Id.ToString());
    }

}






