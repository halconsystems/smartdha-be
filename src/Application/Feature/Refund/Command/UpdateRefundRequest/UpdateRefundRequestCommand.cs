using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DHAFacilitationAPIs.Application.Feature.Refund.Command.UpdateRefundRequest;

public class UpdateRefundRequestCommand : IRequest<string>
{
    public Guid RefundRequestId { get; set; }
    public RefundStatus Status { get; set; }   // Approved / Rejected
    public string? Notes { get; set; }
}

public class UpdateRefundRequestCommandHandler
    : IRequestHandler<UpdateRefundRequestCommand, string>
{
    private readonly IOLMRSApplicationDbContext _context;

    public UpdateRefundRequestCommandHandler(IOLMRSApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(UpdateRefundRequestCommand request, CancellationToken cancellationToken)
    {
        if (request.RefundRequestId == Guid.Empty)
            throw new ArgumentException("RefundRequestId must be provided.");

        var refundRequest = await _context.RefundRequests
            .FirstOrDefaultAsync(rr => rr.Id == request.RefundRequestId, cancellationToken);

        if (refundRequest == null)
            throw new NotFoundException(nameof(RefundRequest), request.RefundRequestId.ToString());

        if (refundRequest.Status != RefundStatus.Pending)
            throw new InvalidOperationException("Only pending refund requests can be updated.");

        refundRequest.Status = request.Status;
        refundRequest.Notes = request.Notes ?? refundRequest.Notes;

        _context.RefundRequests.Update(refundRequest);
        await _context.SaveChangesAsync(cancellationToken);

        var message = "Refund request updated successfully.";
        return message; 
    }
}
