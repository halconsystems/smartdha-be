using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.FumationShops.Queries;

public record GetFMShopsByIdQuery(Guid Id) : IRequest<FMShopsDTO>;
public class GetFMShopsByIdQueryHandler : IRequestHandler<GetFMShopsByIdQuery, FMShopsDTO>
{
    private readonly IApplicationDbContext _context;

    public GetFMShopsByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<FMShopsDTO> Handle(GetFMShopsByIdQuery request, CancellationToken ct)
    {
        var FemgutionShops = await _context.FemgutionShops
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id,ct);

        if (FemgutionShops == null) throw new KeyNotFoundException("Shop not found.");

        var result = new FMShopsDTO
        {
            Id = FemgutionShops.Id,
            Name = FemgutionShops.Name,
            DisplayName = FemgutionShops.DisplayName,
            Code = FemgutionShops.Code,
            Address = FemgutionShops.Address,
            Longitude = FemgutionShops.Longitude,
            Latitude = FemgutionShops.Latitude,
            OwnerName = FemgutionShops.OwnerName,
            OwnerEmail = FemgutionShops.OwnerEmail,
            OwnerPhone = FemgutionShops.OwnerPhone,
            ShopPhoneNumber = FemgutionShops.ShopPhoneNumber,
            IsActive = FemgutionShops.IsActive
        };

        return result;
    }
}




