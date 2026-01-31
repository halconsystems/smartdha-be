using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Commands.CreateFacility;
public record CreateFacilityCommand(
    string Name,
    string DisplayName,
    string Code,
    Guid ClubCategoryId,
    string? ImageURL,
    string? Description,
    FoodType? FoodType
) : IRequest<ApiResult<Guid>>;
public class CreateFacilityCommandHandler : IRequestHandler<CreateFacilityCommand, ApiResult<Guid>>
{
    private readonly ICBMSApplicationDbContext _ctx;

    public CreateFacilityCommandHandler(ICBMSApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<ApiResult<Guid>> Handle(
        CreateFacilityCommand request,
        CancellationToken ct)
    {
        var facility = new Facility
        {
            Name = request.Name,
            DisplayName = request.DisplayName,
            Code = request.Code,
            ClubCategoryId = request.ClubCategoryId,
            ImageURL = request.ImageURL,
            Description = request.Description,
            FoodType = request.FoodType
        };

        _ctx.Facilities.Add(facility);
        await _ctx.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(facility.Id);
    }
}


