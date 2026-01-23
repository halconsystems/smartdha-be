using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.Services.Queries;

public record GetAllServicesByIdQuery(Guid Id) : IRequest<ServiceDTO>;
public class GetAllServicesByIdQueryHandler : IRequestHandler<GetAllServicesByIdQuery, ServiceDTO>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAllServicesByIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ServiceDTO> Handle(GetAllServicesByIdQuery request, CancellationToken ct)
    {
        var services = await _context.FemServices
           .FirstOrDefaultAsync(x => x.Id == request.Id);


        if (services == null)
            throw new KeyNotFoundException("Service Not Found.");


        var result = new ServiceDTO {
            Id = services.Id,
            Name = services.Name,
            DisplayName = services.DisplayName,
            Code = services.Code,
            IsActive = services.IsActive,
        };

        return result;
    }
}







