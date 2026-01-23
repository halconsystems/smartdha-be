using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryCategory;

namespace DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryItems;


public record GetAllLaundryItemsQuery : IRequest<List<LaundryItemsDTO>>;
public class GetAllLaundryItemsQueryHandler : IRequestHandler<GetAllLaundryItemsQuery, List<LaundryItemsDTO>>
{
    private readonly ILaundrySystemDbContext _context;
    private readonly IFileStorageService _fileStorageService;

    public GetAllLaundryItemsQueryHandler(ILaundrySystemDbContext context, IFileStorageService fileStorageService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
    }

    public async Task<List<LaundryItemsDTO>> Handle(GetAllLaundryItemsQuery request, CancellationToken ct)
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
            Image = x.ItemImage == null ? null : _fileStorageService.GetPublicUrl(x.ItemImage)

        }).ToList();

        return result;
    }
}




