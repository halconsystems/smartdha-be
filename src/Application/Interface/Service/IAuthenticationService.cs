using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.DependencyResolver;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Interface.Service;
public interface IAuthenticationService : IServicesType.IScopedService
{
    Task<string> GenerateToken(ApplicationUser user);
    Task<string> GenerateWebUserToken(ApplicationUser user);
    Task<string> GenerateTemporaryToken(ApplicationUser user, string purpose, TimeSpan expiresIn);
    Task<ClaimsPrincipal?> GetPrincipalFromExpiredToken(string token); // 👈 NEW
    Task<string> GenerateDriverToken(ApplicationUser user);
    ClaimsPrincipal ValidateTemporaryToken(
       string token,
       string expectedPurpose);


}
