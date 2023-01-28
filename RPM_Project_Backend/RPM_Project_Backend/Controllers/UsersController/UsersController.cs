using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using RPM_PR_LIB;
using RPM_Project_Backend.Repositories;

namespace RPM_Project_Backend.Controllers.UsersController;

[ApiController]
[Route("api/[controller]")]
[EnableCors("MyPolicy")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IBaseRepository<User> _usersRepository;

    public UsersController(ILogger<UsersController> logger, IBaseRepository<User> usersRepository)
    {
        _logger = logger;
        _usersRepository = usersRepository;
    }
    /// <summary>
    /// Get api/users
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> Get()
    {
        _logger.LogDebug("Get list of users");
        var users = await _usersRepository.GetAllAsync();
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
    public async Task<ActionResult<User>> Get(long id)
    {
        _logger.LogDebug("Get user with id = {id}", id);
        var user = await _usersRepository.GetAsync(id);
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
    public async Task<ActionResult<User>> Post(User user)
    {
        _logger.LogDebug("Create new user with id = {id}", user.Id);
        return user switch
        {
            null => BadRequest(),
            _ => Ok(await _usersRepository.CreateAsync(user))
        };
    }
    /// <summary>
    /// Put api/users/
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<ActionResult<User>> Put(User user)
    {
        _logger.LogDebug("Update existing user with id = {id}", user.Id);
        return user switch
        {
            null => BadRequest(),
            _ when _usersRepository.GetAllAsync().Result.All(u => u.Id != user.Id) => NotFound(),
            _ => Ok(await _usersRepository.UpdateAsync(user))
        };
    }
    /// <summary>
    /// Delete api/users
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<User>> Delete(long id)
    {
        _logger.LogDebug("Delete existing user with id = {id}", id);
        return await _usersRepository.GetAsync(id) switch
        {
            null => NotFound(),
            _ => Ok(await _usersRepository.DeleteAsync(id))
        };
    }
}