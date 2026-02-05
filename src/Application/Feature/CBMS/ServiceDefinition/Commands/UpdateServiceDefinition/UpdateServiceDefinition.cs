using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ServiceDefinition.Commands.UpdateServiceDefinition;
public record UpdateServiceDefinitionCommand(
    UpdateServiceDefinitionDto Dto
) : IRequest<ApiResult<Guid>>;
public class UpdateServiceDefinitionCommandHandler
    : IRequestHandler<UpdateServiceDefinitionCommand, ApiResult<Guid>>
{
    private readonly ICBMSApplicationDbContext _db;

    public UpdateServiceDefinitionCommandHandler(ICBMSApplicationDbContext db)
        => _db = db;

    public async Task<ApiResult<Guid>> Handle(
        UpdateServiceDefinitionCommand request,
        CancellationToken ct)
    {
        var entity = await _db.ServiceDefinitions
            .FirstOrDefaultAsync(x => x.Id == request.Dto.Id && x.IsDeleted != true, ct);

        if (entity == null)
            return ApiResult<Guid>.Fail("Service not found.");

        entity.ClubServiceCategoryId = request.Dto.ClubServiceCategoryId;
        entity.Name = request.Dto.Name;
        entity.Code = request.Dto.Code;
        entity.IsQuantityBased = request.Dto.IsQuantityBased;
        entity.IsActive = request.Dto.IsActive;

        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(entity.Id);
    }
}

