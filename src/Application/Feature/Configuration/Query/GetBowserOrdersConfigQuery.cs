using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.Configuration.Query;
public record GetBowserOrdersConfigQuery : IRequest<DHAFacilitationAPIs.Domain.Entities.Configuration?>;

public class GetBowserOrdersConfigQueryHandler
    : IRequestHandler<GetBowserOrdersConfigQuery, DHAFacilitationAPIs.Domain.Entities.Configuration?>
{
    private readonly IApplicationDbContext _context;

    public GetBowserOrdersConfigQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DHAFacilitationAPIs.Domain.Entities.Configuration?> Handle(GetBowserOrdersConfigQuery request, CancellationToken cancellationToken)
    {
        return await _context.Configurations
            .FirstOrDefaultAsync(x => x.Key == "BowserOrders", cancellationToken);
    }
}
