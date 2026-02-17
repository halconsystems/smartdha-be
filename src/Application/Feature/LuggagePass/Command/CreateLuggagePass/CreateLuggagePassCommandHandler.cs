using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.LuggagePass.Command.CreateLuggagePass;

public class CreateLuggagePassCommandHandler
    : IRequestHandler<CreateLuggagePassCommand, CreateLuggagePassResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartdhaDbContext;

    public CreateLuggagePassCommandHandler(IApplicationDbContext context, ISmartdhaDbContext smartdhaDbContext)
    {
        _context = context;
        _smartdhaDbContext = smartdhaDbContext;
    }

    public async Task<CreateLuggagePassResponse> Handle(CreateLuggagePassCommand request, CancellationToken cancellationToken)
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

        await _smartdhaDbContext.LuggagePasses.AddAsync(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new CreateLuggagePassResponse
        {
            Message = "Luggage Pass Created Successfully"
        };
    }
}
