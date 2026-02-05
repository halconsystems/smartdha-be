using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Enums;
using static Dapper.SqlMapper;

namespace DHAFacilitationAPIs.Application.Feature.Clubs.Commands.CreateClub;
public record CreateClubCommand(string Name,string DisplayName, string? Description, string? Location, string? ContactNumber, string? AccountNo, string? AccountNoAccronym, ClubType ClubType, string Email)
    : IRequest<SuccessResponse<string>>;

public class CreateClubCommandHandler : IRequestHandler<CreateClubCommand, SuccessResponse<string>>
{
    private readonly ICBMSApplicationDbContext _ctx;
    public CreateClubCommandHandler(ICBMSApplicationDbContext ctx) => _ctx = ctx;

    public async Task<SuccessResponse<string>> Handle(CreateClubCommand request, CancellationToken ct)
    {
        try
        {
            var entity = new DHAClub
            {
                Name = request.Name,
                DisplayName = request.DisplayName,
                Description = request.Description,
                Location = request.Location,
                ContactNumber = request.ContactNumber,
                AccountNo = request.AccountNo,
                AccountNoAccronym = request.AccountNoAccronym,
                Email = request.Email
            };
            _ctx.Clubs.Add(entity);
            await _ctx.SaveChangesAsync(ct);
            return Success.Created(entity.Id.ToString());
        }
        catch (Exception)
        {

            throw;
        }
        
    }
}

