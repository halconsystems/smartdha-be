using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.BowserCapacities.Commands;
public class DeleteBowserCapacityRateCommand : IRequest<SuccessResponse<string>>
{
    public Guid Id { get; set; }
}

public class DeleteBowserCapacityRateCommandHandler : IRequestHandler<DeleteBowserCapacityRateCommand, SuccessResponse<string>>
{
    private readonly IOLHApplicationDbContext _context;
   

    public DeleteBowserCapacityRateCommandHandler(IOLHApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
       
    }

    public async Task<SuccessResponse<string>> Handle(DeleteBowserCapacityRateCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.BowserCapacityRates
            .FirstOrDefaultAsync(x => x.Id == request.Id && (x.IsDeleted == null || x.IsDeleted == false), cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(OLH_BowserCapacityRate), request.Id.ToString());
        }

        entity.IsDeleted = true;
  

        await _context.SaveChangesAsync(cancellationToken);
        return new SuccessResponse<string>($"Bowser Capacity Rate Id {request.Id} marked as deleted.");
    }
}
