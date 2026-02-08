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

public record CreateClubCategoryCommand(string Name, string Code,string DisplayName,string Description,int? OrderNo) : IRequest<ApiResult<Guid>>;

public class CreateClubCategoryCommandHandler : IRequestHandler<CreateClubCategoryCommand, ApiResult<Guid>>
{
    private readonly ICBMSApplicationDbContext _db;
    public CreateClubCategoryCommandHandler(ICBMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<Guid>> Handle(CreateClubCategoryCommand request, CancellationToken ct)
    {
        var exists = await _db.Set<ClubServiceCategory>()
            .AnyAsync(x => x.Code == request.Code, ct);

        if (exists) return ApiResult<Guid>.Fail("Club Category code already exists.");

        var entity = new ClubServiceCategory
        {
            Name = request.Name.Trim(),
            Code = request.Code.Trim().ToUpperInvariant(),
            DisplayName= request.DisplayName.Trim(),
            Description = request.Description.Trim(),
            OrderNo = request.OrderNo ?? 1,
        };

        _db.Set<ClubServiceCategory>().Add(entity);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(entity.Id, "Club Category created.");
    }
}

