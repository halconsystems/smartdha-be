using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityService.Commands.CreateFacilityService;
public record CreateFacilityServiceCommand(
    CreateFacilityServiceDto Dto
) : IRequest<ApiResult<Guid>>;
public class CreateFacilityServiceHandler
    : IRequestHandler<CreateFacilityServiceCommand, ApiResult<Guid>>
{
    private readonly ICBMSApplicationDbContext _db;

    public CreateFacilityServiceHandler(ICBMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<Guid>> Handle(
        CreateFacilityServiceCommand request,
        CancellationToken ct)
    {
        var entity = new Domain.Entities.CBMS.FacilityService
        {
            FacilityId = request.Dto.FacilityId,
            Name = request.Dto.Name,
            Code = request.Dto.Code,
            Price = request.Dto.Price,
            IsComplimentary = request.Dto.IsComplimentary,
            IsQuantityBased = request.Dto.IsQuantityBased
        };

        _db.FacilityServices.Add(entity);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(entity.Id, "Service added");
    }
}

