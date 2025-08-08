using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.ResidenceType.Commands.CreateResidenceType;
public record CreateResidenceTypeCommand(string Name, string? Description)
    : IRequest<SuccessResponse<string>>;

public class CreateResidenceTypeCommandHandler : IRequestHandler<CreateResidenceTypeCommand, SuccessResponse<string>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    public CreateResidenceTypeCommandHandler(IOLMRSApplicationDbContext ctx) => _ctx = ctx;

    public async Task<SuccessResponse<string>> Handle(CreateResidenceTypeCommand request, CancellationToken ct)
    {
        var entity = new DHAFacilitationAPIs.Domain.Entities.ResidenceType
        {
            Name = request.Name,
            Description = request.Description,
            IsActive = true,
            IsDeleted = false,
            Created = DateTime.Now
        };
        _ctx.ResidenceTypes.Add(entity);
        await _ctx.SaveChangesAsync(ct);
        return SuccessResponse<string>.FromMessage(entity.Id.ToString(), "ResidenceType created.");
    }
}
