using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Dropdown.Queries.GetDropdown;

// Request
public sealed record GetDropdownQuery<TEntity>(
    ClubType? ClubType = null,
    ServiceType? ServiceType = null)
    : IRequest<List<DropdownDto>> where TEntity : class;

// Handler
public sealed class GetDropdownQueryHandler<TEntity>
    : IRequestHandler<GetDropdownQuery<TEntity>, List<DropdownDto>>
    where TEntity : class
{
    private readonly IOLMRSApplicationDbContext _context;
    public GetDropdownQueryHandler(IOLMRSApplicationDbContext context) => _context = context;

    public async Task<List<DropdownDto>> Handle(GetDropdownQuery<TEntity> request, CancellationToken ct)
    {
        //await _context.Set<TEntity>()
        // .AsNoTracking()
        // .Where(e => EF.Property<bool?>(e, "IsActive") == true
        //          && (EF.Property<bool?>(e, "IsDeleted") == false || EF.Property<bool?>(e, "IsDeleted") == null))
        // .Select(e => new DropdownDto
        // {
        //     Id = EF.Property<Guid>(e, "Id"),
        //     Name = EF.Property<string>(e, "Name")
        // })
        // .OrderBy(x => x.Name)
        // .ToListAsync(ct);

        IQueryable<TEntity> query = _context.Set<TEntity>().AsNoTracking();

        // Apply ClubType filter if entity has it
        if (request.ClubType.HasValue && typeof(TEntity).GetProperty("ClubType") != null)
        {
            query = query.Where(e => EF.Property<ClubType>(e, "ClubType") == request.ClubType.Value);
        }

        // Apply ServiceType filter if entity has it
        if (request.ServiceType.HasValue && typeof(TEntity).GetProperty("ServiceType") != null)
        {
            query = query.Where(e => EF.Property<ServiceType>(e, "ServiceType") == request.ServiceType.Value);
        }

        // Apply common flags if available
        if (typeof(TEntity).GetProperty("IsActive") != null)
            query = query.Where(e => EF.Property<bool>(e, "IsActive") == true);

        if (typeof(TEntity).GetProperty("IsDeleted") != null)
            query = query.Where(e => EF.Property<bool?>(e, "IsDeleted") == false
                                  || EF.Property<bool?>(e, "IsDeleted") == null);

        return await query
            .Select(e => new DropdownDto
            {
                Id = EF.Property<Guid>(e, "Id"),
                Name = EF.Property<string>(e, "Name")
            })
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
    }
}
