using System.Text.Json;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPM_PR_LIB;
using RPM_Project_Backend.Helpers;
using RPM_Project_Backend.Models;
using RPM_Project_Backend.Services.Database;

namespace RPM_Project_Backend.Controllers.UsersController;

[ApiController]
[Route("api/[controller]")]
[EnableCors("MyPolicy")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly ApplicationContext _context;
    private readonly DbSet<User> _dbSet;

    public UsersController(ILogger<UsersController> logger, ApplicationContext context)
    {
        _logger = logger;
        _context = context;
        _dbSet = _context.Set<User>();
    }
    /// <summary>
    /// Get api/users
    /// </summary>
    /// <returns></returns>
    [HttpGet]
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
    [HttpGet("{id:int}")]
    public async Task<ActionResult<User>> Get(int id)
    {
        _logger.LogDebug("Get user with id = {id}", id);
        
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
    /// <param name="user"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<User>> Post([FromBody]User user)
    {
        _logger.LogDebug("Create new user with id = {id}", user.Id);

        if (user is null)
            return BadRequest();
        
        await _dbSet.AddAsync(user);

        return await _context.SaveChangesAsync() switch
        {
            0 => BadRequest(),
            _ => Ok(user)
        };
    }

    /// <summary>
    /// Put api/users/
    /// </summary>
    /// <param name="userDto"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<ActionResult<User>> Put([FromBody]User user)
    {
        if (!_dbSet.Any(u => u.Id == user.Id))
            return NotFound(user);
        
        _logger.LogDebug("Update existing user with id = {id}", user.Id);
        _context.Entry(user).State = EntityState.Modified;

        return await _context.SaveChangesAsync() switch
        {
            0 => BadRequest(),
            _ => Ok(user)
        };
    }

    /// <summary>
    /// Patch api/users/
    /// </summary>
    /// <param name="userPatch"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPatch("{id:int}")]
    public async Task<ActionResult<User>> Patch([FromBody]JsonPatchDocument<User> userPatch, int id)
    {
        if (userPatch is null)
            return BadRequest();

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
    public async Task<ActionResult<User>> Delete(long id)
    {
        var toDelete = await _dbSet.FindAsync(id);
        if (toDelete is null) 
            return NotFound(id);
        
        _logger.LogDebug("Delete existing user with id = {id}", id);

        var entityEntry = _dbSet.Remove(toDelete);

        return await _context.SaveChangesAsync() switch
        {
            0 => NotFound(),
            _ => Ok(entityEntry)
        };
    }
}