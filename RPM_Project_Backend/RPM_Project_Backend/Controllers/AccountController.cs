using System.IdentityModel.Tokens.Jwt;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using RPM_PR_LIB;
using RPM_Project_Backend.Helpers;
using RPM_Project_Backend.Models;
using RPM_Project_Backend.Services.Database;

namespace RPM_Project_Backend.Controllers;

/// <inheritdoc />
[ApiController]
[ApiVersion("1.0")]
[Route("api/auth")]
public class AccountController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AccountController> _logger;
    private readonly ApplicationContext _context;
    private readonly DbSet<User> _dbSet;

    /// <inheritdoc />
    public AccountController(
        IConfiguration configuration, ILogger<AccountController> logger, ApplicationContext context)
    {
        _configuration = configuration;
        _logger = logger;
        _context = context;
        _dbSet = _context.Set<User>();
    }
    /// <summary>
    /// Login as user and get JWT Bearer token to get access to non anonymous methods 
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// POST /api/auth/login&#xA;&#xD;
    ///     {
    ///        "login": "ttlc198",
    ///        "email": "",
    ///        "password": "M$4d3ikx+L"
    ///     }
    /// 
    /// </remarks>
    /// <response code="200">Return Jwt Bearer token</response>
    /// <response code="400">Input data is empty</response>
    /// <response code="404">User not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpPost]
    [AllowAnonymous]
    [Route("login")]
    [ProducesResponseType(typeof(JwtResponseModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> Login([FromBody] UserDto userDto)
    {
        if (userDto is null or {Password: null})
            return BadRequest(new ErrorModel("Input data is empty"));
        
        _logger.LogDebug("Login user with login = {login}", userDto.Login);

        var user = await _dbSet
            .Where(u => u.Email == userDto.Email || u.Login == userDto.Login)
            .Include(u => u.Role)
            .FirstAsync();
        
        if (user is null)
            return NotFound("User with the same login or email does not exist");
        
        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(user, user.Password, userDto.Password);
        var authClaims = new List<Claim>
        {
            new ("id", Strings.Trim($"{user.Id}")),
            new ("role", Strings.Trim(user.Role.Name)),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        
        var token = JwtTokenGeneratorHelper.GetToken(authClaims, _configuration);
        
        return result switch
        {
            PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded => Ok(
                    new JwtResponseModel()
                    {
                        UserId = user.Id,
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                        Expiration = token.ValidTo
                    }
                ),
            _ => BadRequest(new ErrorModel("User with the same login or email and password does not exist"))
        };
    }
}