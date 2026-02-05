using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ServiceDefinition.Commands.DeleteServiceDefinition;
public record DeleteServiceDefinitionCommand(Guid Id)
    : IRequest<ApiResult<bool>>;
public class DeleteServiceDefinitionCommandHandler
    : IRequestHandler<DeleteServiceDefinitionCommand, ApiResult<bool>>
{
    private readonly ICBMSApplicationDbContext _db;

    public DeleteServiceDefinitionCommandHandler(ICBMSApplicationDbContext db)
        => _db = db;

    public async Task<ApiResult<bool>> Handle(
        DeleteServiceDefinitionCommand request,
        CancellationToken ct)
    {
        var entity = await _db.ServiceDefinitions
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.IsDeleted != true, ct);

        if (entity == null)
            return ApiResult<bool>.Fail("Service not found.");

        entity.IsDeleted = true;
        entity.IsActive = false; 
        await _db.SaveChangesAsync(ct);

        return ApiResult<bool>.Ok(true);
    }
}

