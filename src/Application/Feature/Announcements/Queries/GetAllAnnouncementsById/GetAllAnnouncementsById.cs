using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Announcements.Commands.AddAnnouncement;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.Announcements.Queries.GetAllAnnouncementsById;
public class GetAnnouncementByIdQuery : IRequest<SuccessResponse<AnnouncementDto>>
{
    public Guid Id { get; set; }
}

public class GetAnnouncementByIdQueryHandler
    : IRequestHandler<GetAnnouncementByIdQuery, SuccessResponse<AnnouncementDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAnnouncementByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<SuccessResponse<AnnouncementDto>> Handle(GetAnnouncementByIdQuery request, CancellationToken cancellationToken)
    {
        var dto = await _context.Announcements
            .AsNoTracking()
            .Where(a => a.Id == request.Id)
            .ProjectTo<AnnouncementDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(cancellationToken);

        if (dto is null)
        {

            // OPTION B: If you don't, use the response type directly:
            return new SuccessResponse<AnnouncementDto>(data: default!)
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Detail = $"Announcement '{request.Id}' not found."
            };
        }

        return Success.Ok(dto);
    }
}

