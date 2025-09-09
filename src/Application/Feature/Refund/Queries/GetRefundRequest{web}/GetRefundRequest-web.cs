using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Refunds.Queries;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DHAFacilitationAPIs.Application.Feature.Refund.Queries.GetRefundRequest_web_;
public record GetRefundRequestsWebQuery(Guid ClubId) : IRequest<List<RefundRequestWebDto>>;

public class GetRefundRequestsWebQueryHandler : IRequestHandler<GetRefundRequestsWebQuery, List<RefundRequestWebDto>>
{
    private readonly IOLMRSApplicationDbContext _context;
    private readonly IApplicationDbContext _context1;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetRefundRequestsWebQueryHandler(IOLMRSApplicationDbContext context, IApplicationDbContext context1, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _context1 = context1;
        _userManager = userManager;
    }

    public async Task<List<RefundRequestWebDto>> Handle(GetRefundRequestsWebQuery request, CancellationToken ct)
    {
        return await (
        from rr in _context.RefundRequests
        join res in _context.Reservations on rr.ReservationId equals res.Id
        join user in _userManager.Users on res.UserId.ToString() equals user.Id
        where res.ClubId == request.ClubId
        select new RefundRequestWebDto
        {
            RefundRequestId = rr.Id,
            ReservationId = rr.ReservationId,
            AmountPaid = rr.AmountPaid,
            RefundableAmount = rr.RefundableAmount,
            Status = rr.Status.ToString(),
            RequestedAtDateOnly = rr.RequestedAtDateOnly,
            RequestedAtTimeOnly = rr.RequestedAtTimeOnly,

            ClubId = res.ClubId,
            ClubName = res.Club.Name, 

            UserId = user.Id,
            UserName = user.Name,
            CNIC = user.CNIC,
            RegisteredMobileNo = user.RegisteredMobileNo,

            RoomBookings = res.RoomBookings
                .Select(rb => new RoomBookingDto
                {
                    RoomBookingId = rb.Id,
                    RoomName = rb.Room.Name,
                    RoomNo = rb.Room.No
                }).ToList()
        }).ToListAsync(ct);
    }
}
