using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Common.Settings;
using DHAFacilitationAPIs.Application.Interface.Service;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DHAFacilitationAPIs.Infrastructure.Service;
public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtSettings _jwtSettings;
    private readonly IApplicationDbContext _context;

    public AuthenticationService(UserManager<ApplicationUser> userManager,
        IOptions<JwtSettings> jwtSettings,IApplicationDbContext context)
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
        _context = context;
    }

  

    public async Task<string> GenerateTemporaryToken(ApplicationUser user, string purpose, TimeSpan expiresIn)
    {
        IList<Claim> userClaims = await _userManager.GetClaimsAsync(user);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.MobilePhone, user.MobileNo),
            new Claim("purpose", purpose)  // 👈 Add purpose: "set_password"
        };

        return GenerateAccessToken(claims, expiresIn);
    }

    public async Task<string> GenerateToken(ApplicationUser user)
    {
        IList<Claim> userClaims = await _userManager.GetClaimsAsync(user);
        IList<string> roles = await _userManager.GetRolesAsync(user);
        List<Claim> roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

        var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, $"{user.Name}"),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

                // Add these custom claims
                new Claim("CNIC", user.CNIC ?? ""),
                new Claim("MobileNo", user.MobileNo ?? ""),
                new Claim("UserType", user.UserType.ToString()),
                new Claim("MemPK", user.MEMPK.ToString())
            };
        claims.AddRange(userClaims);
        claims.AddRange(roleClaims);

        if (user.UserType == UserType.NonMember)
        {
            var assignedModuleIds = await _context.UserModuleAssignments
                .Where(x => x.UserId == user.Id && x.IsActive == true && x.IsDeleted != true && x.Module != null)
                .Select(x => x.ModuleId.ToString())
                .ToListAsync();
            if (assignedModuleIds != null && assignedModuleIds.Count > 0)
            {
                foreach (var modId in assignedModuleIds)
                {
                    if (modId != null)
                        claims.Add(new Claim("ModuleAccess", modId));
                }
            }
            else
            {
                var nonassignedModuleIds = await _context.MemberTypeModuleAssignments
               .Where(x => x.UserType == user.UserType)
               .Select(x => x.ModuleId.ToString())
               .ToListAsync();

                foreach (var modId in nonassignedModuleIds)
                {
                    if (modId != null)
                        claims.Add(new Claim("ModuleAccess", modId));
                }
            }
        }
        else
        {
            var assignedModuleIds = await _context.MemberTypeModuleAssignments
                .Where(x => x.UserType == user.UserType)
                .Select(x => x.ModuleId.ToString())
                .ToListAsync();

            foreach (var modId in assignedModuleIds)
            {
                if (modId != null)
                    claims.Add(new Claim("ModuleAccess", modId));
            }
        }

        return GenerateAccessToken(claims);
    }

    public async Task<string> GenerateWebUserToken(ApplicationUser user)
    {
        IList<Claim> userClaims = await _userManager.GetClaimsAsync(user);
        //IList<string> roles = await _userManager.GetRolesAsync(user);

        //List<Claim> roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

        // ✅ Fetch roles from your custom AppUserRole → AppRole
        var userRoles = await _context.AppUserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == user.Id)
            .Select(ur => ur.Role.Name)
            .ToListAsync();

        // ✅ Convert them to claims like before
        List<Claim> roleClaims = userRoles
            .Select(role => new Claim(ClaimTypes.Role, role))
            .ToList();



        var claims = new List<Claim>()
    {
        new Claim(ClaimTypes.Name, user.Name ?? ""),
        new Claim(ClaimTypes.Email, user.Email!),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim("UserType", user.UserType.ToString()),
    };
        claims.AddRange(userClaims);
        claims.AddRange(roleClaims);

        // 🔹 Add assigned modules
        var assignedModuleIds = await _context.UserModuleAssignments
            .Where(x => x.UserId == user.Id)
            .Select(x => x.ModuleId.ToString())
            .ToListAsync();

        foreach (var modId in assignedModuleIds)
        {
            if(modId !=null)
                claims.Add(new Claim("ModuleAccess", modId));
        }

        
        return GenerateAccessToken(claims);
    }

    public Task<ClaimsPrincipal?> GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
            ValidateLifetime = false // 👈 ignore expiration
        };


        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwt ||
                !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return Task.FromResult<ClaimsPrincipal?>(principal);// 👈 compiler warning
        }
        catch
        {
            return Task.FromResult<ClaimsPrincipal?>(null); // 👈 null
        }
    }

    #region Private Method



    private string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        //expires: DateTime.Now.AddMinutes(_jwtSettings.DurationInMinutes),
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);


        var tokeOptions = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(2500),
            signingCredentials: signinCredentials
        );
        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        return tokenString;
    }


    private string GenerateAccessToken(IEnumerable<Claim> claims, TimeSpan? expiresIn = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var now = DateTime.Now;
        var expiry = now.Add(expiresIn ?? TimeSpan.FromMinutes(_jwtSettings.DurationInMinutes));

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            notBefore: now,
            expires: expiry,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    #endregion
}
