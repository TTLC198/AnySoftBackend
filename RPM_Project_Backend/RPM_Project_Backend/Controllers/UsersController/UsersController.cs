#nullable enable
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPM_PR_LIB;
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
    public async Task<ActionResult<IEnumerable<User>>> Get()
    {
        _logger.LogDebug("Get list of users");
        
        var users = await _dbSet
            .AsNoTracking()
            .Include(user => user.Addresses)
            .Include(user => user.Role)
            .ToListAsync()!;
        
        return users.Count() switch
        {
            0 => NotFound(),
            _ => Ok(users)
        };
    }
    /// <summary>
    /// Get api/users/{id}
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetById(int id)
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
    /// Get api/users/?id={id}
    /// </summary>
    /// <param name="userFields"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetByFieldsFromQuery([FromQuery]User? userFields)
    {
        if (userFields is null)
            return await Get();
        
        var user = await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u =>
                (userFields.Id == 0 || u.Id == userFields.Id) &&
                (userFields.Login == null || u.Login == userFields.Login) &&
                (userFields.Email == null || u.Email == userFields.Email)
            );
        
        _logger.LogDebug("Get user with query params");
        
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
    /// <param name="user"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<ActionResult<User>> Put([FromBody]User user)
    {
        if (!_dbSet.Any(u => u.Id == user.Id)) return NotFound(user);
        
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
    /// <param name="user"></param>
    /// <returns></returns>
    [HttpPatch]
    public async Task<ActionResult<User>> Patch([FromBody]User user)
    {
        if (!_dbSet.Any(u => u.Id == user.Id)) return NotFound(user);
        
        _logger.LogDebug("Update existing user with id = {id}", user.Id);
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
    [HttpDelete("{id}")]
    public async Task<ActionResult<User>> DeleteById(long id)
    {
        var toDelete = await _dbSet.FindAsync(id);
        if (toDelete is null) return NotFound(id);
        
        _logger.LogDebug("Delete existing user with id = {id}", id);

        var entityEntry = _dbSet.Remove(toDelete);

        return await _context.SaveChangesAsync() switch
        {
            0 => NotFound(),
            _ => Ok(entityEntry)
        };
    }

    /// <summary>
    /// Delete api/users
    /// </summary>
    /// <param name="userFields"></param>
    /// <returns></returns>
    [HttpDelete]
    public async Task<ActionResult<User>> DeleteFromQuery([FromQuery]User? userFields)
    {
        if (userFields is null)
            return NotFound(userFields);
        
        var toDelete = await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u =>
                (userFields.Id == 0 || u.Id == userFields.Id) &&
                (userFields.Login == null || u.Login == userFields.Login) &&
                (userFields.Email == null || u.Email == userFields.Email)
            );
        
        if (toDelete is null)
            return NotFound(userFields);

        _logger.LogDebug("Delete existing user with id = {id}", toDelete.Id);

        var entityEntry = _dbSet.Remove(toDelete);

        return await _context.SaveChangesAsync() switch
        {
            0 => NotFound(),
            _ => Ok(entityEntry)
        };
    }
}