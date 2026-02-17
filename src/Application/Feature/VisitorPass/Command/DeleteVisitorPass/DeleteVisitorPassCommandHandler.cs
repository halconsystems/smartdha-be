using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Vehicles.Commands.DeleteVehicle;

namespace DHAFacilitationAPIs.Application.Feature.VisitorPass.Command.DeleteVisitorPass;

public class DeleteVisitorPassCommandHandler
    : IRequestHandler<DeleteVisitorPassCommand, DeleteVisitorPassResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartdhaDbContext;
    public DeleteVisitorPassCommandHandler(IApplicationDbContext context, ISmartdhaDbContext smartdhaDbContext)
    {
        _context = context;
        _smartdhaDbContext = smartdhaDbContext;
    }

    public async Task<DeleteVisitorPassResponse> Handle(DeleteVisitorPassCommand request, CancellationToken cancellationToken)
    {
        var entity = await _smartdhaDbContext.VisitorPasses
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            return new DeleteVisitorPassResponse
            {
                Success = false,
                Message = "Vehicle Not Found"
            };
        }

        entity.IsDeleted = true;
        entity.IsActive = false;

        await _smartdhaDbContext.SaveChangesAsync(cancellationToken);

        return new DeleteVisitorPassResponse
        {
            Success = true,
            Message = "Vehicle Deleted Successfully"
        };
    }
}
