using System.Text.Json;
using System.Linq.Dynamic.Core;
using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPM_PR_LIB;
using RPM_Project_Backend.Helpers;
using RPM_Project_Backend.Models;
using RPM_Project_Backend.Services.Database;

namespace RPM_Project_Backend.Controllers;

/// <inheritdoc />
[ApiController]
[ApiVersion("1.0")]
[Route("api/users")]
[EnableCors("MyPolicy")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly ApplicationContext _context;
    private readonly DbSet<User> _dbSet;
    private readonly IMapper _mapper;

    /// <inheritdoc />
    public UsersController(ILogger<UsersController> logger, ApplicationContext context, IMapper mapper)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
        _dbSet = _context.Set<User>();
    }
    /// <summary>
    /// Get users list
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// GET api/users
    /// 
    /// </remarks>
    /// <response code="200">Return users list</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Users not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpGet]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(IEnumerable<User>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<User>>> Get([FromQuery]QueryParameters<User> queryParameters)
    {
        _logger.LogDebug("Get list of users");

        IQueryable<User> allUsers = 
            _dbSet
                .Include(u => u.Addresses)
                .Include(u => u.Role)
                .OrderBy(queryParameters.OrderBy, queryParameters.IsDescending());

        if (queryParameters.HasQuery())
        {
            try
            {
                var userQuery = (User)queryParameters.Object;
                allUsers = allUsers.Where(u =>
                    u.Id == userQuery.Id ||
                    u.Email == userQuery.Email ||
                    u.Login == userQuery.Login
                );
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return StatusCode(500, new ErrorModel("Some error has occurred"));
            }
        }

        var paginationMetadata = new
        {
            totalCount = allUsers.Count(),
            pageSize = queryParameters.PageCount,
            currentPage = queryParameters.Page,
            totalPages = queryParameters.GetTotalPages(allUsers.Count())
        };

        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

        return await allUsers.CountAsync() switch
        {
            0 => NotFound(new ErrorModel("User not found")),
            _ => Ok(
                allUsers
                    .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                    .Take(queryParameters.PageCount)
            )
        };
    }
    /// <summary>
    /// Get single user
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// GET api/users/11
    /// 
    /// </remarks>
    /// <param name="id"></param>
    /// <response code="200">Return user with specific id</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">User not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpGet("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<User>> Get(int id)
    {
        _logger.LogDebug("Get user with id = {id}", id);
        
        var user = await _dbSet
            .Include(u => u.Addresses)
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user is null)
            return NotFound(new ErrorModel("User not found"));

        var userIdClaim = User.Claims.SingleOrDefault(cl => cl.Type == "id") ?? throw new InvalidOperationException("Invalid auth. Null id claims");
        var userRoleClaim = User.Claims.SingleOrDefault(cl => cl.Type.Contains("role")) ?? throw new InvalidOperationException("Invalid auth. Null role claims");

        if (!(userRoleClaim.Value == "admin" || userIdClaim.Value == $"{id}"))
            return Unauthorized(new ErrorModel("Access is denied"));

        return Ok(user);
    }

    /// <summary>
    /// Create new user (registration)
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// POST api/users&#xA;&#xD;
    ///     {
    ///         "login": "ttlc198",
    ///         "password": "M$4d3ikx+L",
    ///         "email": "ttlc198@gmail.com",
    ///         "roleId": 3
    ///     }
    /// 
    /// </remarks>
    /// <param name="userFields"></param>
    /// <response code="200">Return created user</response>
    /// <response code="400">Same user found</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<User>> Post([FromBody]UserDto userFields)
    {
        if (userFields is null)
            return BadRequest(new ErrorModel("Input data is empty"));
        
        _logger.LogDebug("Create new user with login = {login}", userFields.Login);

        var existedUser = await _dbSet.FirstOrDefaultAsync(u => u.Email == userFields.Email || u.Login == userFields.Login);
        if (existedUser is not null)
            return BadRequest("User with the same login or email already exists");

        var hasher = new PasswordHasher<User>();
        var user = _mapper.Map<User>(userFields);
        
        user.Password = hasher.HashPassword(user, user.Password);
        user.RoleId = 2; // Client by default

        await _dbSet.AddAsync(user);

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(500,new ErrorModel("Some error has occurred")),
            _ => Ok(await _dbSet.FirstAsync(u => u.Login == userFields.Login && u.Email == userFields.Password))
        };
    }

    /// <summary>
    /// Update single user
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// PUT api/users&#xA;&#xD;
    ///     {
    ///         "id": 3,
    ///         "login": "ttlc198",
    ///         "password": "M$4d3ikx+L1",
    ///         "email": "ttlc198@gmail.com",
    ///         "roleId": 3
    ///     }
    /// 
    /// </remarks>
    /// <param name="userFields"></param>
    /// <response code="200">Return changed user</response>
    /// <response code="400">Input data is empty</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">User not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpPut]
    [Authorize(Roles = "user, admin, seller")]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<User>> Put([FromBody]User userFields)
    {
        if (userFields is null)
            return BadRequest(new ErrorModel("Input data is empty"));
        
        if (!_dbSet.Any(u => u.Id == userFields.Id))
            return NotFound(new ErrorModel("User not found"));
        
        if (!User.Claims.Any(cl => cl.Type == "id" && cl.Value == $"{userFields.Id}"))
            return Unauthorized(new ErrorModel("Access is denied"));
        
        _logger.LogDebug("Update existing user with id = {id}", userFields.Id);
        _context.Entry(userFields).State = EntityState.Modified;

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(500,new ErrorModel("Some error has occurred")),
            _ => Ok(await _dbSet.FirstAsync(u => u.Login == userFields.Login && u.Email == userFields.Password))
        };
    }

    /// <summary>
    /// Patch single user
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// PATCH api/users/11&#xA;&#xD;
    ///     [
    ///         {
    ///             "op": "replace",
    ///             "path": "/login",
    ///             "value": "ttlc198"
    ///         }
    ///     ]
    /// 
    /// </remarks>
    /// <param name="userPatch"></param>
    /// <param name="id"></param>
    /// <response code="200">Return changed user</response>
    /// <response code="400">Input data is empty</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">User not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpPatch("{id:int}")]
    [Authorize(Roles = "user, admin, seller")]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<User>> Patch([FromBody]JsonPatchDocument<User> userPatch, int id)
    {
        if (userPatch is null)
            return BadRequest(new ErrorModel("Input data is empty"));
        
        if (!User.Claims.Any(cl => cl.Type == "id" && cl.Value == $"{id}"))
            return Unauthorized(new ErrorModel("Access is denied"));

        var user = await _dbSet.FirstOrDefaultAsync(u => u.Id == id);
        
        if (user is null) 
            return NotFound(new ErrorModel("User not found"));
        
        _logger.LogDebug("Update existing user with id = {id}", id);
        
        userPatch.ApplyTo(user, ModelState);
        
        if (!ModelState.IsValid)
            return BadRequest(new ErrorModel("Model state is invalid"));

        _context.Entry(user).State = EntityState.Modified;

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(500,new ErrorModel("Some error has occurred")),
            _ => Ok(user)
        };
    }
    /// <summary>
    /// Delete single user
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// DELETE api/users/11
    /// 
    /// </remarks>
    /// <param name="id"></param>
    /// <response code="204">Delete user</response>
    /// <response code="400">The input data is empty</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">User not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "user, admin, seller")]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<User>> Delete(long id)
    {
        if (id <= 0) 
            return BadRequest(new ErrorModel("The input data is empty"));
        
        _logger.LogDebug("Delete existing user with id = {id}", id);
        
        if (!User.Claims.Any(cl => cl.Type == "id" && cl.Value == $"{id}"))
            return Unauthorized(new ErrorModel("Access is denied"));
        
        var toDelete = await _dbSet.FindAsync(id);
        
        if (toDelete is null) 
            return NotFound(new ErrorModel("User not found"));

        _dbSet.Remove(toDelete);

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(500,new ErrorModel("Some error has occurred")),
            _ => NoContent()
        };
    }
}