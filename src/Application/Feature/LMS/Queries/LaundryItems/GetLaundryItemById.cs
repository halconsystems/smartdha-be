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
    private readonly IFileStorageService _fileStorageService;

    public GetLaundryItemByIdQueryHandler(ILaundrySystemDbContext context, IFileStorageService fileStorageService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
    }

    public async Task<List<LaundryItemsDTO>> Handle(GetLaundryItemByIdQuery request, CancellationToken ct)
    {
        var LaundryItems = await _context.LaundryItems
            .Where(x => x.CategoryId == request.CategoryId)
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
            Image = x.ItemImage == null ? null : _fileStorageService.GetPublicUrl(x.ItemImage)
        }).ToList();

        return result;
    }
}





