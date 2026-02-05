using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnitService.Commands.AddFacilityUnitService;
using DHAFacilitationAPIs.Domain.Entities.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnitService.Commands.UpdateFacilityUnitService;

public record UpdateFacilityUnitServiceCommand(
    Guid Id,
    AddFacilityUnitServiceDto Dto
) : IRequest<ApiResult<Guid>>;
public class UpdateFacilityUnitServiceCommandHandler
    : IRequestHandler<UpdateFacilityUnitServiceCommand, ApiResult<Guid>>
{
    private readonly ICBMSApplicationDbContext _db;

    public UpdateFacilityUnitServiceCommandHandler(ICBMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<Guid>> Handle(
        UpdateFacilityUnitServiceCommand request,
        CancellationToken ct)
    {
        var existServiceUnit = await _db.FacilityUnitServices
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (existServiceUnit == null)
            return ApiResult<Guid>.Fail("Facility Unit Service Not Found");

        existServiceUnit.FacilityUnitId = request.Dto.FacilityUnitId;
        existServiceUnit.IsEnabled = request.Dto.IsEnabled;
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(existServiceUnit.Id, "Unit service added");
    }
}

