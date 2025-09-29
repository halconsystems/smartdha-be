using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.RoomCategories.Commands.CreateRoomCategory;
public record CreateRoomCategoryCommand(string Name, string? Description, ClubType ClubType)
    : IRequest<SuccessResponse<string>>;

public class CreateRoomCategoryCommandHandler : IRequestHandler<CreateRoomCategoryCommand, SuccessResponse<string>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    public CreateRoomCategoryCommandHandler(IOLMRSApplicationDbContext ctx) => _ctx = ctx;

    public async Task<SuccessResponse<string>> Handle(CreateRoomCategoryCommand request, CancellationToken ct)
    {
        var entity = new Domain.Entities.RoomCategory
        {
            Name = request.Name,
            Description = request.Description,
            ClubType = request.ClubType,
            IsActive = true,
            IsDeleted = false,
            Created = DateTime.UtcNow
        };
        _ctx.RoomCategories.Add(entity);
        await _ctx.SaveChangesAsync(ct);
        return Success.Created(entity.Id.ToString());
    }
}

