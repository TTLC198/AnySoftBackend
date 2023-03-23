using System.Text.Json;
using System.Linq.Dynamic.Core;
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

[ApiController]
[Route("api/users")]
[EnableCors("MyPolicy")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly ApplicationContext _context;
    private readonly DbSet<User> _dbSet;
    private readonly IMapper _mapper;

    public UsersController(ILogger<UsersController> logger, ApplicationContext context, IMapper mapper)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
        _dbSet = _context.Set<User>();
    }
    /// <summary>
    /// Get api/users
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<IEnumerable<User>>> Get([FromQuery]QueryParameters<User> queryParameters)
    {
        _logger.LogDebug("Get list of users");

        IQueryable<User> allUsers = 
            _dbSet.OrderBy(queryParameters.OrderBy, queryParameters.IsDescending());

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

        return allUsers.Count() switch
        {
            0 => NotFound(),
            _ => Ok(
                allUsers
                    .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                    .Take(queryParameters.PageCount)
            )
        };
    }
    /// <summary>
    /// Get api/users/{id}
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Roles = "user, admin, seller")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<User>> Get(int id)
    {
        _logger.LogDebug("Get user with id = {id}", id);

        if (!User.Claims.Any(cl => cl.Type == "id" && cl.Value == $"{id}"))
            return Unauthorized(new
            {
                message = "Access is denied"
            });
        
        var user = await _dbSet
            .FindAsync(id);

        return user switch
        {
            null => NotFound(),
            _ => Ok(user)
        };
    }

    /// <summary>
    /// Post api/users
    /// </summary>
    /// <param name="userFields"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<User>> Post([FromBody]UserDto userFields)
    {
        if (userFields is null)
            return BadRequest();
        
        _logger.LogDebug("Create new user with login = {login}", userFields.Login);

        var existedUser = await _dbSet.FirstOrDefaultAsync(u => u.Email == userFields.Email || u.Login == userFields.Login);
        if (existedUser is not null)
            return BadRequest("User with the same login or email already exists");

        var hasher = new PasswordHasher<User>();
        var user = _mapper.Map<User>(userFields);
        
        user.Password = hasher.HashPassword(user, user.Password);
        user.RoleId = 2; // Client

        await _dbSet.AddAsync(user);

        return await _context.SaveChangesAsync() switch
        {
            0 => BadRequest(),
            _ => Ok(userFields)
        };
    }

    /// <summary>
    /// Put api/users/
    /// </summary>
    /// <param name="userFields"></param>
    /// <returns></returns>
    [HttpPut]
    [Authorize(Roles = "user, admin, seller")]
    public async Task<ActionResult<User>> Put([FromBody]User userFields)
    {
        if (userFields is null)
            return BadRequest();
        
        if (!_dbSet.Any(u => u.Id == userFields.Id))
            return NotFound(userFields);
        
        if (!User.Claims.Any(cl => cl.Type == "id" && cl.Value == $"{userFields.Id}"))
            return Unauthorized(new
            {
                message = "Access is denied"
            });
        
        _logger.LogDebug("Update existing user with id = {id}", userFields.Id);
        _context.Entry(userFields).State = EntityState.Modified;

        return await _context.SaveChangesAsync() switch
        {
            0 => BadRequest(),
            _ => Ok(userFields)
        };
    }

    /// <summary>
    /// Patch api/users/
    /// </summary>
    /// <param name="userPatch"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPatch("{id:int}")]
    [Authorize(Roles = "user, admin, seller")]
    public async Task<ActionResult<User>> Patch([FromBody]JsonPatchDocument<User> userPatch, int id)
    {
        if (userPatch is null)
            return BadRequest();
        
        if (!User.Claims.Any(cl => cl.Type == "id" && cl.Value == $"{id}"))
            return Unauthorized(new
            {
                message = "Access is denied"
            });

        var user = await _dbSet.FirstOrDefaultAsync(u => u.Id == id);
        
        if (user is null) 
            return NotFound();
        
        _logger.LogDebug("Update existing user with id = {id}", id);
        
        userPatch.ApplyTo(user, ModelState);
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Entry(user).State = EntityState.Modified;

        return await _context.SaveChangesAsync() switch
        {
            0 => BadRequest(),
            _ => Ok(user)
        };
    }
    /// <summary>
    /// Delete api/users
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "user, admin, seller")]
    public async Task<ActionResult<User>> Delete(long id)
    {
        if (id == 0) 
            return BadRequest();
        
        _logger.LogDebug("Delete existing user with id = {id}", id);
        
        if (!User.Claims.Any(cl => cl.Type == "id" && cl.Value == $"{id}"))
            return Unauthorized(new
            {
                message = "Access is denied"
            });
        
        var toDelete = await _dbSet.FindAsync(id);
        
        if (toDelete is null) 
            return NotFound(id);

        var entityEntry = _dbSet.Remove(toDelete);

        return await _context.SaveChangesAsync() switch
        {
            0 => NotFound(),
            _ => Ok(entityEntry)
        };
    }
}