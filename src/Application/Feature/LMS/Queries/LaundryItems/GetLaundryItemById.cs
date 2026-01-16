using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryItems;

public record GetLaundryItemByIdQuery(Guid CategoryId) : IRequest<List<LaundryItemsDTO>>;
public class GetLaundryItemByIdQueryHandler : IRequestHandler<GetLaundryItemByIdQuery, List<LaundryItemsDTO>>
{
    private readonly ILaundrySystemDbContext _context;

    public GetLaundryItemByIdQueryHandler(ILaundrySystemDbContext context)
    {
        _context = context;
    }

    public async Task<List<LaundryItemsDTO>> Handle(GetLaundryItemByIdQuery request, CancellationToken ct)
    {
        var LaundryItems = await _context.LaundryItems
            .Where(x => x.CategoryId == request.CategoryId)
            .AsNoTracking()
            .ToListAsync(ct);

        var result = LaundryItems.Select(x => new LaundryItemsDTO
        {
            Name = x.Name,
            DisplayName = x.DisplayName,
            Code = x.Code,
            CategoryID = x.CategoryId.ToString(),
            ItemPrice = x.ItemPrice,
        }).ToList();

        return result;
    }
}





