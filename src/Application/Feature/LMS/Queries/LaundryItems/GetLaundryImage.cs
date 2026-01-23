using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryItems;

internal class GetLaundryImage
{
}
public record GetLaundryImageQuery(Guid Id) : IRequest<List<LaundryItemsDTO>>;
public class GetLaundryImageQueryHandler : IRequestHandler<GetLaundryImageQuery, List<LaundryItemsDTO>>
{
    private readonly ILaundrySystemDbContext _context;

    public GetLaundryImageQueryHandler(ILaundrySystemDbContext context)
    {
        _context = context;
    }

    public async Task<List<LaundryItemsDTO>> Handle(GetLaundryImageQuery request, CancellationToken ct)
    {
        var LaundryItems = await _context.LaundryItems
            .AsNoTracking()
            .ToListAsync(ct);


        var result = LaundryItems.Select(x => new LaundryItemsDTO
        {
            Id = x.Id,
            Name = x.Name,
            DisplayName = x.DisplayName,
            Code = x.Code,
            CategoryID = x.CategoryId.ToString(),
            ItemPrice = x.ItemPrice,
            Image = x.ItemImage
        }).ToList();

        return result;
    }
}




