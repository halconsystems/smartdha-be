using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.LaggagePass.Command.CreateLaggagePass;

public class CreateLaggagePassCommandHandler
    : IRequestHandler<CreateLaggagePassCommand, CreateLaggagePassResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartdhaDbContext;

    public CreateLaggagePassCommandHandler(IApplicationDbContext context, ISmartdhaDbContext smartdhaDbContext)
    {
        _context = context;
        _smartdhaDbContext = smartdhaDbContext;
    }

    public async Task<CreateLaggagePassResponse> Handle(CreateLaggagePassCommand request, CancellationToken cancellationToken)
    {
        var entity = new Domain.Entities.Smartdha.LaggagePass
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

        await _smartdhaDbContext.LaggagePasses.AddAsync(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new CreateLaggagePassResponse
        {
            Message = "Laggage Pass Created Successfully"
        };
    }
}
