using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.BowserCapacityRates.Commands;
public class UpdateBowserCapacityRateCommand : IRequest<SuccessResponse<string>>
{
    public Guid Id { get; set; }
    public Guid BowserCapacityId { get; set; }
    public decimal Rate { get; set; }
   
}

public class UpdateBowserCapacityRateCommandHandler : IRequestHandler<UpdateBowserCapacityRateCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;


    public UpdateBowserCapacityRateCommandHandler(IOLHApplicationDbContext context)
    {
        _context = context;

    }

    public async Task<SuccessResponse<string>> Handle(UpdateBowserCapacityRateCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.BowserCapacityRates
            .FirstOrDefaultAsync(x => x.Id == request.Id && (x.IsDeleted == null || x.IsDeleted == false), cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(OLH_BowserCapacityRate), request.Id.ToString());
        }
        entity.BowserCapacityId = request.BowserCapacityId;
        entity.Rate = request.Rate;
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Bowser Capacity Rate Id {request.Id} data successfully updated.");
    }
}
