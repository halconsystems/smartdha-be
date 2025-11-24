using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.CreateSvPoint;
public record CreateSvPointCommand(
    string Code,
    string Name,
    string? Description,
    double Latitude,
    double Longitude
) : IRequest<Guid>;

public class CreateSvPointCommandHandler
    : IRequestHandler<CreateSvPointCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IGeocodingService _geocodingService;

    public CreateSvPointCommandHandler(IApplicationDbContext context, IGeocodingService geocodingService)
    {
        _context = context;
        _geocodingService = geocodingService;
    }

    public async Task<Guid> Handle(CreateSvPointCommand request, CancellationToken ct)
    {
        var entity = new SvPoint
        {
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Address = await _geocodingService.GetAddressFromLatLngAsync(request.Latitude, request.Longitude, ct),
        };

        _context.SvPoints.Add(entity);
        await _context.SaveChangesAsync(ct);

        return entity.Id;
    }
}

