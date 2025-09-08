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

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.CreatePanicRequest;
public record CreatePanicRequestCommand(
    Guid EmergencyTypeId, decimal Latitude, decimal Longitude, string? Notes, string? MediaUrl
) : IRequest<PanicRequestDto>;

public class CreatePanicRequestValidator : AbstractValidator<CreatePanicRequestCommand>
{
    public CreatePanicRequestValidator()
    {
        RuleFor(x => x.EmergencyTypeId).NotEmpty();
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
    }
}

public class CreatePanicRequestHandler : IRequestHandler<CreatePanicRequestCommand, PanicRequestDto>
{
    private readonly IApplicationDbContext _ctx;
    private readonly ICurrentUserService _current;
    private readonly IPanicRealtime _realtime;
    private readonly ICaseNoGenerator _caseNo;

    public CreatePanicRequestHandler(IApplicationDbContext ctx, ICurrentUserService current, IPanicRealtime realtime, ICaseNoGenerator caseNo)
        => (_ctx, _current, _realtime, _caseNo) = (ctx, current, realtime, caseNo);

    public async Task<PanicRequestDto> Handle(CreatePanicRequestCommand r, CancellationToken ct)
    {
        var et = await _ctx.EmergencyTypes
            .FirstOrDefaultAsync(x => x.Id == r.EmergencyTypeId, ct)
            ?? throw new NotFoundException("Emergency type not found.");

        var entity = new PanicRequest
        {
            RequestedByUserId = _current.UserId.ToString() ?? throw new UnAuthorizedException("Not signed in."),
            EmergencyTypeId = et.Id,
            Latitude = r.Latitude,
            Longitude = r.Longitude,
            Notes = r.Notes,
            MediaUrl = r.MediaUrl,
            Status = PanicStatus.Created,
            CaseNo = await _caseNo.NextAsync(ct)
        };

        _ctx.PanicRequests.Add(entity);
        await _ctx.SaveChangesAsync(ct);

        await _realtime.PanicCreatedAsync(entity.Id);

        return new PanicRequestDto(entity.Id, entity.CaseNo, et.Code, et.Name, entity.Latitude, entity.Longitude, entity.Status, entity.Created);
    }
}
