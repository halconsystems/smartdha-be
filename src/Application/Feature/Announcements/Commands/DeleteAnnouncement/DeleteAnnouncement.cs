using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Announcements.Commands.DeleteAnnouncement;
public class DeleteAnnouncementCommand : IRequest<SuccessResponse<string>>
{
    public Guid Id { get; set; }
    public bool HardDelete { get; set; } = false;
}

public class DeleteAnnouncementCommandHandler
    : IRequestHandler<DeleteAnnouncementCommand, SuccessResponse<string>>
{
    private readonly IApplicationDbContext _context;

    public DeleteAnnouncementCommandHandler(IApplicationDbContext context)
        => _context = context;

    public async Task<SuccessResponse<string>> Handle(DeleteAnnouncementCommand request, CancellationToken ct)
    {
        var entity = await _context.Announcements
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity is null)
            throw new KeyNotFoundException("Announcement not found.");

        if (request.HardDelete)
        {
            _context.Announcements.Remove(entity);
        }
        else
        {
            entity.IsDeleted = true;
            entity.IsActive = false;
            entity.LastModified = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(ct);

        return SuccessResponse<string>.FromMessage(
            request.HardDelete ? "Announcement deleted permanently." : "Announcement deleted successfully.",
            "Deleted"
        );
    }
}
