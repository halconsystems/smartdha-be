using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.Dropdown.Queries.GetDropdown;

// Request
public sealed record GetDropdownQuery<TEntity>()
    : IRequest<List<DropdownDto>> where TEntity : class;

// Handler
public sealed class GetDropdownQueryHandler<TEntity>
    : IRequestHandler<GetDropdownQuery<TEntity>, List<DropdownDto>>
    where TEntity : class
{
    private readonly IOLMRSApplicationDbContext _context;
    public GetDropdownQueryHandler(IOLMRSApplicationDbContext context) => _context = context;

    public async Task<List<DropdownDto>> Handle(GetDropdownQuery<TEntity> request, CancellationToken ct)
        => await _context.Set<TEntity>()
            .AsNoTracking()
            .Select(e => new DropdownDto
            {
                Id = EF.Property<Guid>(e, "Id"),
                Name = EF.Property<string>(e, "Name")
            })
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
}
