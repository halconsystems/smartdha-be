using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.ShopVehicles.Command;


public record CreateShopVehicleCommand(Guid ShopID, string Name, string RegistrationNo,ShopVehicleType ShopVehicleType,string? MapIconKey, Guid? SvPointId,ShopVehicleStatus Status)
    : IRequest<SuccessResponse<string>>;
public class CreateShopVehicleCommandHandler : IRequestHandler<CreateShopVehicleCommand, SuccessResponse<string>>
{
    private readonly ILaundrySystemDbContext _context;

    public CreateShopVehicleCommandHandler(ILaundrySystemDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(CreateShopVehicleCommand command, CancellationToken ct)
    {
        try
        {
            var entity = new Domain.Entities.LMS.ShopVehicles
            {
                ShopId = command.ShopID,
                Name = command.Name,
                RegistrationNo = command.RegistrationNo,
                VehicleType = command.ShopVehicleType,
                MapIconKey = command.MapIconKey,
                Status = command.Status,
            };

            _context.ShopVehicles.Add(entity);
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




