using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.Discount.Commands.AssignFacilityUnitDiscount;
public record AssignFacilityUnitDiscountCommand(
    AssignFacilityUnitDiscountDto Dto
) : IRequest<ApiResult<Guid>>;
public class AssignFacilityUnitDiscountHandler
    : IRequestHandler<AssignFacilityUnitDiscountCommand, ApiResult<Guid>>
{
    private readonly ICBMSApplicationDbContext _db;

    public AssignFacilityUnitDiscountHandler(ICBMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<Guid>> Handle(
        AssignFacilityUnitDiscountCommand request,
        CancellationToken ct)
    {
        var exists = await _db.FacilityUnitDiscounts.AnyAsync(x =>
            x.FacilityUnitId == request.Dto.FacilityUnitId &&
            x.DiscountId == request.Dto.DiscountId,
            ct);

        if (exists)
            return ApiResult<Guid>.Fail("Discount already assigned");

        var entity = new FacilityUnitDiscount
        {
            FacilityUnitId = request.Dto.FacilityUnitId,
            DiscountId = request.Dto.DiscountId,
            IsActive = true
        };

        _db.FacilityUnitDiscounts.Add(entity);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(entity.Id, "Discount assigned to facility unit");
    }
}

