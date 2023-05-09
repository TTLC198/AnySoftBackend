using System.Linq.Dynamic.Core;
using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
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
[Route("api/carts")]
[EnableCors("MyPolicy")]
public class ShoppingCartsController : ControllerBase
{
    private readonly ILogger<ShoppingCartsController> _logger;
    private readonly ApplicationContext _context;
    private readonly IMapper _mapper;

    /// <inheritdoc />
    public ShoppingCartsController(ILogger<ShoppingCartsController> logger, ApplicationContext context, IMapper mapper)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
    }
    
    /// <summary>
    /// Create new shopping cart
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// POST api/carts&#xA;&#xD;
    ///     {
    ///         "productIds":
    ///         [
    ///             1, 2, 3
    ///         ]
    ///     }
    /// 
    /// </remarks>
    /// <param name="shoppingCartDto"></param>
    /// <response code="200">Return created shopping cart</response>
    /// <response code="400">There are no products with this ID</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ShoppingCartResponseDto), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(void), (int) HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<ShoppingCartResponseDto>> Post(ShoppingCartDto shoppingCartDto)
    {
        if (shoppingCartDto is null)
            return BadRequest(new ErrorModel("The input data is empty"));
        
        var userId = int.Parse(User.Claims.First(cl => cl.Type == "id").Value);
        if (!await _context.Products.AnyAsync(p => shoppingCartDto.ProductIds.Any(c => c == p.Id)))
            return BadRequest(new ErrorModel("There are no products with this ID"));

        _logger.LogDebug("Create new shopping cart with user id = {userId}", userId);

        var usersHaveProducts = new List<UsersHaveProducts>();
        foreach (var productId in shoppingCartDto.ProductIds)
        {
            var existedItem = await _context.UsersHaveProducts.FirstOrDefaultAsync(c => c.ProductId == productId);
            if (existedItem is not null)
                existedItem.Quantity++;
            else
                usersHaveProducts.Add(new UsersHaveProducts()
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = 1
                });
        }

        await _context.UsersHaveProducts.AddRangeAsync(usersHaveProducts);
        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(500, new ErrorModel("Some error has occurred")),
            _ => Ok(new ShoppingCartResponseDto()
            {
                Products = _context.Products
                    .Include(p => p.ProductsHaveGenres)
                    .ThenInclude(phg => phg.Genre)
                    .Include(p => p.Seller)
                    .ThenInclude(s => s.Images)
                    .Include(p => p.Images)
                    .Include(p => p.ProductsHaveProperties)
                    .ThenInclude(php => php.Property)
                    .Where(p => shoppingCartDto.ProductIds.Any(c => p.Id == c))
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
                            Image = p.Seller.Images == null
                                ? ""
                                : ImageUriHelper.GetImagePathAsUri(
                                    (p.Seller.Images.FirstOrDefault() ?? new Image()).ImagePath)
                        },
                        Images = (p.Images ?? new List<Image>())
                            .Select(i => ImageUriHelper.GetImagePathAsUri(i.ImagePath))
                            .ToList(),
                        Properties = (p.ProductsHaveProperties ?? new List<ProductsHaveProperties>())
                            .Select(php => php.Property)
                            .ToList(),
                        Genres = (p.ProductsHaveGenres ?? new List<ProductsHaveGenres>())
                            .Select(phg => phg.Genre)
                            .ToList()
                    })
            })
        };
    }
    
    /// <summary>
    /// Delete single product from cart
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// DELETE api/carts?productId=1
    /// 
    /// </remarks>
    /// <param name="id"></param>
    /// <param name="productId"></param>
    /// <response code="204">Delete genre</response>
    /// <response code="400">The input data is empty</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Genre not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [Authorize]
    [HttpDelete("delete")]
    [ProducesResponseType(typeof(void), (int) HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult> Delete(
        [FromQuery] int productId)
    {
        if (productId <= 0)
            return BadRequest(new ErrorModel("The input data is empty"));

        var userId = int.Parse(User.Claims.First(cl => cl.Type == "id").Value);
        var usersHaveProducts = await _context.UsersHaveProducts.FirstOrDefaultAsync(c => c.ProductId == productId && c.UserId == userId);
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (usersHaveProducts is null)
            return NotFound(new ErrorModel("Product in cart not found"));
        if (product is null)
            return NotFound(new ErrorModel("Product not found"));
        
        _logger.LogDebug("Remove product with id = {id} from shopping cart", productId);

        _context.UsersHaveProducts.Remove(usersHaveProducts);

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel("Some error has occurred")),
            _ => NoContent()
        };
    }
}