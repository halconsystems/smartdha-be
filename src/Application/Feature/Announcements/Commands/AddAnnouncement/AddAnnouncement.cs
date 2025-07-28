using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.Announcements.Commands.AddAnnouncement;
public class AddAnnouncementCommand : IRequest<SuccessResponse<string>>
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}
public class AddAnnouncementCommandHandler : IRequestHandler<AddAnnouncementCommand, SuccessResponse<string>>
{
    private readonly IApplicationDbContext _context;

    public AddAnnouncementCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(AddAnnouncementCommand request, CancellationToken cancellationToken)
    {
        var announcement = new DHAFacilitationAPIs.Domain.Entities.Announcement
        {
            Name = request.Name,
            Title = request.Title,
            Description = request.Description
        };


        await _context.Announcements.AddAsync(announcement, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return SuccessResponse<string>.FromMessage("Announcement created successfully.");
    }
}


