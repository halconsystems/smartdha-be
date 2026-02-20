using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.Smartdha;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.VisitorPass.Command.CreateVisitorPass;

public class CreateVisitorPassCommandHandler
    : IRequestHandler<CreateVisitorPassCommand, Result<CreateVisitorPassResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartdhaDbContext;


    public CreateVisitorPassCommandHandler(IApplicationDbContext context, ISmartdhaDbContext smartdhaDbContext)
    {
        _context = context;
        _smartdhaDbContext = smartdhaDbContext;
    }

    public async Task<Result<CreateVisitorPassResponse>> Handle(CreateVisitorPassCommand request, CancellationToken cancellationToken)
    {
        var entity = new Domain.Entities.Smartdha.VisitorPass
        {
            Name = request.Name,
            CNIC = request.CNIC,
            VehicleLicensePlate = request.VehicleLicensePlate,
            VehicleLicenseNo = request.VehicleLicenseNo,
            VisitorPassType = (VisitorPassType)request.VisitorPassType,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo,
            QRCode = Guid.NewGuid().ToString()
        };

        await _smartdhaDbContext.VisitorPasses.AddAsync(entity, cancellationToken);
        try
        {

            await _smartdhaDbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.ToString());
        }
        return Result<CreateVisitorPassResponse>.Success(
    new CreateVisitorPassResponse
     {
         Id = entity.Id,
         Message = "Visitor Pass Created Successfully"
     });

    }
}
