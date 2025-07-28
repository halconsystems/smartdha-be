using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Common.Settings;
using DHAFacilitationAPIs.Application.Interface.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Infrastructure.Service;
public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtSettings _jwtSettings;

    public AuthenticationService(UserManager<ApplicationUser> userManager,
        IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
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
                new Claim(ClaimTypes.Name, $"{user.UserName}"),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                // Add these custom claims
                new Claim("CNIC", user.CNIC ?? ""),
                new Claim("MobileNo", user.MobileNo ?? ""),
                new Claim("UserType", user.UserType.ToString())
            };
        claims.AddRange(userClaims);
        claims.AddRange(roleClaims);
        return GenerateAccessToken(claims);
    }



    #region Private Method

    private string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var tokeOptions = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtSettings.DurationInMinutes),
            signingCredentials: signinCredentials
        );
        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        return tokenString;
    }


    private string GenerateAccessToken(IEnumerable<Claim> claims, TimeSpan? expiryOverride = null)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var tokenOptions = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.Now.Add(expiryOverride ?? TimeSpan.FromMinutes(_jwtSettings.DurationInMinutes)),
            signingCredentials: signinCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    #endregion
}
