using System.Text.Json;
using System.Linq.Dynamic.Core;
using System.Net;
using AnySoftBackend.Library.DataTransferObjects;
using AnySoftBackend.Library.DataTransferObjects.Order;
using AnySoftBackend.Library.DataTransferObjects.Property;
using AnySoftBackend.Library.DataTransferObjects.User;
using AnySoftBackend.Library.Misc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPM_Project_Backend.Domain;
using RPM_Project_Backend.Helpers;
using RPM_Project_Backend.Models;
using RPM_Project_Backend.Services.Database;

namespace RPM_Project_Backend.Controllers;

/// <inheritdoc />
[ApiController]
[ApiVersion("1.0")]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly ApplicationContext _context;
    private readonly IMapper _mapper;

    /// <inheritdoc />
    public UsersController(ILogger<UsersController> logger, ApplicationContext context, IMapper mapper)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
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
    [ProducesResponseType(typeof(IEnumerable<User>), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(void), (int) HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<User>>> Get([FromQuery] QueryParameters<User> queryParameters)
    {
        _logger.LogDebug("Get list of users");

        IQueryable<User> allUsers =
            _context.Users
                .Include(u => u.Role)
                .Include(u => u.Images)
                .Include(u => u.Orders)
                .Include(u => u.Payments)
                .OrderBy(queryParameters.OrderBy, queryParameters.IsDescending());

        if (queryParameters.HasQuery())
        {
            try
            {
                var userQuery = (User) queryParameters.Object;
                allUsers = allUsers.Where(u =>
                    u.Id == userQuery.Id ||
                    u.Email == userQuery.Email ||
                    u.Login == userQuery.Login
                );
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel("Some error has occurred"));
            }
        }

        var paginationMetadata = new
        {
            totalCount = allUsers.Count(),
            pageSize = allUsers.Count() < queryParameters.PageCount
                ? allUsers.Count()
                : queryParameters.PageCount,
            currentPage = queryParameters.GetTotalPages(allUsers.Count()) < queryParameters.Page
                ? (int) queryParameters.GetTotalPages(allUsers.Count())
                : queryParameters.Page,
            totalPages = queryParameters.GetTotalPages(allUsers.Count())
        };

        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

        if (queryParameters is {Page: <= 0} or {PageCount: <= 0})
            return NotFound(new ErrorModel("Users not found"));

        return await allUsers.CountAsync() switch
        {
            0 => NotFound(new ErrorModel("Users not found")),
            _ => Ok(
                allUsers
                    .Skip(paginationMetadata.pageSize * (paginationMetadata.currentPage - 1))
                    .Take(paginationMetadata.pageSize)
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
    [ProducesResponseType(typeof(UserResponseDto), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<UserResponseDto>> Get(int id)
    {
        _logger.LogDebug("Get user with id = {Id}", id);

        var user = await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Images)
            .Include(u => u.UsersHaveProducts)
            .Include(u => u.Orders)
            .ThenInclude(o => o.OrdersHaveProducts)
            .Include(u => u.Payments)
            .FirstOrDefaultAsync(u => u.Id == id);
        
        if (user is null)
            return NotFound(new ErrorModel("User not found"));

        var userIdClaim = User.Claims.SingleOrDefault(cl => cl.Type == "id") ??
                          throw new InvalidOperationException("Invalid auth. Null id claims");
        var userRoleClaim = User.Claims.SingleOrDefault(cl => cl.Type.Contains("role")) ??
                            throw new InvalidOperationException("Invalid auth. Null role claims");

        if (!(userRoleClaim.Value == "admin" || userIdClaim.Value == $"{id}"))
            return Unauthorized(new ErrorModel("Access is denied"));

        var productsInCart = _context.Products
            .Include(p => p.ProductsHaveGenres)
            .ThenInclude(phg => phg.Genre)
            .Include(p => p.Seller)
            .ThenInclude(s => s.Images)
            .Include(p => p.Images)
            .Include(p => p.ProductsHaveProperties)
            .ThenInclude(php => php.Property)
            .ToList()
            .Where(p => (user.UsersHaveProducts ?? Array.Empty<UsersHaveProducts>())
                .Any(uhp => uhp.ProductId == p.Id));

        return Ok(new UserResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            Login = user.Login,
            Image = ImageUriHelper.GetImagePathAsUri((user.Images!.FirstOrDefault() ?? new Image()).ImagePath),
            Orders = (user.Orders ?? new List<Order>())
                .Select(o => new OrderResponseDto()
                        {
                            Id = o.Id,
                            FinalCost = o.FinalCost,
                            Status = o.Status,
                            Ts = o.Ts,
                            UserId = o.UserId,
                            PurchasedProductsIds = o.OrdersHaveProducts?
                                .Select(ohp => ohp.ProductId)
                                .ToList()!
                        })
                .OrderBy(order => order.Id)
                .ToList(),
            ShoppingCart = productsInCart
                .Select(p => new ProductResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Cost = p.Cost,
                    Discount = p.Discount,
                    Rating = p.Rating,
                    Ts = p.Ts,
                    Seller = new UserResponseDto()
                    {
                        Id = p.Seller!.Id,
                        Login = p.Seller.Login,
                        Image = p.Seller.Images is null
                            ? ""
                            : ImageUriHelper.GetImagePathAsUri(
                                (p.Seller.Images.FirstOrDefault() ?? new Image()).ImagePath)
                    },
                    Images = (p.Images ?? new List<Image>())
                        .Select(i => ImageUriHelper.GetImagePathAsUri(i.ImagePath))
                        .ToList(),
                    Properties = (p.ProductsHaveProperties ?? new List<ProductsHaveProperties>())
                        .Select(php => _mapper.Map<PropertyDto>(php.Property))
                        .ToList(),
                    Genres = (p.ProductsHaveGenres ?? new List<ProductsHaveGenres>())
                        .Select(phg => _mapper.Map<GenreDto>(phg.Genre))
                        .ToList()
                })
                .OrderBy(product => product.Id)
                .ToList()
        });
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
    /// <param name="userCreateFields"></param>
    /// <response code="201">Return created user</response>
    /// <response code="400">Same user found</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserResponseDto), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<User>> Post([FromBody] UserCreateDto userCreateFields)
    {
        if (userCreateFields is null)
            return BadRequest(new ErrorModel("Input data is empty"));

        _logger.LogDebug("Create new user with login = {Login}", userCreateFields.Login);

        var existedUser =
            await _context.Users.FirstOrDefaultAsync(u => u.Email == userCreateFields.Email || u.Login == userCreateFields.Login);
        if (existedUser is not null)
            return BadRequest(new ErrorModel("User with the same login or email already exists"));

        var hasher = new PasswordHasher<User>();
        var user = _mapper.Map<User>(userCreateFields);

        user.Password = hasher.HashPassword(user, user.Password ?? string.Empty);
        user.RoleId = 2; // Client by default

        var createdUser = await _context.Users.AddAsync(user);

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel("Some error has occurred")),
            _ => StatusCode(StatusCodes.Status201Created,
                new UserResponseDto
                {
                    Id = createdUser.Entity.Id,
                    Email = createdUser.Entity.Email,
                    Login = createdUser.Entity.Login
                })
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
    [ProducesResponseType(typeof(User), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<User>> Put([FromBody] UserEditDto userFields)
    {
        if (userFields is null)
            return BadRequest(new ErrorModel("Input data is empty"));

        if (!_context.Users.Any(u => u.Id == userFields.Id))
            return NotFound(new ErrorModel("User not found"));

        if (!User.Claims.Any(cl => cl.Type == "id" && cl.Value == $"{userFields.Id}"))
            return Unauthorized(new ErrorModel("Access is denied"));

        _logger.LogDebug("Update existing user with id = {Id}", userFields.Id);

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userFields.Id);
        
        if (user is null)
            return BadRequest(new ErrorModel("User does not exist"));

        var hasher = new PasswordHasher<User>();
        
        user.Login = userFields.Login;
        user.Email = userFields.Email;
        if (userFields.Password is not null)
            user.Password = hasher.HashPassword(user, userFields.Password);
        
        _context.Entry(user).State = EntityState.Modified;

        switch (await _context.SaveChangesAsync())
        {
            case 0:
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel("Some error has occurred"));
            default:
                var createdUser =
                    await _context.Users.FirstOrDefaultAsync(u => u.Login == userFields.Login && u.Email == userFields.Email);
                return createdUser is null
                    ? StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel("Some error has occurred"))
                    : Ok(createdUser);
        }
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
    [ProducesResponseType(typeof(User), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<User>> Patch([FromBody] JsonPatchDocument<User> userPatch, int id)
    {
        if (userPatch is null)
            return BadRequest(new ErrorModel("Input data is empty"));

        if (!User.Claims.Any(cl => cl.Type == "id" && cl.Value == $"{id}"))
            return Unauthorized(new ErrorModel("Access is denied"));

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user is null)
            return NotFound(new ErrorModel("User not found"));

        _logger.LogDebug("Update existing user with id = {Id}", id);

        userPatch.ApplyTo(user, ModelState);

        if (!ModelState.IsValid)
            return BadRequest(new ErrorModel("Model state is invalid"));

        _context.Entry(user).State = EntityState.Modified;

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel("Some error has occurred")),
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
    [ProducesResponseType(typeof(void), (int) HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<User>> Delete(long id)
    {
        if (id <= 0)
            return BadRequest(new ErrorModel("The input data is empty"));

        _logger.LogDebug("Delete existing user with id = {Id}", id);

        if (!User.Claims.Any(cl => cl.Type == "id" && cl.Value == $"{id}"))
            return Unauthorized(new ErrorModel("Access is denied"));

        var toDelete = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (toDelete is null)
            return NotFound(new ErrorModel("User not found"));

        _context.Users.Remove(toDelete);

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel("Some error has occurred")),
            _ => NoContent()
        };
    }
}