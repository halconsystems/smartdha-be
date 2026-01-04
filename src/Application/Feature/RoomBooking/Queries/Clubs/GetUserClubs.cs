using Application.Feature.RoomBooking.Queries.Clubs.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.Clubs;

public class GetUserClubsQuery() : IRequest<List<UserClubDto>>
{
    public Guid UserId { get; set; } = default!;  // Pass this from claims or query param
}

public class GetUserClubsQueryHandler : IRequestHandler<GetUserClubsQuery, List<UserClubDto>>
{
    private readonly IOLMRSApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetUserClubsQueryHandler(IOLMRSApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<UserClubDto>> Handle(GetUserClubsQuery request, CancellationToken cancellationToken)
    {
        //var clubs = await (
        //    from m in _context.UserClubMemberships.AsNoTracking()
        //    join c in _context.Clubs.AsNoTracking() on m.ClubId equals c.Id
        //    where m.UserId == request.UserId
        //          && m.IsActive == true
        //          && m.IsDeleted != true
        //          && c.ClubType == ClubType.GuestRoom
        //    select new UserClubDto
        //    {
        //        ClubId = c.Id.ToString(), // Assuming ClubId is Guid in DB
        //        Name = c.Name
        //    }
        //)
        //.Distinct() // Works here because projecting primitive types in DTO
        //.OrderBy(x => x.Name)
        //.ToListAsync(cancellationToken);

        var clubs = await _context.Clubs
        .AsNoTracking()
        .Where(x=>x.ClubType== ClubType.GuestRoom && x.IsDeleted==false && x.IsActive==true)
        .Select(c => new UserClubDto
        {
            ClubId = c.Id.ToString(),
            Name = c.Name
        })
        .OrderBy(x => x.Name)
        .ToListAsync(cancellationToken);

        return clubs;
    }
}
