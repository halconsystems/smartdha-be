using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryServices;

public record CreateAddLaundryServiceCommand(string name, string DisplayName)
    : IRequest<SuccessResponse<string>>;
public class CreateAddLaundryServiceCommandHandler : IRequestHandler<CreateAddLaundryServiceCommand, SuccessResponse<string>>
{
    private readonly ILaundrySystemDbContext _context;

    public CreateAddLaundryServiceCommandHandler(ILaundrySystemDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(CreateAddLaundryServiceCommand command, CancellationToken ct)
    {
        try
        {
            var entity = new Domain.Entities.LaundryService
            {
                Name = command.name,
                DisplayName = command.DisplayName,
                Code = command.DisplayName.Substring(0, command.DisplayName.Length / 2).ToUpper()
            };

            _context.LaundryServices.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Success.Created(entity.Id.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return Success.Created(Guid.NewGuid().ToString());
        }
    }
}


