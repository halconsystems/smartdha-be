using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnit.Commands.CreateFacilityUnit;
public record CreateFacilityUnitCommand(
    CreateFacilityUnitDto Dto
) : IRequest<ApiResult<Guid>>;
public class CreateFacilityUnitHandler
    : IRequestHandler<CreateFacilityUnitCommand, ApiResult<Guid>>
{
    private readonly ICBMSApplicationDbContext _db;

    public CreateFacilityUnitHandler(ICBMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<Guid>> Handle(
        CreateFacilityUnitCommand request,
        CancellationToken ct)
    {
        var unit = new Domain.Entities.CBMS.FacilityUnit
        {
            ClubId = request.Dto.ClubId,
            FacilityId = request.Dto.FacilityId,
            Name = request.Dto.Name,
            Code = request.Dto.Code,
            Description=request.Dto.Description,
            UnitType = request.Dto.UnitType
        };

        _db.FacilityUnits.Add(unit);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(unit.Id, "Facility unit created");
    }
}

