using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryPackaging;

public record CreateLaundryPackagingCommand(string name, string DisplayName)
    : IRequest<SuccessResponse<string>>;
public class CreateLaundryPackagingCommandHandler : IRequestHandler<CreateLaundryPackagingCommand, SuccessResponse<string>>
{
    private readonly ILaundrySystemDbContext _context;

    public CreateLaundryPackagingCommandHandler(ILaundrySystemDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(CreateLaundryPackagingCommand command, CancellationToken ct)
    {
        try
        {
            var entity = new Domain.Entities.LaundryPackaging
            {
                Name = command.name,
                DisplayName = command.DisplayName,
                Code = command.DisplayName.Substring(0, command.DisplayName.Length / 2).ToUpper()
            };

            _context.LaundryPackagings.Add(entity);
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



