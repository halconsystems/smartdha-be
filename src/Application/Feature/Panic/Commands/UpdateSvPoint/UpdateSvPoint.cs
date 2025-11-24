using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.UpdateSvPoint;
public record UpdateSvPointCommand(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    double Latitude,
    double Longitude
) : IRequest<Guid>;
public class UpdateSvPointCommandHandler : IRequestHandler<UpdateSvPointCommand,Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IGeocodingService _geocodingService;

    public UpdateSvPointCommandHandler(IApplicationDbContext context, IGeocodingService geocodingService)
    {
        _context = context;
        _geocodingService = geocodingService;
    }

    public async Task<Guid> Handle(UpdateSvPointCommand request, CancellationToken ct)
    {
        var entity = await _context.SvPoints
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new NotFoundException("SvPoint", request.Id.ToString());

        bool locationChanged = entity.Latitude != request.Latitude ||
                               entity.Longitude != request.Longitude;

        entity.Code = request.Code;
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Latitude = request.Latitude;
        entity.Longitude = request.Longitude;
        if (locationChanged)
        {
            entity.Latitude = request.Latitude;
            entity.Longitude = request.Longitude;

            entity.Address = await _geocodingService.GetAddressFromLatLngAsync(request.Latitude, request.Longitude, ct);
        }

        await _context.SaveChangesAsync(ct);

        return entity.Id;
    }
}

