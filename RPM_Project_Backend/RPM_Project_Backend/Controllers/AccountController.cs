using System.IdentityModel.Tokens.Jwt;
using System.Linq.Dynamic.Core;
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
using RPM_Project_Backend.Models;
using RPM_Project_Backend.Services.Database;

namespace RPM_Project_Backend.Controllers;

[ApiController]
[Route("api/auth")]
public class AccountController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AccountController> _logger;
    private readonly ApplicationContext _context;
    private readonly DbSet<User> _dbSet;

    public AccountController(
        IConfiguration configuration, ILogger<AccountController> logger, ApplicationContext context)
    {
        _configuration = configuration;
        _logger = logger;
        _context = context;
        _dbSet = _context.Set<User>();
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] UserDto userDto)
    {
        if (userDto is null)
            return BadRequest();
        
        _logger.LogDebug("Login user with login = {login}", userDto.Login);

        var user = await _dbSet
            .Where(u => u.Email == userDto.Email || u.Login == userDto.Login)
            .Include(u => u.Role)
            .FirstAsync();
        
        if (user is null)
            return BadRequest("User with the same login or email does not exist");
        
        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(user, user.Password, userDto.Password);
        var authClaims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, Strings.Trim(user.Login)),
            new ("role", user.Role.Name),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        
        var token = GetToken(authClaims);
        
        return result switch
        {
            PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded => Ok(
                    new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    }
                ),
            _ => BadRequest("Wrong password")
        };
    }

    private JwtSecurityToken GetToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtConfiguration:Secret"] ?? throw new InvalidOperationException()));

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtConfiguration:Issuer"],
            audience: _configuration["JwtConfiguration:Audience"],
            expires: DateTime.UtcNow.Add(TimeSpan.FromMilliseconds(Int64.Parse(_configuration["JwtConfiguration:Expiration"]))),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }
}