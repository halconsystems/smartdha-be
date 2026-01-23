using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryItems;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.FumationShops.Queries;


public record GetFMShopsQuery : IRequest<List<FMShopsDTO>>;
public class GetFMShopsQueryHandler : IRequestHandler<GetFMShopsQuery, List<FMShopsDTO>>
{
    private readonly IApplicationDbContext _context;

    public GetFMShopsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<FMShopsDTO>> Handle(GetFMShopsQuery request, CancellationToken ct)
    {
        var LaundryItems = await _context.FemgutionShops
            .AsNoTracking()
            .ToListAsync(ct);

        var result = LaundryItems.Select(x => new FMShopsDTO
        {
            Id = x.Id,
            Name = x.Name,
            DisplayName = x.DisplayName,
            Code = x.Code,
            Address = x.Address,
            Longitude = x.Longitude,
            Latitude = x.Latitude,
            OwnerName = x.OwnerName,
            OwnerEmail = x.OwnerEmail,
            OwnerPhone = x.OwnerPhone,
            ShopPhoneNumber = x.ShopPhoneNumber,
            IsActive = x.IsActive
        }).ToList();

        return result;
    }
}




