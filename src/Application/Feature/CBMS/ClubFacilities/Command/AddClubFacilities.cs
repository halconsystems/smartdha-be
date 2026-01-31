using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Command;

public record AddCategoryFacilitiesCommand(string Name, string Code, string DisplayName, string Description,Guid CategoryId,string? descrption,FoodType FoodType,string? Price, bool? IsAvailable, bool? IsPriceVisible, bool? Action,string? ActionName, string? ActionType) : IRequest<ApiResult<Guid>>;

public class AddCategoryFacilitiesCommandHandler : IRequestHandler<AddCategoryFacilitiesCommand, ApiResult<Guid>>
{
    private readonly IOLMRSApplicationDbContext _db;
    public AddCategoryFacilitiesCommandHandler(IOLMRSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<Guid>> Handle(AddCategoryFacilitiesCommand request, CancellationToken ct)
    {
        var exists = await _db.Set<Facility>()
            .AnyAsync(x => x.Code == request.Code, ct);

        if (exists) return ApiResult<Guid>.Fail("Facility code already exists.");

        var entity = new Facility
        {
            Name = request.Name.Trim(),
            Code = request.Code.Trim().ToUpperInvariant(),
            DisplayName = request.DisplayName.Trim(),
            Description = request.Description.Trim(),
            ClubCategoryId = request.CategoryId,
            FoodType = request.FoodType,
            //Price = request.Price,
            //IsAvailable = request.IsAvailable,
            //IsPriceVisible = request.IsPriceVisible,
            //Action = request.Action,
            //ActionName = request.ActionName,
            //ActionType = request.ActionType
        };

        _db.Set<Facility>().Add(entity);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(entity.Id, "Club Facility created.");
    }
}


