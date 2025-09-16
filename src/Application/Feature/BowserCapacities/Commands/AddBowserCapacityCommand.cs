using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.BowserCapacities.Commands;

public class AddBowserCapacityCommand : IRequest<SuccessResponse<string>>
{
    public decimal Capacity { get; set; }
    public string Unit { get; set; } = default!;

}

public class AddBowserCapacityCommandHandler : IRequestHandler<AddBowserCapacityCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;

    public AddBowserCapacityCommandHandler(IOLHApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(AddBowserCapacityCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.BowserCapacitys
            .AnyAsync(c => c.Capacity == request.Capacity, cancellationToken);

        if (exists)
        {
            throw new ArgumentException($"Bowser Capacity {request.Capacity} already exists.");
        }

        var entity = new OLH_BowserCapacity
        {
            Capacity = request.Capacity,
            Unit = request.Unit,
         // CreatedBy=request.UserID,

         
        };

        _context.BowserCapacitys.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Bowser Capacity {request.Capacity} successfully added.");
    }
}
