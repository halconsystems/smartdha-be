using System.Threading;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.UserFamily.Commands.AddUserFamilyCommandHandler;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Application.Feature.UserFamily.UserFamilyCommands.AddUserFamilyCommandHandler;

public class AddUserFamilyCommandHandler : IRequestHandler<AddUserFamilyCommand, AddUserFamilyResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartDhaContext;

    public AddUserFamilyCommandHandler(IApplicationDbContext context, ISmartdhaDbContext smartDhaContext)
    {
        _context = context;
        _smartDhaContext = smartDhaContext;
    }

    public async Task<AddUserFamilyResponse> Handle(AddUserFamilyCommand request, CancellationToken cancellationToken)
    {
        var response = new AddUserFamilyResponse();
        var entity = new Domain.Entities.Smartdha.UserFamily
        {
            Name = request.Name,
            Relation = (Relation)request.Relation,
            DateOfBirth = request.DOB,
            Cnic = request.CNIC,
            FatherOrHusbandName = request.FatherName,
            ProfilePicture = request.Image,
            PhoneNumber = request.PhoneNo,
        };

        await _smartDhaContext.UserFamilies.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        response.Success = true;
        response.Message = "Family member added successfully.";
        response.Id = entity.Id;
        return response;
    }
}
