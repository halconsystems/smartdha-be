using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.LaggagePass.Command.UpdateLaggagePass;

public class UpdateLaggagePassCommandHandler
    : IRequestHandler<UpdateLaggagePassCommand, UpdateLaggagePassResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartdhaDbContext;

    public UpdateLaggagePassCommandHandler(IApplicationDbContext context, ISmartdhaDbContext smartdhaDbContext)
    {
        _context = context;
        _smartdhaDbContext = smartdhaDbContext;
    }

    public async Task<UpdateLaggagePassResponse> Handle(UpdateLaggagePassCommand request, CancellationToken cancellationToken)
    {
        var entity = await _smartdhaDbContext.LaggagePasses
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new Exception("Laggage Pass Not Found");

        entity.Name = request.Name;
        entity.CNIC = request.CNIC;
        entity.VehicleLicensePlate = request.VehicleLicensePlate;
        entity.VehicleLicenseNo = request.VehicleLicenseNo;
        entity.Description = request.Description;
        entity.ValidTo = request.ValidTo;
        entity.ValidFrom = request.ValidFrom;
        entity.ValidTo = request.ValidTo;

        await _smartdhaDbContext.SaveChangesAsync(cancellationToken);

        return new UpdateLaggagePassResponse
        {
            Message = "Laggage Pass Updated Successfully"
        };
    }
}
