using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.Tax.Commands.CreateTax;
public record CreateTaxCommand(
    CreateTaxDto Dto
) : IRequest<ApiResult<Guid>>;
public class CreateTaxHandler
    : IRequestHandler<CreateTaxCommand, ApiResult<Guid>>
{
    private readonly ICBMSApplicationDbContext _db;

    public CreateTaxHandler(ICBMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<Guid>> Handle(
        CreateTaxCommand request,
        CancellationToken ct)
    {
        var tax = new Domain.Entities.CBMS.Tax
        {
            Name = request.Dto.Name,
            Code = request.Dto.Code,
            Type = request.Dto.Type,
            Value = request.Dto.Value,
            IsActive = true
        };

        _db.Taxes.Add(tax);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(tax.Id, "Tax created");
    }
}

