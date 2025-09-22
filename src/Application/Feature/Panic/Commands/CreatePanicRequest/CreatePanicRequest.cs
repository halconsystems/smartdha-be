using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;
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
    private readonly UserManager<ApplicationUser> _userManager;


    public CreatePanicRequestHandler(IApplicationDbContext ctx, ICurrentUserService current, IPanicRealtime realtime, ICaseNoGenerator caseNo,UserManager<ApplicationUser> userManager)
        => (_ctx, _current, _realtime, _caseNo, _userManager) = (ctx, current, realtime, caseNo,userManager);

    public async Task<PanicRequestDto> Handle(CreatePanicRequestCommand r, CancellationToken ct)
    {
        var et = await _ctx.EmergencyTypes
            .FirstOrDefaultAsync(x => x.Id == r.EmergencyTypeId, ct)
            ?? throw new NotFoundException("Emergency type not found.");

        var userId = _current.UserId.ToString()
        ?? throw new UnAuthorizedException("Not signed in.");

        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == userId, ct)
            ?? throw new NotFoundException("User not found.");

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

        var apiDto = new PanicRequestDto(
         entity.Id,
         entity.CaseNo,
         et.Code,
         et.Name,
         entity.Latitude,
         entity.Longitude,
         entity.Status,
         entity.Created
     );

        // Realtime DTO (with user info)
        var rtDto = new PanicCreatedRealtimeDto(
            Id: entity.Id,
            CaseNo: entity.CaseNo,
            EmergencyCode: et.Code,
            EmergencyName: et.Name,
            Latitude: entity.Latitude,
            Longitude: entity.Longitude,
            Status: entity.Status,
            CreatedUtc: entity.Created,

            RequestedByName: user.Name,
            RequestedByEmail: user.Email ?? user.RegisteredEmail ?? string.Empty,
            RequestedByPhone: user.MobileNo ?? user.RegisteredMobileNo ?? string.Empty,
            RequestedByUserType: user.UserType
        );

        //await _realtime.PanicCreatedAsync(rtDto);
        try
        {
            await _realtime.PanicCreatedAsync(rtDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            //_logger.LogWarning(ex, "PanicCreated realtime broadcast failed for {CaseNo}", entity.CaseNo);
            // don't rethrow; creation succeeded already
        }

        return apiDto;
    }
}
