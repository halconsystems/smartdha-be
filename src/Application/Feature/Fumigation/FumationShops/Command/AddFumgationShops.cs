using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.FumationShops.Command;

public record CreateFMShopsCommand(string name, string DisplayName, string Address, float Latitude, float Longitude, string OwnerName, string OwnerEmail, string OwnerPhone, string ShopPhoneNumber)
    : IRequest<SuccessResponse<string>>;
public class CreateShopsCommandHandler : IRequestHandler<CreateFMShopsCommand, SuccessResponse<string>>
{
    private readonly IApplicationDbContext _context;

    public CreateShopsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(CreateFMShopsCommand command, CancellationToken ct)
    {
        try
        {
            var entity = new Domain.Entities.FMS.FemgutionShops
            {
                Name = command.name,
                DisplayName = command.DisplayName,
                Code = command.DisplayName.Substring(0, command.DisplayName.Length / 2).ToUpper(),
                Address = command.Address,
                Latitude = command.Latitude,
                Longitude = command.Longitude,
                OwnerName = command.OwnerName,
                OwnerEmail = command.OwnerEmail,
                OwnerPhone = command.OwnerPhone,
                ShopPhoneNumber = command.ShopPhoneNumber
            };

            _context.FemgutionShops.Add(entity);
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




