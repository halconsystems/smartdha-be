namespace DHAFacilitationAPIs.Application.Feature.Announcement.Queries.GetAllAnnouncements;

using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Announcements.Commands.AddAnnouncement;
using DHAFacilitationAPIs.Application.ViewModels;
using MediatR;

public class GetAllAnnouncementsQuery : IRequest<SuccessResponse<List<AnnouncementDto>>> { }

public class GetAllAnnouncementsQueryHandler : IRequestHandler<GetAllAnnouncementsQuery, SuccessResponse<List<AnnouncementDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllAnnouncementsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<List<AnnouncementDto>>> Handle(GetAllAnnouncementsQuery request, CancellationToken cancellationToken)
    {
        var result = await _context.Announcements
            .Select(a => new AnnouncementDto
            {
                Id = a.Id,
                Name = a.Name,
                Title = a.Title,
                Description = a.Description
            })
            .ToListAsync(cancellationToken);

        return new SuccessResponse<List<AnnouncementDto>>(result);
    }
}

