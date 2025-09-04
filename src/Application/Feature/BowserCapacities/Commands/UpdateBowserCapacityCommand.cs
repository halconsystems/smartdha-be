using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.BowserCapacities.Commands;
public class UpdateBowserCapacityCommand : IRequest<SuccessResponse<string>>
{
    public Guid Id { get; set; }
    public decimal Capacity { get; set; }
    public string Unit { get; set; } = default!;
}

public class UpdateBowserCapacityCommandHandler : IRequestHandler<UpdateBowserCapacityCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;
  

    public UpdateBowserCapacityCommandHandler(IOLHApplicationDbContext context)
    {
        _context = context;
     
    }

    public async Task<SuccessResponse<string>> Handle(UpdateBowserCapacityCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.BowserCapacitys
            .FirstOrDefaultAsync(x => x.Id == request.Id && (x.IsDeleted == null || x.IsDeleted == false), cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(OLH_BowserCapacity), request.Id.ToString());
        }
        entity.Capacity = request.Capacity;
        entity.Unit = request.Unit;
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Bowser Capacity Id {request.Id} data successfully updated.");
    }
}


