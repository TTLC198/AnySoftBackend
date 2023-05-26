using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.VisualBasic;
using Moq;

namespace AnySoftBackend.Tests.Helpers;

public static class JwtClaimsHelper
{
    public static ClaimsPrincipal GetClaims(int id, string role)
    {
        var claimsContextMock = new Mock<HttpContext>();

        claimsContextMock
            .Setup(x => x.User)
            .Returns(new ClaimsPrincipal());

        var authClaims = new List<Claim>
        {
            new ("id", Strings.Trim($"{id}")),
            new ("role", Strings.Trim(role)),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        claimsContextMock.Object.User.AddIdentity(new ClaimsIdentity(authClaims));

        return claimsContextMock.Object.User;
    }
}