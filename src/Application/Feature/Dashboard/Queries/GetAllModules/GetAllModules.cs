using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Dashboard.Queries.GetAllModules;
using System.Security.Claims;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Modules.Queries.GetUserModulePermissions;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetAllNonMemberPurposes;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

public record GetAllModulesQuery : IRequest<SuccessResponse<List<AllModuleDto>>>;

public class GetAllModulesQueryHandler : IRequestHandler<GetAllModulesQuery, SuccessResponse<List<AllModuleDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetAllModulesQueryHandler(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<SuccessResponse<List<AllModuleDto>>> Handle(GetAllModulesQuery request, CancellationToken cancellationToken)
    {
        // Get UserType from token
        var userTypeClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue("UserType");
        if (string.IsNullOrEmpty(userTypeClaim) || !Enum.TryParse<UserType>(userTypeClaim, out var userType))
            throw new UnauthorizedAccessException("Invalid or missing UserType in token.");

        // Fetch modules assigned to that UserType
        var modules = await _context.MemberTypeModuleAssignments
            .Where(x => x.UserType == userType && x.IsDeleted==false && x.IsActive==true)
            .Select(x => new AllModuleDto
            {
                Id = x.Module.Id,
                Name = x.Module.Name,
                Title = x.Module.Title,
            })
            .ToListAsync(cancellationToken);

        // Wrap in SuccessResponse
        return new SuccessResponse<List<AllModuleDto>>(modules);
    }
}

