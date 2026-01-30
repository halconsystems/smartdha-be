using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubCategories.Command;

public record UpdateClubCategoryCommand(
    Guid Id,
    string Name,
    string Code,
    string DisplayName, 
    string Descriptio
) : IRequest<ApiResult<bool>>;
public class UpdateClubCategoryCommandHandler
    : IRequestHandler<UpdateClubCategoryCommand, ApiResult<bool>>
{
    private readonly IOLMRSApplicationDbContext _db;

    public UpdateClubCategoryCommandHandler(IOLMRSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        UpdateClubCategoryCommand request,
        CancellationToken ct)
    {
        var entity = await _db.Set<Domain.Entities.CBMS.ClubCategory>()
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity == null)
            return ApiResult<bool>.Fail("Club Category not found.");

        var codeExists = await _db.Set<Domain.Entities.CBMS.ClubCategory>()
            .AnyAsync(x => x.Code == request.Code && x.Id != request.Id, ct);

        if (codeExists)
            return ApiResult<bool>.Fail("Club Category code already exists.");

        entity.Name = request.Name.Trim();
        entity.Code = request.Code.Trim().ToUpperInvariant();
        entity.DisplayName = request.DisplayName.Trim();
        entity.Description = request.Descriptio.Trim();

        await _db.SaveChangesAsync(ct);

        return ApiResult<bool>.Ok(true, "Club Category updated successfully.");
    }
}

