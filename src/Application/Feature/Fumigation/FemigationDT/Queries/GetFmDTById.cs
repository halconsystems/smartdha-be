using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Fumigation.Tanker.Queries;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.FemigationDT.Queries;


public record GetFmDTByIdQuery(Guid Id) : IRequest<FemigationDTdTO>;
public class GetFmDTByIdQueryHandler : IRequestHandler<GetFmDTByIdQuery, FemigationDTdTO>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetFmDTByIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<FemigationDTdTO> Handle(GetFmDTByIdQuery request, CancellationToken ct)
    {
        var tanker = await _context.FemDTSettings
           .FirstOrDefaultAsync(x => x.Id == request.Id);


        if (tanker == null)
            throw new KeyNotFoundException("FemDTSettings Not Found.");


        var result = new FemigationDTdTO
        {
            Id = tanker.Id,
            Name = tanker.Name,
            DisplayName = tanker.DisplayName,
            Code = tanker.Code,
            Value = tanker.Value,
            ValueType = tanker.ValueType,
            IsDiscount = tanker.IsDiscount,
        };

        return result;
    }
}









