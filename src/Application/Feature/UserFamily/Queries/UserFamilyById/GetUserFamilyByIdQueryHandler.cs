using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.UserFamily.Queries.UserFamilyById;

public class GetUserFamilyByIdQueryHandler: IRequestHandler<GetUserFamilyByIdQuery, GetUserFamilybyIdQueryResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartdhaDbContext;

    public GetUserFamilyByIdQueryHandler(IApplicationDbContext context , ISmartdhaDbContext smartdhaDbContext)
    {
        _context = context;
        _smartdhaDbContext = smartdhaDbContext;
    }

    public async Task<GetUserFamilybyIdQueryResponse> Handle(
     GetUserFamilyByIdQuery request,
     CancellationToken cancellationToken)
    {
        var userData = await _smartdhaDbContext.UserFamilies
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (userData == null)
            throw new Exception("User family not found");   

        var userResponse = new GetUserFamilybyIdQueryResponse
        {
            DOB = userData.DateOfBirth,
            Name = userData.Name,
            Phone = userData.PhoneNumber!,
            Relation = (int)userData.Relation,
            CNIC = userData.Cnic!,
            Image = userData.ProfilePicture ?? string.Empty,
            ResidentCardNumber = userData.ResidentCardNumber!   
        };

        return userResponse;
    }

}

