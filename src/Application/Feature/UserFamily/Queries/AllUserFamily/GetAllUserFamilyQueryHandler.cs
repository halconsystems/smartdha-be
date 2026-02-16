using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.UserFamily.Queries.AllUserFamily;
using DHAFacilitationAPIs.Application.Feature.UserFamily.Queries.UserFamilyById;
using DHAFacilitationAPIs.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Application.Feature.UserFamily.Queries.AllUserFamily
{
    public class GetAllUserFamilyQueryHandler : IRequestHandler<GetAllUserFamilyQuery, Result<List<GetAllUserFamilyQueryResponse>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ISmartdhaDbContext _smartdhaDbContext;

        public GetAllUserFamilyQueryHandler(IApplicationDbContext context, ISmartdhaDbContext smartdhaDbContext)
        {
            _context = context;
            _smartdhaDbContext = smartdhaDbContext;
        }

        public async Task<Result<List<GetAllUserFamilyQueryResponse>>> Handle(GetAllUserFamilyQuery request,CancellationToken cancellationToken)
        {
            var users = await _smartdhaDbContext.UserFamilies.ToListAsync(cancellationToken);

            if (!users.Any())
                return Result<List<GetAllUserFamilyQueryResponse>>
                    .Failure(new[] { "No users found" });

            var response = users.Select(user => new GetAllUserFamilyQueryResponse
            {
                DOB = user.DateOfBirth,
                Name = user.Name,
                Phone = user.PhoneNumber!,
                Relation = (int)user.Relation,
                CNIC = user.Cnic!,
                Image = user.ProfilePicture ?? string.Empty,
                ResidentCardNumber = user.ResidentCardNumber!
            }).ToList();

            return Result<List<GetAllUserFamilyQueryResponse>>.Success(response);
        }

    }
}
