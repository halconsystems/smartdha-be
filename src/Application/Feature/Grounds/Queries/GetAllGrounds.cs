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
using DHAFacilitationAPIs.Domain.Entities.GBMS;
using DHAFacilitationAPIs.Domain.Enums;
using DHAFacilitationAPIs.Domain.Enums.GBMS;

namespace DHAFacilitationAPIs.Application.Feature.Grounds.Queries;

public record GetGroundQuery(GroundCategory GroundCategory, DateOnly bookingdate) : IRequest<List<GroundDTO>>;

public class GetGroundQueryHandler : IRequestHandler<GetGroundQuery, List<GroundDTO>>
{
    private readonly IOLMRSApplicationDbContext _context;
    private readonly IFileStorageService _fileStorageService;

    public GetGroundQueryHandler(
         IOLMRSApplicationDbContext context, IFileStorageService fileStorageService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
    }

    public async Task<List<GroundDTO>> Handle(GetGroundQuery request, CancellationToken cancellationToken)
    {
        var ground =  await _context.Grounds.Where(x => x.GroundCategory == request.GroundCategory)
            .AsNoTracking()
            .ToListAsync();

        var groundImage = await _context.GroundImages.Where(x => ground.Select(x => x.Id).Contains(x.GroundId)).ToListAsync();

        var groundSlots = await _context.GroundSlots.Where(x => ground.Select(x => x.Id).Contains(x.GroundId)).ToListAsync();

        var groundBooked = await _context.GroundBookings.Where(x => ground.Select(g => g.Id).Contains(x.GroundId) && x.BookingDateOnly == request.bookingdate)
           .AsNoTracking()
           .ToListAsync();

        var bookedSlots = await _context.GroundBookingSlots.Where(x => groundBooked.Select(x => x.Id).Contains(x.BookingId) && groundSlots.Select(x => x.Id).Contains(x.SlotId)).ToListAsync();

        var groundGroundStandardTime = await _context.GroundStandtardTimes.Where(x => ground.Select(x => x.Id).Contains(x.GroundId)).ToListAsync();

        var clubs = await _context.Clubs.Where(x => ground.Select(x => x.ClubId).Contains(x.Id)).ToListAsync();

        var grounds = ground.Select(x =>
        {
            var imagePath = groundImage?
                .FirstOrDefault(y => y.GroundId == x.Id)?
                .ImageURL;

            return new GroundDTO
            {
                Id = x.Id,
                GroundName = x.Name,
                MainImageUrl = imagePath == null
                    ? null
                    : _fileStorageService.GetPublicUrl(imagePath),

                GroundDescription = x.Description,
                GroundType = x.GroundType,
                GroundCategory = x.GroundCategory,
                Location = x.Location,
                ContactNumber = x.ContactNumber,
                AccountNo = x.AccountNo,
                AccountNoAccronym = x.AccountNoAccronym,
                SlotCount = groundSlots.Count(g => g.GroundId == x.Id).ToString(),
                GroundStandtardTime = groundGroundStandardTime
                                        .FirstOrDefault(s => s.GroundId == x.Id),
                ClubId = clubs?.FirstOrDefault(c => c.Id == x.ClubId)?.Id,
                ClubName = clubs?.FirstOrDefault(c => c.Id == x.ClubId)?.Name ?? ""
            };
        }).ToList();

        return grounds;
    }
}



