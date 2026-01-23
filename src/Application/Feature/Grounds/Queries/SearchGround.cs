using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.SearchRooms;
using DHAFacilitationAPIs.Domain.Entities.GBMS;
using DHAFacilitationAPIs.Domain.Enums;
using DHAFacilitationAPIs.Domain.Enums.GBMS;

namespace DHAFacilitationAPIs.Application.Feature.Grounds.Queries;

public record SearchGroundQuery(
    DateOnly CheckInDate,
    DateOnly CheckOutDate,
    GroundCategory GroundCategory, // "self" or "guest"
    TimeOnly? CheckInTime = null,
    TimeOnly? CheckOutTime = null
) : IRequest<List<GroundDTO>>;

public class SearchGroundQueryHandler : IRequestHandler<SearchGroundQuery, List<GroundDTO>>
{
    private readonly IOLMRSApplicationDbContext _context;
    private readonly IFileStorageService _fileStorageService;

    public SearchGroundQueryHandler(IOLMRSApplicationDbContext context, IFileStorageService fileStorageService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
    }

    public async Task<List<GroundDTO>> Handle(SearchGroundQuery request, CancellationToken ct)
    {
        var startDate = request.CheckInDate;   // DateOnly
        var endDate = request.CheckOutDate;  // DateOnly
        TimeOnly checkIn;
        TimeOnly checkOut;
        GroundStandtardTime grounstandard = new GroundStandtardTime();
        if (request.CheckInTime.HasValue && request.CheckOutTime.HasValue)
        {
            checkIn = request.CheckInTime.Value;
            checkOut = request.CheckOutTime.Value;
        }

        

        var GroundSlots = await _context.GroundSlots.AsNoTracking()
                            .Where(std => std.IsDeleted == false && std.IsActive == true &&
                            std.FromDateOnly == startDate  &&
                            std.ToDateOnly == endDate &&
                            std.FromTimeOnly == request.CheckInTime &&
                            std.ToTimeOnly == request.CheckOutTime
                            )
                            .ToListAsync(ct);

        var ground = await _context.Grounds.Where(x => x.GroundCategory == request.GroundCategory && GroundSlots.Select(x => x.GroundId).Contains(x.Id))
           .AsNoTracking()
           .ToListAsync();

        var groundBooked = await _context.GroundBookings.Where(x => ground.Select(g => g.Id).Contains(x.GroundId))
           .AsNoTracking()
           .ToListAsync();

        var bookedSlots = await _context.GroundBookingSlots.Where(x => groundBooked.Select(x => x.Id).Contains(x.BookingId) && GroundSlots.Select(x => x.Id).Contains(x.SlotId)).ToListAsync();

        var groundImage = await _context.GroundImages.Where(x => ground.Select(x => x.Id).Contains(x.GroundId)).ToListAsync();

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
                SlotCount = GroundSlots.Count(g => g.GroundId == x.Id).ToString(),
                GroundStandtardTime = groundGroundStandardTime
                                        .FirstOrDefault(s => s.GroundId == x.Id),
                ClubId = clubs?.FirstOrDefault(c => c.Id == x.ClubId)?.Id,
                ClubName = clubs?.FirstOrDefault(c => c.Id == x.ClubId)?.Name ?? "",
                Action = bookedSlots.FirstOrDefault(g => g.Equals(x.Id)) == null ? AvailabilityAction.Booked : AvailabilityAction.Available,
            };
            })
            .OrderByDescending(x => x.Action == AvailabilityAction.Available)
            .ToList();

        return grounds;
    }





}

