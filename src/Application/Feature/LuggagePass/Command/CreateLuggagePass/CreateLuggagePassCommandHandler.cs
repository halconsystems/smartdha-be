using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.Vehicles.Commands.CreateVehicleCommandHandler;

namespace DHAFacilitationAPIs.Application.Feature.LuggagePass.Command.CreateLuggagePass;

public class CreateLuggagePassCommandHandler
    : IRequestHandler<CreateLuggagePassCommand, Result<CreateLuggagePassResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartdhaDbContext;

    public CreateLuggagePassCommandHandler(IApplicationDbContext context, ISmartdhaDbContext smartdhaDbContext)
    {
        _context = context;
        _smartdhaDbContext = smartdhaDbContext;
    }

    public async Task<Result<CreateLuggagePassResponse>> Handle(CreateLuggagePassCommand request, CancellationToken cancellationToken)
    {
        var entity = new Domain.Entities.Smartdha.LuggagePass
        {
            Name = request.Name,
            CNIC = request.CNIC,
            VehicleLicensePlate = request.VehicleLicensePlate,
            VehicleLicenseNo = request.VehicleLicenseNo,
            Description = request.Description,
            ValidFrom = request.ValidityDate,
            ValidTo = request.ValidityDate,
            QRCode = Guid.NewGuid().ToString()
        };

        await _smartdhaDbContext.LuggagePasses.AddAsync(entity, cancellationToken);
        await _smartdhaDbContext.SaveChangesAsync(cancellationToken); 

        return Result<CreateLuggagePassResponse>.Success(
            new CreateLuggagePassResponse
            {
                Id = entity.Id,  
                Message = "Luggage Pass Created Successfully"
            });
    }
}
