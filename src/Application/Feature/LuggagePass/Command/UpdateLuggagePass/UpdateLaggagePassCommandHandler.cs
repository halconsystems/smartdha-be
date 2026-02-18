using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.LuggagePass.Command.UpdateLuggagePass;

public class UpdateLuggagePassCommandHandler
    : IRequestHandler<UpdateLuggagePassCommand, Result<UpdateLuggagePassResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartdhaDbContext;

    public UpdateLuggagePassCommandHandler(IApplicationDbContext context, ISmartdhaDbContext smartdhaDbContext)
    {
        _context = context;
        _smartdhaDbContext = smartdhaDbContext;
    }

    public async Task<Result<UpdateLuggagePassResponse>> Handle(UpdateLuggagePassCommand request, CancellationToken cancellationToken)
    {
        var entity = await _smartdhaDbContext.LuggagePasses
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new Exception("Luggage Pass Not Found");

        entity.Name = request.Name;
        entity.CNIC = request.CNIC;
        entity.VehicleLicensePlate = request.VehicleLicensePlate;
        entity.VehicleLicenseNo = request.VehicleLicenseNo;
        entity.Description = request.Description;
        entity.ValidTo = request.ValidTo;
        entity.ValidFrom = request.ValidFrom;
        entity.ValidTo = request.ValidTo;

        await _smartdhaDbContext.SaveChangesAsync(cancellationToken);

        return Result<UpdateLuggagePassResponse>.Success(
    new UpdateLuggagePassResponse
    {
        Message = "Luggage Pass Updated Successfully"
    });

    }
}
