using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.ClubServices.Commands.CreateService;
public record CreateServiceCommand(string Name, string? Description, ServiceType ServiceType)
    : IRequest<SuccessResponse<string>>;

public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, SuccessResponse<string>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    public CreateServiceCommandHandler(IOLMRSApplicationDbContext ctx) => _ctx = ctx;

    public async Task<SuccessResponse<string>> Handle(CreateServiceCommand request, CancellationToken ct)
    {
        var entity = new Services
        {
            Name = request.Name,
            Description = request.Description,
            ServiceType = request.ServiceType
        };
        _ctx.Services.Add(entity);
        await _ctx.SaveChangesAsync(ct);
        return Success.Created(entity.Id.ToString());
    }
}
