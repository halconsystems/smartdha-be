using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.BowserCapacityRates.Commands;

public class AddBowserCapacityRatesCommand : IRequest<SuccessResponse<string>>
{
    public Guid BowserCapacityId { get; set; }
    public decimal Rate { get; set; } = default!;

}

public class AddBowserCapacityRatesCommandHandler : IRequestHandler<AddBowserCapacityRatesCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;

    public AddBowserCapacityRatesCommandHandler(IOLHApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(AddBowserCapacityRatesCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.BowserCapacityRates
            .AnyAsync(c => c.BowserCapacityId == request.BowserCapacityId, cancellationToken);

        if (exists)
        {
            throw new ArgumentException($"Bowser Capacity {request.BowserCapacityId} already exists.");
        }

        var entity = new OLH_BowserCapacityRate
        {
            BowserCapacityId = request.BowserCapacityId,
            Rate = request.Rate,
          
        };

        _context.BowserCapacityRates.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Bowser Capacity {request.BowserCapacityId} successfully added.");
    }
}
