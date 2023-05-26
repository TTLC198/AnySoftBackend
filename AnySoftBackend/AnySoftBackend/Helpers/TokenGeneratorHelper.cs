using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AnySoftBackend.Helpers;

public static class TokenGeneratorHelper
{
    /// <summary>
    /// Get Jwt Bearer security token
    /// </summary>
    /// <param name="authClaims"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Check appsettings.json file</exception>
    public static JwtSecurityToken GenerateToken(List<Claim> authClaims, IConfiguration configuration)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtConfiguration:Secret"] ?? throw new InvalidOperationException("JWT Secret is null")));

        var token = new JwtSecurityToken(
            issuer: configuration["JwtConfiguration:Issuer" ?? throw new InvalidOperationException("JWT Issuer is null")],
            audience: configuration["JwtConfiguration:Audience" ?? throw new InvalidOperationException("JWT Audience is null")],
            expires: DateTime.UtcNow.Add(TimeSpan.FromMilliseconds(long.Parse(configuration["JwtConfiguration:Expiration"] ?? throw new InvalidOperationException("JWT Expiration is null")))),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }
}