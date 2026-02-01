using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Queries.ClubFacilities;
using DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnit.Commands.CreateFacilityUnit;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnit.Commands.UpdateFacilityUnit;

public record UpdateFacilityUnitCommand(
    Guid Id,
    CreateFacilityUnitDto Dto
) : IRequest<ApiResult<Guid>>;
public class UpdateFacilityUnitCommandHandler
    : IRequestHandler<UpdateFacilityUnitCommand, ApiResult<Guid>>
{
    private readonly ICBMSApplicationDbContext _db;

    public UpdateFacilityUnitCommandHandler(ICBMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<Guid>> Handle(
        UpdateFacilityUnitCommand request,
        CancellationToken ct)
    {
        var existFacilityUnit = await _db.FacilityUnits
            .FirstOrDefaultAsync(x => x.Id == request.Id,ct);

        if (existFacilityUnit == null) return ApiResult<Guid>.Fail("Facility Unit not found.");


        existFacilityUnit.ClubId = request.Dto.ClubId;
        existFacilityUnit.FacilityId = request.Dto.FacilityId;
        existFacilityUnit.Name = request.Dto.Name;
        existFacilityUnit.Code = request.Dto.Code;
        existFacilityUnit.UnitType = request.Dto.UnitType;

        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(existFacilityUnit.Id, "Facility unit Updated");
    }
}

