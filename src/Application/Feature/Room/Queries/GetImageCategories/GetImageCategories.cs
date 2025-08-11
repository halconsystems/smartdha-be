using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Room.Queries.GetImageCategories;
public record GetImageCategoriesQuery() : IRequest<SuccessResponse<List<ImageCategoryDto>>>;

public record ImageCategoryDto(int Value, string Name);

public class GetImageCategoriesQueryHandler
    : IRequestHandler<GetImageCategoriesQuery, SuccessResponse<List<ImageCategoryDto>>>
{
    public Task<SuccessResponse<List<ImageCategoryDto>>> Handle(GetImageCategoriesQuery request, CancellationToken ct)
    {
        var list = Enum.GetValues(typeof(ImageCategory))
            .Cast<ImageCategory>()
            .Select(e => new ImageCategoryDto((int)e, e.ToString()))
            .OrderBy(x => x.Value)
            .ToList();

        return Task.FromResult(new SuccessResponse<List<ImageCategoryDto>>(list, "OK"));
    }
}

