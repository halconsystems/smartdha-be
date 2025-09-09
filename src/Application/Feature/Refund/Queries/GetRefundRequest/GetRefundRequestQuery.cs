using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Refunds.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Application.Feature.Refunds.Queries;

public record GetRefundRequestsQuery : IRequest<List<GetRefundRequestDto>>;

public class GetRefundRequestsQueryHandler
    : IRequestHandler<GetRefundRequestsQuery, List<GetRefundRequestDto>>
{
    private readonly IOLMRSApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetRefundRequestsQueryHandler(
        IOLMRSApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<GetRefundRequestDto>> Handle(GetRefundRequestsQuery request, CancellationToken ct)
    {
        var userIdStr = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userIdStr))
            throw new UnauthorizedAccessException("Invalid or missing UserId in token.");

        var userId = Guid.Parse(userIdStr);

        // Get refund requests for reservations belonging to this user
        var refunds = await _context.RefundRequests
            .AsNoTracking()
            .Where(r => r.Reservation.UserId == userId)
            .Select(r => new GetRefundRequestDto
            {
                Id = r.Id,
                ReservationId = r.ReservationId,
                RequestedAt = r.RequestedAt,
                AmountPaid = r.AmountPaid,
                RefundableAmount = r.RefundableAmount,
                Status = r.Status,
                Notes = r.Notes
            })
            .ToListAsync(ct);

        return refunds;
    }
}
