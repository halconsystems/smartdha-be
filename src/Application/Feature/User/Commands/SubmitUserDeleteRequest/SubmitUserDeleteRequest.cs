using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.User.Commands.SubmitUserDeleteRequest;
public record SubmitUserDeleteRequestCommand(
    string? Reason
) : IRequest<ApiResult<Guid>>;

public class SubmitUserDeleteRequestCommandHandler
    : IRequestHandler<SubmitUserDeleteRequestCommand, ApiResult<Guid>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public SubmitUserDeleteRequestCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ApiResult<Guid>> Handle(
        SubmitUserDeleteRequestCommand request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        if (userId == Guid.Empty)
            return ApiResult<Guid>.Fail("Invalid user");

        var alreadyRequested = await _db.Set<UserDeleteRequest>()
            .AnyAsync(x => x.UserId == userId, ct);

        if (alreadyRequested)
            return ApiResult<Guid>.Fail("Delete request already exists");

        var entity = new UserDeleteRequest
        {
            UserId = userId,
            Reason = request.Reason
        };

        _db.Set<UserDeleteRequest>().Add(entity);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(
            entity.Id,
            "Delete account request submitted successfully"
        );
    }
}

