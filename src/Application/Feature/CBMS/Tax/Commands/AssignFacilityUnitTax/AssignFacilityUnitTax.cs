using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.Tax.Commands.AssignFacilityUnitTax;
public record AssignFacilityUnitTaxCommand(
    AssignFacilityUnitTaxDto Dto
) : IRequest<ApiResult<Guid>>;
public class AssignFacilityUnitTaxHandler
    : IRequestHandler<AssignFacilityUnitTaxCommand, ApiResult<Guid>>
{
    private readonly ICBMSApplicationDbContext _db;

    public AssignFacilityUnitTaxHandler(ICBMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<Guid>> Handle(
        AssignFacilityUnitTaxCommand request,
        CancellationToken ct)
    {
        var exists = await _db.FacilityUnitTax.AnyAsync(x =>
            x.FacilityUnitId == request.Dto.FacilityUnitId &&
            x.TaxId == request.Dto.TaxId,
            ct);

        if (exists)
            return ApiResult<Guid>.Fail("Tax already assigned");

        var entity = new FacilityUnitTax
        {
            FacilityUnitId = request.Dto.FacilityUnitId,
            TaxId = request.Dto.TaxId,
            IsActive = true
        };

        _db.FacilityUnitTax.Add(entity);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(entity.Id, "Tax assigned to facility unit");
    }
}

