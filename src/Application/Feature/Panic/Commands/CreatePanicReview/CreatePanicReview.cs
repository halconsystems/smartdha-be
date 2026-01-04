using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;
using ValidationException = FluentValidation.ValidationException;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.CreatePanicReview;
public record CreatePanicReviewCommand(
    Guid PanicRequestId,
    int Rating,
    string? ReviewText
) : IRequest<Guid>;
public class CreatePanicReviewCommandHandler
    : IRequestHandler<CreatePanicReviewCommand, Guid>
{
    private readonly IApplicationDbContext _ctx;
    private readonly ICurrentUserService _current;

    public CreatePanicReviewCommandHandler(IApplicationDbContext ctx, ICurrentUserService current)
    {
        _ctx = ctx;
        _current = current;
    }

    public async Task<Guid> Handle(CreatePanicReviewCommand request, CancellationToken ct)
    {
        if (request.Rating < 1 || request.Rating > 5)
            throw new ValidationException("Rating must be between 1 and 5.");

        var userId = _current.UserId.ToString()
            ?? throw new UnAuthorizedException("Not signed in.");

        // Load panic
        var panic = await _ctx.PanicRequests
            .FirstOrDefaultAsync(x => x.Id == request.PanicRequestId, ct)
            ?? throw new NotFoundException("Panic not found.");

        // Ensure this panic belongs to the current user (review by requester only)
        if (panic.RequestedByUserId != userId)
            throw new UnAuthorizedException("You can only review your own panic request.");

        // Ensure panic is completed/resolved (or you can allow Cancelled too)
        if (panic.Status != PanicStatus.Resolved)
            throw new ValidationException("You can only review after the panic is resolved.");

        panic.TakeReview = false;
        // Prevent duplicate review
        var exists = await _ctx.PanicReviews
            .AnyAsync(x => x.PanicRequestId == request.PanicRequestId, ct);

        if (exists)
            throw new RecordAlreadyExistException("Review already submitted for this panic.");

        var entity = new PanicReview
        {
            Id = Guid.NewGuid(),
            PanicRequestId = request.PanicRequestId,
            Rating = request.Rating,
            ReviewText = string.IsNullOrWhiteSpace(request.ReviewText) ? null : request.ReviewText.Trim()
        };

        _ctx.PanicReviews.Add(entity);
        await _ctx.SaveChangesAsync(ct);

        return entity.Id;
    }
}

