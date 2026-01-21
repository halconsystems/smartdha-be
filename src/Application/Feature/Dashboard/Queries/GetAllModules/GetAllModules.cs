using System.Security.Claims;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.Dashboard.Queries.GetAllModules;
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
        // Get UserType and UserID from token
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        var userTypeClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue("UserType");
        if (string.IsNullOrEmpty(userTypeClaim) || !Enum.TryParse<UserType>(userTypeClaim, out var userType))
            throw new UnauthorizedAccessException("Invalid or missing UserType in token.");
        if (string.IsNullOrWhiteSpace(userId))
            throw new UnauthorizedAccessException("Invalid or missing UserId in token.");

        List<AllModuleDto> modules;

        // Local method for default modules by user type
        async Task<List<AllModuleDto>> GetDefaultModules(UserType type)
        {
            return await _context.MemberTypeModuleAssignments
                .Where(x => x.UserType == type && x.IsActive == true && x.IsDeleted != true)
                .Select(x => new AllModuleDto
                {
                    Id = x.Module.Id,
                    Name = x.Module.Name,
                    Title = x.Module.Title,
                })
                .ToListAsync(cancellationToken);
        }
        
        if (userType == UserType.NonMember)
        {
            // fetch assigned modules for the non-member else fetches default
            var assignedModules = await _context.UserModuleAssignments
                .Where(x => x.UserId == userId && x.IsActive == true && x.IsDeleted != true && x.Module != null)
                .Select(x => new AllModuleDto
                {
                    Id = x.Module!.Id,
                    Name = x.Module.Name,
                    Title = x.Module.Title,
                })
                .ToListAsync(cancellationToken);

            modules = assignedModules.Any() ? assignedModules : await GetDefaultModules(UserType.NonMember);
        }
        else
        {
            modules = await GetDefaultModules(UserType.Member);

            var moduleOrder = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                ["Smart DHA"] = 1,
                ["Property"] = 2,
                ["My Bills"] = 3,
                ["Services"] = 4,
                ["Panic"] = 5,
                ["Complaint"] = 6,
                ["Ground Booking"] = 7,
                ["Bowser"] = 8,
                ["Club"] = 9,
                ["QR"] = 10
            };

            modules = modules
           .OrderBy(m => moduleOrder.TryGetValue(m.Name, out var order) ? order : int.MaxValue)
           .ThenBy(m => m.Name)
           .ToList();

         }

        // Wrap in SuccessResponse
        return new SuccessResponse<List<AllModuleDto>>(modules);
    }
}

