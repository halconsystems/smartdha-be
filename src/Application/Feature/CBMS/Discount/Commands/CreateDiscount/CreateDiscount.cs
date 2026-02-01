using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.Discount.Commands.CreateDiscount;
public record CreateDiscountCommand(
    CreateDiscountDto Dto
) : IRequest<ApiResult<Guid>>;
public class CreateDiscountHandler
    : IRequestHandler<CreateDiscountCommand, ApiResult<Guid>>
{
    private readonly ICBMSApplicationDbContext _db;

    public CreateDiscountHandler(ICBMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<Guid>> Handle(
        CreateDiscountCommand request,
        CancellationToken ct)
    {
        var discount = new Domain.Entities.CBMS.Discount
        {
            Name = request.Dto.Name,
            Code = request.Dto.Code,
            Type = request.Dto.Type,
            Value = request.Dto.Value,
            ValidFrom = request.Dto.ValidFrom,
            ValidTo = request.Dto.ValidTo,
            MinOrderAmount = request.Dto.MinOrderAmount,
            IsActive = true
        };

        _db.Discounts.Add(discount);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(discount.Id, "Discount created");
    }
}

