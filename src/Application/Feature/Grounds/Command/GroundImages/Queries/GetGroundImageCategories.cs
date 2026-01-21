using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Grounds.Command.GroundImages.Queries;

public record GetGroundImageCategoriesQuery() : IRequest<SuccessResponse<List<ImageCategoryDto>>>;

public record ImageCategoryDto(int Value, string Name);

public class GetGroundImageCategoriesHandler
    : IRequestHandler<GetGroundImageCategoriesQuery, SuccessResponse<List<ImageCategoryDto>>>
{
    public Task<SuccessResponse<List<ImageCategoryDto>>> Handle(GetGroundImageCategoriesQuery request, CancellationToken ct)
    {
        var list = Enum.GetValues(typeof(ImageCategory))
            .Cast<ImageCategory>()
            .Select(e => new ImageCategoryDto((int)e, e.ToString()))
            .OrderBy(x => x.Value)
            .ToList();

        return Task.FromResult(new SuccessResponse<List<ImageCategoryDto>>(list, "OK"));
    }
}

