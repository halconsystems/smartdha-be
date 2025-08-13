using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Announcements.Commands.UpdateAnnouncement;
public class UpdateAnnouncementCommand : IRequest<SuccessResponse<string>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateAnnouncementCommandHandler
    : IRequestHandler<UpdateAnnouncementCommand, SuccessResponse<string>>
{
    private readonly IApplicationDbContext _context;

    public UpdateAnnouncementCommandHandler(IApplicationDbContext context)
        => _context = context;

    public async Task<SuccessResponse<string>> Handle(UpdateAnnouncementCommand request, CancellationToken ct)
    {
        var entity = await _context.Announcements
            .FirstOrDefaultAsync(x => x.Id == request.Id && (x.IsDeleted == false || x.IsDeleted == null), ct);

        if (entity is null)
            throw new KeyNotFoundException("Announcement not found.");

        entity.Name = request.Name;
        entity.Title = request.Title;
        entity.Description = request.Description;
        entity.LastModified = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return SuccessResponse<string>.FromMessage("Announcement updated successfully.", "Updated");
    }
}

