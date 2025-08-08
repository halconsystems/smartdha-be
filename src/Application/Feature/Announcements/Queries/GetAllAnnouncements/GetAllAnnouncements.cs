namespace DHAFacilitationAPIs.Application.Feature.Announcement.Queries.GetAllAnnouncements;

using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Announcements.Commands.AddAnnouncement;
using DHAFacilitationAPIs.Application.ViewModels;
using MediatR;

public class GetAllAnnouncementsQuery : IRequest<SuccessResponse<List<AnnouncementDto>>> { }

public class GetAllAnnouncementsQueryHandler : IRequestHandler<GetAllAnnouncementsQuery, SuccessResponse<List<AnnouncementDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllAnnouncementsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<SuccessResponse<List<AnnouncementDto>>> Handle(GetAllAnnouncementsQuery request, CancellationToken cancellationToken)
    {
        var result = await _context.Announcements
            .AsNoTracking()
            .ProjectTo<AnnouncementDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Success.Ok(result);

    }
}

