using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.RoomCategories.Commands.CreateRoomCategory;
public record CreateRoomCategoryCommand(string Name, string? Description)
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
            IsActive = true,
            IsDeleted = false,
            Created = DateTime.UtcNow
        };
        _ctx.RoomCategories.Add(entity);
        await _ctx.SaveChangesAsync(ct);
        return SuccessResponse<string>.FromMessage(entity.Id + "RoomCategory created.");
    }
}

