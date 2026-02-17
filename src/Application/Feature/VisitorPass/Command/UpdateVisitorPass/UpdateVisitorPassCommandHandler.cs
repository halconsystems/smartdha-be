using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.VisitorPass.Command.UpdateVisitorPass;

public class UpdateVisitorPassCommandHandler
    : IRequestHandler<UpdateVisitorPassCommand, UpdateVisitorPassResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartdhaDbContext;
    public UpdateVisitorPassCommandHandler(IApplicationDbContext context, ISmartdhaDbContext smartdhaDbContext)
    {
        _context = context;
        _smartdhaDbContext = smartdhaDbContext;
    }

    public async Task<UpdateVisitorPassResponse> Handle(UpdateVisitorPassCommand request, CancellationToken cancellationToken)
    {
        var entity = await _smartdhaDbContext.VisitorPasses
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new Exception("Visitor Pass Not Found");

        entity.Name = request.Name;
        entity.CNIC = request.CNIC;
        entity.VehicleLicensePlate = request.VehicleLicensePlate;
        entity.VehicleLicenseNo = request.VehicleLicenseNo;
        entity.VisitorPassType = request.VisitorPassType;
        entity.ValidFrom = request.ValidFrom;
        entity.ValidTo = request.ValidTo;

        await _smartdhaDbContext.SaveChangesAsync(cancellationToken);

        return new UpdateVisitorPassResponse
        {
            Message = "Visitor Pass Updated Successfully"
        };
    }
}
