using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.Clubs.Commands.CreateClub;
public record CreateClubCommand(string Name, string? Description, string? Location, string? ContactNumber)
    : IRequest<SuccessResponse<string>>;

public class CreateClubCommandHandler : IRequestHandler<CreateClubCommand, SuccessResponse<string>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    public CreateClubCommandHandler(IOLMRSApplicationDbContext ctx) => _ctx = ctx;

    public async Task<SuccessResponse<string>> Handle(CreateClubCommand request, CancellationToken ct)
    {
        var entity = new Club
        {
            Name = request.Name,
            Description = request.Description,
            Location = request.Location,
            ContactNumber = request.ContactNumber
        };
        _ctx.Clubs.Add(entity);
        await _ctx.SaveChangesAsync(ct);

        return Success.Created(entity.Id.ToString());
    }
}

