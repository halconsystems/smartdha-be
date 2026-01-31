using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Command;

public record UpdateFacilityCommand(Guid Id,string Name, string Code, string DisplayName, string Description, Guid CategoryId, string? descrption, FoodType FoodType, string? Price, bool? IsAvailable, bool? IsPriceVisible, bool? Action, string? ActionName, string? ActionType) : IRequest<ApiResult<bool>>;
public class UpdateFacilityCommandHandler
    : IRequestHandler<UpdateFacilityCommand, ApiResult<bool>>
{
    private readonly IOLMRSApplicationDbContext _db;

    public UpdateFacilityCommandHandler(IOLMRSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        UpdateFacilityCommand request,
        CancellationToken ct)
    {
        var entity = await _db.Set<Domain.Entities.CBMS.Facility>()
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity == null)
            return ApiResult<bool>.Fail("Facility not found.");

        var codeExists = await _db.Set<Domain.Entities.CBMS.Facility>()
            .AnyAsync(x => x.Code == request.Code && x.Id != request.Id, ct);

        if (codeExists)
            return ApiResult<bool>.Fail("Club Category code already exists.");

        entity.Name = request.Name.Trim();
        entity.Code = request.Code.Trim().ToUpperInvariant();
        entity.DisplayName = request.DisplayName.Trim();
        entity.Description = request.Description.Trim();
        entity.ClubCategoryId = request.CategoryId;
        entity.FoodType = request.FoodType;
        //entity.Price = request.Price;
        //entity.IsAvailable = request.IsAvailable;
        //entity.IsPriceVisible = request.IsPriceVisible;
        //entity.Action = request.Action;
        //entity.ActionName = request.ActionName;
        //entity.ActionType = request.ActionType;

        await _db.SaveChangesAsync(ct);

        return ApiResult<bool>.Ok(true, "Facility updated successfully.");
    }
}


