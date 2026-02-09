using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Queries.GetProcessPrerequisites;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.Proeperty.Queries.GetPropertyByMemPK;
public class GetMyPropertiesQuery : IRequest<SuccessResponse<List<PlotInfoDto>>>{}

public class GetMyPropertiesQueryHandler : IRequestHandler<GetMyPropertiesQuery, SuccessResponse<List<PlotInfoDto>>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IPropertyInfoService _propertyInfoService;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetMyPropertiesQueryHandler(ICurrentUserService currentUserService,IPropertyInfoService propertyInfoService, UserManager<ApplicationUser> userManager)
    {
        _currentUserService=currentUserService;
        _propertyInfoService=propertyInfoService;
        _userManager=userManager;

    }

    public async Task<SuccessResponse<List<PlotInfoDto>>> Handle(GetMyPropertiesQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId.ToString() ?? throw new UnauthorizedAccessException("Invalid user.");
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            throw new UnAuthorizedException("Invalid CNIC. Please verify and try again.");
        }

        var spProperties = await _propertyInfoService.GetPropertiesByCnicAsync(user.CNIC, cancellationToken);

        if (spProperties == null || !spProperties.Any())
        {
            throw new Exception("No property found.");
        }

        var properties = spProperties
            .Select(x => new PlotInfoDto(            
                PropertyNo: x.PLOT_NO ?? x.PLTNO ?? string.Empty,
                Sector: x.SUBDIV,
                Phase: x.PHASE,
                Address: x.PROPERTY_ADDRESS,
                Name: user.Name,
                CNIC: user.CNIC
            ))
            .ToList();

        return new SuccessResponse<List<PlotInfoDto>>(properties, "Properties fetched successfully.");
    }
}
