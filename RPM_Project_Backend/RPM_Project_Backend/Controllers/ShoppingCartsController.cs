using System.Linq.Dynamic.Core;
using System.Net;
using System.Text.Json;
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
[Route("api/cart")]
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
    /// Get products list in cart
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// GET api/cart
    /// 
    /// </remarks>
    /// <response code="200">Return products list</response>
    /// <response code="404">Products not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<ProductResponseDto>), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProductResponseDto>>> Get(
        [FromQuery] QueryParameters<object> queryParameters)
    {
        _logger.LogDebug("Get list of product in cart");

        var userId = int.Parse(User.Claims.First(cl => cl.Type == "id").Value);

        var usersHaveProducts = _context.UsersHaveProducts
            .Where(uhp => uhp.UserId == userId);
        
        var products = _context.Products
            .Where(p => usersHaveProducts.Any(uhp => uhp.ProductId == p.Id))
            .Include(p => p.ProductsHaveGenres)
            .ThenInclude(phg => phg.Genre)
            .Include(p => p.Seller)
            .ThenInclude(s => s.Images)
            .Include(p => p.Images)
            .Include(p => p.ProductsHaveProperties)
            .ThenInclude(php => php.Property)
            .OrderBy(queryParameters.OrderBy, queryParameters.IsDescending())
            .ToList();

        var paginationMetadata = new
        {
            totalCount = products.Count,
            pageSize = products.Count < queryParameters.PageCount
                ? products.Count
                : queryParameters.PageCount,
            currentPage = queryParameters.GetTotalPages(products.Count) < queryParameters.Page
                ? (int)queryParameters.GetTotalPages(products.Count)
                : queryParameters.Page,
            totalPages = queryParameters.GetTotalPages(products.Count)
        };

        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
        
        if (queryParameters is {Page: <= 0} or {PageCount: <= 0})
            return NotFound(new ErrorModel("Products not found"));

        return products.Count switch
        {
            0 => NotFound(new ErrorModel("Products not found")),
            _ => Ok(
                products
                    .Skip(paginationMetadata.pageSize * (paginationMetadata.currentPage - 1))
                    .Take(paginationMetadata.pageSize)
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
                            Image = p.Seller.Images is not null
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
            )
        };
    }
    
    /// <summary>
    /// Create new order
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// POST api/cart/order&#xA;&#xD;
    /// 
    /// </remarks>
    /// <response code="200">Return created shopping cart</response>
    /// <response code="400">There are no products with this ID</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpPost("order")]
    [Authorize]
    [ProducesResponseType(typeof(ShoppingCartResponseDto), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(void), (int) HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<ShoppingCartResponseDto>> Order()
    {
        var userId = int.Parse(User.Claims.First(cl => cl.Type == "id").Value);
        if (!await _context.UsersHaveProducts.AnyAsync(p => p.UserId == userId))
            return NotFound(new ErrorModel("User's cart is empty"));

        var userHaveProducts = _context.UsersHaveProducts
            .Where(c => c.UserId == userId)
            .ToList();

        var products = _context.Products
            .ToList()
            .Where(p => userHaveProducts.Any(uhp => uhp.ProductId == p.Id));

        var order = new Order()
        {   
            UserId = userId,
            Status = "new",
            FinalCost = products
                .Sum(p => p.Cost * (1 - (p.Discount ?? 0) * 0.01)),
            Ts = DateTime.UtcNow
        };

        var createdOrder = await _context.Orders.AddAsync(order);

        switch (await _context.SaveChangesAsync())
        {
            case 0:
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel("Some error has occurred"));
            default:
                var ordersHaveProducts = userHaveProducts
                        .Select(product => 
                            new OrdersHaveProduct
                            {
                                OrderId = createdOrder.Entity.Id, 
                                ProductId = product.ProductId
                            }).ToList();

                _context.UsersHaveProducts.RemoveRange(userHaveProducts);
                await _context.OrdersHaveProducts.AddRangeAsync(ordersHaveProducts);
                
                return await _context.SaveChangesAsync() switch
                {
                    0 => StatusCode(StatusCodes.Status500InternalServerError,
                        new ErrorModel("Some error has occurred")),
                    _ => Ok(_mapper.Map<OrderResponseDto>(createdOrder.Entity))
                };
        }
    }
    
    /// <summary>
    /// Create new shopping cart
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// POST api/cart&#xA;&#xD;
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
            return NotFound(new ErrorModel("There are no products with this ID"));

        _logger.LogDebug("Create new shopping cart with user id = {userId}", userId);

        var usersHaveProducts = new List<UsersHaveProducts>();
        foreach (var productId in shoppingCartDto.ProductIds)
        {
            var existedItem = await _context.UsersHaveProducts.FirstOrDefaultAsync(c => c.ProductId == productId && c.UserId == userId);
            if (existedItem is not null)
                return BadRequest(new ErrorModel("The product is already in the cart"));
            usersHaveProducts.Add(new UsersHaveProducts()
            {
                UserId = userId,
                ProductId = productId
            });
        }

        await _context.UsersHaveProducts.AddRangeAsync(usersHaveProducts);
        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel("Some error has occurred")),
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
                    .ToList()
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
                            Image = p.Seller.Images is not null
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
    /// DELETE api/cart?productId=1
    /// 
    /// </remarks>
    /// <param name="productId"></param>
    /// <response code="204">Delete genre</response>
    /// <response code="400">The input data is empty</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Genre not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [Authorize]
    [HttpDelete]
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
        var usersHaveProducts = await _context.UsersHaveProducts
            .FirstOrDefaultAsync(c => c.ProductId == productId && c.UserId == userId);
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