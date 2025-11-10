using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.BowserMake.Commands;
public record AddVehicleStatusCommand(VehicleStatus Status, string Remarks) : IRequest<SuccessResponse<string>>;

public class AddVehicleStatusHandler : IRequestHandler<AddVehicleStatusCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;
    public AddVehicleStatusHandler(IOLHApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<string>> Handle(AddVehicleStatusCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.VehicleStatuses
            .AnyAsync(x => x.Status == request.Status && x.IsDeleted != true, cancellationToken);

        if (exists)
            throw new ArgumentException($"Vehicle Status '{request.Status}' already exists.");

        var entity = new OLH_VehicleStatus { Status = request.Status, Remarks = request.Remarks };
        _context.VehicleStatuses.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Vehicle Status '{request.Status}' successfully added.");
    }
}
