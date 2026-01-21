using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryCategory;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryItems;
using DHAFacilitationAPIs.Application.Feature.Orders.Queries;
using DHAFacilitationAPIs.Application.Feature.OrderTaxDiscount.Queries;
using DHAFacilitationAPIs.Application.Feature.Room.Queries.GetAllRooms;
using DHAFacilitationAPIs.Domain.Enums;
using DHAFacilitationAPIs.Domain.Enums.GBMS;

namespace DHAFacilitationAPIs.Application.Feature.Grounds.Queries;

public record GetGroundQuery(GroundCategory GroundCategory) : IRequest<List<GroundDTO>>;

public class GetGroundQueryHandler : IRequestHandler<GetGroundQuery, List<GroundDTO>>
{
    private readonly IOLMRSApplicationDbContext _context;

    public GetGroundQueryHandler(
         IOLMRSApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<GroundDTO>> Handle(GetGroundQuery request, CancellationToken cancellationToken)
    {
        var ground =  await _context.Grounds.Where(x => x.GroundCategory == request.GroundCategory)
            .AsNoTracking()
            .ToListAsync();

        var groundImage = await _context.GroundImages.Where(x => ground.Select(x => x.Id).Contains(x.GroundId)).ToListAsync();

        var groundSlots = await _context.GroundSlots.Where(x => ground.Select(x => x.Id).Contains(x.GroundId)).ToListAsync();

        var groundGroundStandardTime = await _context.GroundStandtardTimes.Where(x => ground.Select(x => x.Id).Contains(x.GroundId)).ToListAsync();

        var clubs = await _context.Clubs.Where(x => ground.Select(x => x.ClubId).Contains(x.Id)).ToListAsync();

        var grounds = ground.Select(x => new GroundDTO
        {
            Id = x.Id,
            GroundName = x.Name,
            MainImageUrl = groundImage?.FirstOrDefault(y => y.GroundId == x.Id)?.ImageURL,
            GroundDescription = x.Description,
            GroundType = x.GroundType,
            GroundCategory = x.GroundCategory,
            Location = x.Location,
            ContactNumber = x.ContactNumber,
            AccountNo = x.AccountNo,
            AccountNoAccronym = x.AccountNoAccronym,
            SlotCount = groundSlots.Where(g => g.GroundId == x.Id).Count().ToString(),
            GroundStandtardTime = groundGroundStandardTime.Where(s => s.GroundId == x.Id).FirstOrDefault() 
            //ClubName = clubs != null ? clubs.FirstOrDefault(c => c.Id == x.ClubId).Name : ""

        }).ToList();

        return grounds;
    }
}



