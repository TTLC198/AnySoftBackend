﻿using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using AnySoftBackend.Helpers;
using AnySoftBackend.Library.DataTransferObjects.User;
using AnySoftBackend.Library.Misc;
using AnySoftBackend.Services.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using AnySoftBackend.Domain;
using AnySoftBackend.Models;

namespace AnySoftBackend.Controllers;

/// <inheritdoc />
[ApiController]
[ApiVersion("1.0")]
[Route("api/auth")]
public class AccountController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AccountController> _logger;
    private readonly ApplicationContext _context;

    /// <inheritdoc />
    public AccountController(
        IConfiguration configuration, ILogger<AccountController> logger, ApplicationContext context)
    {
        _configuration = configuration;
        _logger = logger;
        _context = context;
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
    /// <response code="200">Return Jwt Bearer token as string</response>
    /// <response code="400">Input data is empty</response>
    /// <response code="404">User not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpPost]
    [AllowAnonymous]
    [Route("login")]
    [ProducesResponseType(typeof(string), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> Login([FromBody] UserCreateDto userCreateDto)
    {
        if (userCreateDto is null or {Password: null})
            return BadRequest(new ErrorModel("Input data is empty"));

        _logger.LogDebug("Login user with login = {Login}", userCreateDto.Login);

        var user = await _context.Users
            .Where(u => u.Email == userCreateDto.Email || u.Login == userCreateDto.Login)
            .Include(u => u.Role)
            .FirstOrDefaultAsync();

        if (user is null)
            return NotFound(new ErrorModel("User with the same login or email does not exist"));

        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(user, user.Password!, userCreateDto.Password);
        var authClaims = new List<Claim>
        {
            new("id", Strings.Trim($"{user.Id}")),
            new("role", Strings.Trim(user.Role?.Name)),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = TokenGeneratorHelper.GenerateToken(authClaims, _configuration);

        return result switch
        {
            PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded => Ok(
                new JwtSecurityTokenHandler().WriteToken(token)
            ),
            _ => BadRequest(new ErrorModel("User with the same login or email and password does not exist"))
        };
    }
}