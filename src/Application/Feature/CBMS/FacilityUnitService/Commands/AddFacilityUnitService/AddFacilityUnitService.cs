using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnitService.Commands.AddFacilityUnitService;
public record AddFacilityUnitServiceCommand(
    AddFacilityUnitServiceDto Dto
) : IRequest<ApiResult<Guid>>;
public class AddFacilityUnitServiceHandler
    : IRequestHandler<AddFacilityUnitServiceCommand, ApiResult<Guid>>
{
    private readonly ICBMSApplicationDbContext _db;

    public AddFacilityUnitServiceHandler(ICBMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<Guid>> Handle(
        AddFacilityUnitServiceCommand request,
        CancellationToken ct)
    {
        var entity = new Domain.Entities.CBMS.FacilityUnitService
        {
            FacilityUnitId = request.Dto.FacilityUnitId,
            IsEnabled=request.Dto.IsEnabled,
        };

        _db.FacilityUnitServices.Add(entity);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(entity.Id, "Unit service added");
    }
}

