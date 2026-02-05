using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ServiceDefinition.Commands.CreateServiceDefinition;
public record CreateServiceDefinitionCommand(
    CreateServiceDefinitionDto Dto
) : IRequest<ApiResult<Guid>>;
public class CreateServiceDefinitionCommandHandler
    : IRequestHandler<CreateServiceDefinitionCommand, ApiResult<Guid>>
{
    private readonly ICBMSApplicationDbContext _db;

    public CreateServiceDefinitionCommandHandler(ICBMSApplicationDbContext db)
        => _db = db;

    public async Task<ApiResult<Guid>> Handle(
        CreateServiceDefinitionCommand request,
        CancellationToken ct)
    {
        var exists = await _db.ServiceDefinitions
            .AnyAsync(x =>
                x.Code == request.Dto.Code &&
                x.ClubServiceCategoryId == request.Dto.ClubServiceCategoryId &&
                x.IsDeleted != true,
                ct);

        if (exists)
            return ApiResult<Guid>.Fail("Service with this code already exists.");

        var entity = new Domain.Entities.CBMS.ServiceDefinition
        {
            ClubServiceCategoryId = request.Dto.ClubServiceCategoryId,
            Name = request.Dto.Name,
            Code = request.Dto.Code,
            IsQuantityBased = request.Dto.IsQuantityBased
        };

        _db.ServiceDefinitions.Add(entity);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(entity.Id);
    }
}

