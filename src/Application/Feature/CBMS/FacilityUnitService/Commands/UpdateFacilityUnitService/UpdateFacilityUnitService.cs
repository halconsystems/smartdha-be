using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnitService.Commands.AddFacilityUnitService;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnitService.Commands.UpdateFacilityUnitService;

public record UpdateFacilityUnitServiceCommand(
    Guid Id,
    decimal Price,
    bool IsComplimentary,
    bool IsEnabled
) : IRequest<ApiResult<bool>>;

public class UpdateFacilityUnitServiceHandler
    : IRequestHandler<UpdateFacilityUnitServiceCommand, ApiResult<bool>>
{
    private readonly ICBMSApplicationDbContext _db;

    public UpdateFacilityUnitServiceHandler(ICBMSApplicationDbContext db)
        => _db = db;

    public async Task<ApiResult<bool>> Handle(
        UpdateFacilityUnitServiceCommand request,
        CancellationToken ct)
    {
        var entity = await _db.FacilityUnitServices
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.IsDeleted != true, ct);

        if (entity == null)
            throw new NotFoundException("Facility Unit Service not found");

        entity.Price = request.Price;
        entity.IsComplimentary = request.IsComplimentary;
        entity.IsEnabled = request.IsEnabled;

        await _db.SaveChangesAsync(ct);
        return ApiResult<bool>.Ok(true);
    }
}


