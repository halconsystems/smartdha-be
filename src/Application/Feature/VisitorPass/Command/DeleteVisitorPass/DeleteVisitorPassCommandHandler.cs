using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.Vehicles.Commands.DeleteVehicle;

namespace DHAFacilitationAPIs.Application.Feature.VisitorPass.Command.DeleteVisitorPass;

public class DeleteVisitorPassCommandHandler
    : IRequestHandler<DeleteVisitorPassCommand, Result<DeleteVisitorPassResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartdhaDbContext;
    public DeleteVisitorPassCommandHandler(IApplicationDbContext context, ISmartdhaDbContext smartdhaDbContext)
    {
        _context = context;
        _smartdhaDbContext = smartdhaDbContext;
    }

    public async Task<Result<DeleteVisitorPassResponse>> Handle(DeleteVisitorPassCommand request, CancellationToken cancellationToken)
    {
        var entity = await _smartdhaDbContext.VisitorPasses
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            return Result<DeleteVisitorPassResponse>.Failure(
     new[] { "Error deleting visitor pass" });

        }

        entity.IsDeleted = true;
        entity.IsActive = false;

        await _smartdhaDbContext.SaveChangesAsync(cancellationToken);

        return Result<DeleteVisitorPassResponse>.Success(
    new DeleteVisitorPassResponse
    {
        Success = true,
        Message = "Visitor Pass Deleted Successfully"
    });

    }
}
