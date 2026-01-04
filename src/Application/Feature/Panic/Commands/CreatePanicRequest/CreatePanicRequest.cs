using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.CreatePanicRequest;
public record CreatePanicRequestCommand(
    Guid EmergencyTypeId, double Latitude, double Longitude, string? Notes, string? MediaUrl, string? MobileNumber
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
    private readonly IGeocodingService _geocodingService;



    public CreatePanicRequestHandler(IApplicationDbContext ctx, ICurrentUserService current, IPanicRealtime realtime, ICaseNoGenerator caseNo,UserManager<ApplicationUser> userManager,IGeocodingService geocodingService)
        => (_ctx, _current, _realtime, _caseNo, _userManager,_geocodingService) = (ctx, current, realtime, caseNo,userManager,geocodingService);

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

        // -----------------------------------------
        // GPS Validation (double cannot be null)
        // -----------------------------------------
        if (r.Latitude == 0 || r.Longitude == 0)
            throw new InvalidOperationException("Location disabled. Please enable location and try again.");

        if (r.Latitude < -90 || r.Latitude > 90 ||
            r.Longitude < -180 || r.Longitude > 180)
            throw new InvalidOperationException("Invalid location received. Please enable GPS and try again.");



        var existingActivePanic = await _ctx.PanicRequests
         .Where(x =>
            x.RequestedByUserId == userId &&
             (
                (x.Status != PanicStatus.Resolved && x.Status != PanicStatus.Cancelled)
                ||
                (x.Status == PanicStatus.Resolved && x.TakeReview == true)
             )
            )
         .OrderByDescending(p => p.Created)
         .FirstOrDefaultAsync(ct);

        if (existingActivePanic != null)
        {
            throw new InvalidOperationException(
                "A panic request is already active. You cannot create another one until the previous panic is resolved or cancelled."
            );
        }

        var entity = new PanicRequest
        {
            RequestedByUserId = _current.UserId.ToString() ?? throw new UnAuthorizedException("Not signed in."),
            EmergencyTypeId = et.Id,
            Latitude = r.Latitude,
            Longitude = r.Longitude,
            Notes = r.Notes,
            MediaUrl = r.MediaUrl,
            MobileNumber = r.MobileNumber,
            Status = PanicStatus.Created,
            Address= await _geocodingService.GetAddressFromLatLngAsync(r.Latitude, r.Longitude, ct),
            CaseNo = await _caseNo.NextAsync(ct),
            TakeReview=false
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
         entity.Created,
         entity.TakeReview
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
            Address: entity.Address ?? "",
            Note:entity.Notes ?? "",
            MobileNumber:entity.MobileNumber ?? "",

            RequestedByName: user.Name,
            RequestedByEmail: user.Email ?? user.RegisteredEmail ?? string.Empty,
            RequestedByPhone: user.MobileNo ?? user.RegisteredMobileNo ?? string.Empty,
            RequestedByUserType: user.UserType
        );

        //await _realtime.PanicCreatedAsync(rtDto);
        try
        {
            var finaldto = new PanicUpdatedRealtimeDto
            {
                PanicId = entity.Id,
                CaseNo = entity.CaseNo,
                EmergencyCode = et.Code,
                EmergencyName = et.Name,
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
                Created= entity.Created,
                PanicStatus = entity.Status,
                Note=entity.Notes ?? "",
                Address=entity.Address ?? "",
                MobileNumber =entity.MobileNumber ?? "",

                RequestedByName= user.Name,
                RequestedByEmail = user.Email ?? user.RegisteredEmail ?? string.Empty,
                RequestedByPhone = user.MobileNo ?? user.RegisteredMobileNo ?? string.Empty,
            };


            //await _realtime.PanicCreatedAsync(rtDto);
            await _realtime.SendPanicUpdatedAsync(finaldto, ct);
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
