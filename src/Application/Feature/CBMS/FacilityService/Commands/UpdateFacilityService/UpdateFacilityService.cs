using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.CBMS.FacilityService.Commands.CreateFacilityService;
using DHAFacilitationAPIs.Domain.Entities.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityService.Commands.UpdateFacilityService;

public record UpdateFacilityServiceCommand(
    Guid Id,
    CreateFacilityServiceDto Dto
) : IRequest<ApiResult<bool>>;
public class UpdateFacilityServiceCommandHandler
    : IRequestHandler<UpdateFacilityServiceCommand, ApiResult<bool>>
{
    private readonly ICBMSApplicationDbContext _db;

    public UpdateFacilityServiceCommandHandler(ICBMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        UpdateFacilityServiceCommand request,
        CancellationToken ct)
    {
        var existService = await _db.FacilityServices.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if(existService == null) return ApiResult<bool>.Fail("Service not found.");

        existService.FacilityId = request.Dto.FacilityId;
        existService.Name = request.Dto.Name;
        existService.Code = request.Dto.Code;
        existService.Price = request.Dto.Price;
        existService.IsComplimentary = request.Dto.IsComplimentary;
        existService.IsQuantityBased = request.Dto.IsQuantityBased;

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {

            throw new Exception(ex.Message);
        }

        return ApiResult<bool>.Ok(true, "Service Updated");
    }
}

