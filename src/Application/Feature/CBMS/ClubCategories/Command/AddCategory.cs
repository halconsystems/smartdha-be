using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubCategories.Command;

public record CreateClubCategoryCommand(Guid ClubId, string Name, string Code) : IRequest<ApiResult<Guid>>;

public class CreateClubCategoryCommandHandler : IRequestHandler<CreateClubCategoryCommand, ApiResult<Guid>>
{
    private readonly IOLMRSApplicationDbContext _db;
    public CreateClubCategoryCommandHandler(IOLMRSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<Guid>> Handle(CreateClubCategoryCommand request, CancellationToken ct)
    {
        var exists = await _db.Set<Domain.Entities.CBMS.ClubCategories>()
            .AnyAsync(x => x.Code == request.Code && x.ClubId == request.ClubId, ct);

        if (exists) return ApiResult<Guid>.Fail("Club Category code already exists.");

        var entity = new Domain.Entities.CBMS.ClubCategories
        {
            ClubId = request.ClubId,
            Name = request.Name.Trim(),
            Code = request.Code.Trim().ToUpperInvariant()
        };

        _db.Set<Domain.Entities.CBMS.ClubCategories>().Add(entity);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(entity.Id, "Club Category created.");
    }
}

