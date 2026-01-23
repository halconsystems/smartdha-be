using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Fumigation.Tanker.Queries;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.FemigationDT.Queries;

public record GetAllFMDiscountQuery : IRequest<List<FemigationDTdTO>>;
public class GetAllFMDiscountQueryHandler : IRequestHandler<GetAllFMDiscountQuery, List<FemigationDTdTO>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAllFMDiscountQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<FemigationDTdTO>> Handle(GetAllFMDiscountQuery request, CancellationToken ct)
    {
        var tanker = await _context.FemDTSettings
            .AsNoTracking()
            .ToListAsync(ct);


        if (tanker == null)
            throw new KeyNotFoundException("Phases Not Found.");


        var result = tanker.Select(x => new FemigationDTdTO
        {
            Id = x.Id,
            Name = x.Name,
            DisplayName = x.DisplayName,
            Code = x.Code,
            Value = x.Value,
            ValueType = x.ValueType,
            IsDiscount = x.IsDiscount,
            IsActive = x.IsActive,
        }).ToList();

        return result;
    }
}






