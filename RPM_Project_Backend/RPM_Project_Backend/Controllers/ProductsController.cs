using System.Linq.Dynamic.Core;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPM_Project_Backend.Services.Database;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.JsonPatch;
using RPM_Project_Backend.Domain;
using RPM_Project_Backend.Helpers;
using RPM_Project_Backend.Models;

namespace RPM_Project_Backend.Controllers;

/// <inheritdoc />
[ApiController]
[ApiVersion("1.0")]
[Route("api/products")]
[EnableCors("MyPolicy")]
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly ApplicationContext _context;
    private readonly DbSet<Product> _dbSet;
    private readonly IMapper _mapper;

    /// <inheritdoc />
    public ProductsController(ILogger<ProductsController> logger, ApplicationContext context, IMapper mapper)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
        _dbSet = _context.Set<Product>();
    }

    /// <summary>
    /// Get products list
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// GET api/products
    /// 
    /// </remarks>
    /// <response code="200">Return products list</response>
    /// <response code="404">Products not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<Product>>> Get(
        [FromQuery] QueryParameters<ProductRequestDto> queryParameters)
    {
        _logger.LogDebug("Get list of products");
        
        var allProducts = _dbSet
                .Include(p => p.ProductsHaveGenres)
                .Include(p => p.Reviews)
                .Include(p => p.Seller)
                .Include(p => p.ProductsHaveProperties)
                .OrderBy(queryParameters.OrderBy, queryParameters.IsDescending())
                .AsQueryable();

        if (queryParameters.HasQuery())
        {
            try
            {
                var productQuery = (ProductRequestDto)queryParameters.Object;
                if (productQuery.Name is not null)
                    allProducts = allProducts
                        .Where(product => product.Name.Contains(productQuery.Name));
                if (productQuery.Rating is {Min: not null} and {Max: not null})
                    allProducts = allProducts
                        .Where(product => productQuery.Rating.Min <= product.Rating && product.Rating <= productQuery.Rating.Max); 
                if (productQuery.Cost is {Min: not null} and {Max: not null})
                    allProducts = allProducts
                        .Where(product => productQuery.Cost.Min <= product.Cost && product.Cost <= productQuery.Cost.Max);
                if (productQuery.Discount is not null)
                    allProducts = allProducts
                        .Where(product => product.Discount == productQuery.Discount);
                if (productQuery.GenreId is not null)
                    allProducts = allProducts
                        .Where(product => product.ProductsHaveGenres.Any(g => g.Id == productQuery.GenreId));
                if (productQuery.PropertyId is not null)
                    allProducts = allProducts
                        .Where(product => product.ProductsHaveProperties.Any(p => p.Id == productQuery.PropertyId));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        var paginationMetadata = new
        {
            totalCount = allProducts.Count(),
            pageSize = queryParameters.PageCount,
            currentPage = queryParameters.Page,
            totalPages = queryParameters.GetTotalPages(allProducts.Count())
        };

        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

        return await allProducts.CountAsync() switch
        {
            0 => NotFound(new ErrorModel("Products not found")),
            _ => Ok(
                allProducts
                    .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                    .Take(queryParameters.PageCount)
            )
        };
    }

    /// <summary>
    /// Get single product
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// GET api/products/11
    /// 
    /// </remarks>
    /// <param name="id"></param>
    /// <response code="200">Return product with specific id</response>
    /// <response code="404">Product not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<Product>> Get(long id)
    {
        if (id <= 0) 
            return BadRequest(new ErrorModel("The input data is empty"));
        
        _logger.LogDebug("Get product with id = {id}", id);

        var product = await _dbSet.FirstOrDefaultAsync(p => p.Id == id);

        return product switch
        {
            null => NotFound(new ErrorModel("Product not found")),
            _ => Ok(product)
        };
    }

    /// <summary>
    /// Create new product
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// POST api/products&#xA;&#xD;
    ///     {
    ///         "name": "SomeName",
    ///         "cost": 100,
    ///         "discount": 0,
    ///         "catId": 3
    ///     }
    /// 
    /// </remarks>
    /// <param name="productDto"></param>
    /// <response code="200">Return created product</response>
    /// <response code="400">Same product found</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpPost]
    [Authorize(Roles = "seller")]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<Product>> Post(ProductDto productDto)
    {
        if (productDto is null)
            return BadRequest();
        
        _logger.LogDebug("Create new product with name = {id}", productDto.Name);

        var sellerId = int.Parse(User.Claims.First(cl => cl.Type == "id").Value);

        var existedProduct = await _dbSet.FirstOrDefaultAsync(
                    p => p.Name == productDto.Name 
                 && p.SellerId == sellerId);
        if (existedProduct is not null)
            return BadRequest("Product with same name already exists");
        
        var product = _mapper.Map<Product>(productDto);

        product.SellerId = sellerId;
        product.Rating = 5;

        await _dbSet.AddAsync(product);

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(500, new ErrorModel("Some error has occurred")),
            _ => Ok(await _dbSet.FirstAsync(p => p.Name == productDto.Name && p.SellerId == sellerId))
        };
    }

    /// <summary>
    /// Update single product
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// PUT api/products&#xA;&#xD;
    ///     {
    ///         "id": 11
    ///         "name": "SomeName",
    ///         "cost": 100,
    ///         "discount": 0,
    ///         "quantity": 0,
    ///         "categoryId": 3
    ///     }
    /// 
    /// </remarks>
    /// <param name="product"></param>
    /// <response code="200">Return created user</response>
    /// <response code="400">The input data is empty</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">User not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpPut]
    [Authorize(Roles = "seller")]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<Product>> Put([FromBody] Product product)
    {
        var sellerId = int.Parse(User.Claims.First(cl => cl.Type == "id").Value);
        
        if (!User.Claims.Any(cl => cl.Type == "id" && cl.Value == $"{sellerId}"))
            return Unauthorized(new ErrorModel("Access is denied"));
        
        if (!_dbSet.Any(p => p.Id == product.Id))
            return NotFound(new ErrorModel("Product not found"));
        
        product.SellerId = sellerId;

        _logger.LogDebug("Update existing product with id = {id}", product.Id);
        _context.Entry(product).State = EntityState.Modified;

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(500,new ErrorModel("Some error has occurred")),
            _ => Ok(product)
        };
    }

    /// <summary>
    /// Patch single product
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// PATCH api/products/11&#xA;&#xD;
    ///     [
    ///         {
    ///             "op": "replace",
    ///             "path": "/name",
    ///             "value": "anotherName"
    ///         }
    ///     ]
    /// 
    /// </remarks>
    /// <param name="productPatch"></param>
    /// <param name="id"></param>
    /// <response code="200">Return changed product</response>
    /// <response code="400">Input data is empty</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Product not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpPatch("{id:int}")]
    [Authorize(Roles = "seller")]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<Product>> Patch([FromBody] JsonPatchDocument<Product> productPatch, int id)
    {
        if (productPatch is null || id <= 0)
            return BadRequest(new ErrorModel("Input data is empty"));

        var product = await _dbSet.FirstOrDefaultAsync(u => u.Id == id);

        if (product is null)
            return NotFound(new ErrorModel("Product not found"));

        if (!User.Claims.Any(cl => cl.Type == "id" && cl.Value == $"{product.SellerId}"))
            return Unauthorized(new ErrorModel("Access is denied"));

        _logger.LogDebug("Update existing product with id = {id}", id);

        productPatch.ApplyTo(product, ModelState);

        if (!ModelState.IsValid)
            return BadRequest(new ErrorModel("Model state is invalid"));

        _context.Entry(product).State = EntityState.Modified;

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(500,new ErrorModel("Some error has occurred")),
            _ => Ok(product)
        };
    }

    /// <summary>
    /// Delete single product
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// DELETE api/products/11
    /// 
    /// </remarks>
    /// <param name="id"></param>
    /// <response code="204">Delete product</response>
    /// <response code="400">The input data is empty</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Product not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [Authorize(Roles = "seller")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<Product>> Delete(long id)
    {
        if (id <= 0) 
            return BadRequest(new ErrorModel("The input data is empty"));
        
        var product = await _dbSet.FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
            return NotFound(new ErrorModel("Product not found"));
        
        if (!User.Claims.Any(cl => cl.Type == "id" && cl.Value == $"{product.SellerId}"))
            return Unauthorized(new ErrorModel("Access is denied"));
        
        _logger.LogDebug("Delete existing product with id = {id}", id);

        _dbSet.Remove(product);

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(StatusCodes.Status500InternalServerError,new ErrorModel("Some error has occurred")),
            _ => NoContent()
        };
    }
}