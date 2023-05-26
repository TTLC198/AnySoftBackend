using System.Linq.Dynamic.Core;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPM_Project_Backend.Services.Database;
using System.Text.Json;
using AnySoftBackend.Library.DataTransferObjects;
using AnySoftBackend.Library.DataTransferObjects.Product;
using AnySoftBackend.Library.DataTransferObjects.Property;
using AnySoftBackend.Library.DataTransferObjects.Review;
using AnySoftBackend.Library.DataTransferObjects.User;
using AnySoftBackend.Library.Misc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using RPM_Project_Backend.Domain;
using RPM_Project_Backend.Helpers;
using RPM_Project_Backend.Models;

namespace RPM_Project_Backend.Controllers;

/// <inheritdoc />
[ApiController]
[ApiVersion("1.0")]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly ApplicationContext _context;
    private readonly IMapper _mapper;

    /// <inheritdoc />
    public ProductsController(ILogger<ProductsController> logger, ApplicationContext context, IMapper mapper)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
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
    [ProducesResponseType(typeof(IEnumerable<ProductResponseDto>), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProductResponseDto>>> Get(
        [FromQuery] QueryParameters<ProductRequestDto> queryParameters)
    {
        _logger.LogDebug("Get list of products");

        var products = _context.Products
            .Include(p => p.ProductsHaveGenres)
            .ThenInclude(phg => phg.Genre)
            .Include(p => p.Seller)
            .ThenInclude(s => s.Images)
            .Include(p => p.Images)
            .Include(p => p.ProductsHaveProperties)
            .ThenInclude(php => php.Property)
            .OrderBy(queryParameters.OrderBy, queryParameters.IsDescending())
            .ToList();

        if (queryParameters.HasQuery())
        {
            try
            {
                var productQuery = (ProductRequestDto) queryParameters.Object;
                if (productQuery.Ids is {Count: > 0})
                    products = products
                        .Where(product => productQuery.Ids
                            .Contains(product.Id))
                        .ToList();
                if (productQuery.Name is not null)
                    products = products
                        .Where(product => product.Name!.ToLower().Contains(productQuery.Name.ToLower()))
                        .ToList();
                if (productQuery.Rating is {Min: not null} and {Max: not null})
                    products = products
                        .Where(product =>
                            productQuery.Rating.Min <= product.Rating && product.Rating <= productQuery.Rating.Max)
                        .ToList();
                if (productQuery.Cost is {Min: not null} and {Max: not null})
                    products = products
                        .Where(product =>
                            productQuery.Cost.Min <= product.Cost && product.Cost <= productQuery.Cost.Max)
                        .ToList();
                if (productQuery.Discount is {Min: not null} and {Max: not null})
                    products = products
                        .Where(product =>
                            productQuery.Discount.Min <= product.Discount &&
                            product.Discount <= productQuery.Discount.Max)
                        .ToList();
                if (productQuery.PublicationDate is {Min: not null} and {Max: not null})
                    products = products
                        .Where(product =>
                            productQuery.PublicationDate.Min <= product.Ts &&
                            product.Ts <= productQuery.PublicationDate.Max)
                        .ToList();
                if (productQuery.Genres is {Count: > 0})
                    products = products
                        .Where(product => productQuery.Genres
                            .All(g => product.ProductsHaveGenres!.Any(phg => phg.GenreId == g)))
                        .ToList();
                if (productQuery.Properties is {Count: > 0})
                    products = products
                        .Where(product => productQuery.Properties!
                            .All(p => product.ProductsHaveProperties!.Any(php => php.PropertyId == p)))
                        .ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

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

        return products.Count() switch
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
    [ProducesResponseType(typeof(ProductResponseDto), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<ProductResponseDto>> Get(int id)
    {
        if (id < 0)
            return BadRequest(new ErrorModel("The input data is empty"));

        _logger.LogDebug("Get product with id = {Id}", id);

        var products = _context.Products
            .Include(p => p.ProductsHaveGenres)
            .ThenInclude(phg => phg.Genre)
            .Include(p => p.Reviews)
            .ThenInclude(r => r.User)
            .ThenInclude(u => u.Images)
            .Include(p => p.Seller)
            .Include(p => p.ProductsHaveProperties)
            .ThenInclude(php => php.Property)
            .AsQueryable();

        return await products.AnyAsync(p => p.Id == id) switch
        {
            false => NotFound(new ErrorModel("Product not found")),
            _ => Ok(
                products
                    .Select(p => new ProductResponseDto()
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
                            Id = p.Seller.Id,
                            Login = p.Seller.Login,
                            Image = ImageUriHelper.GetImagePathAsUri(p.Seller.Images.FirstOrDefault().ImagePath)
                        },
                        Images = p.Images
                            .Select(i => ImageUriHelper.GetImagePathAsUri(i.ImagePath))
                            .ToList(),
                        Reviews = p.Reviews
                            .Select(r => new ReviewResponseDto()
                            {
                                Id = r.Id,
                                Text = r.Text,
                                Grade = r.Grade,
                                Ts = r.Ts,
                                ProductId = r.ProductId,
                                User = new UserResponseDto()
                                {
                                    Id = r.User.Id,
                                    Login = r.User.Login,
                                    Image = ImageUriHelper.GetImagePathAsUri(r.User.Images.FirstOrDefault().ImagePath)
                                }
                            }),
                        Properties = p.ProductsHaveProperties
                            .Select(php => _mapper.Map<PropertyDto>(php.Property))
                            .ToList(),
                        Genres = p.ProductsHaveGenres
                            .Select(phg => _mapper.Map<GenreDto>(phg.Genre))
                            .ToList()
                    })
                    .FirstOrDefault(p => p.Id == id))
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
    /// <param name="productCreateDto"></param>
    /// <response code="200">Return created product</response>
    /// <response code="400">Same product found</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpPost]
    [Authorize(Roles = "seller")]
    [ProducesResponseType(typeof(Product), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(void), (int) HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<Product>> Post(ProductCreateDto productCreateDto)
    {
        if (productCreateDto is null)
            return BadRequest(new ErrorModel("The input data is empty"));
        
        if (!ModelState.IsValid)
            return BadRequest(new ErrorModel("Model state is invalid"));

        _logger.LogDebug("Create new product with name = {Id}", productCreateDto.Name);

        var sellerId = int.Parse(User.Claims.First(cl => cl.Type == "id").Value);

        var existedProduct = await _context.Products.FirstOrDefaultAsync(
            p => p.Name == productCreateDto.Name
                 && p.SellerId == sellerId);
        if (existedProduct is not null)
            return BadRequest("Product with same name already exists");

        var product = _mapper.Map<Product>(productCreateDto);

        product.Ts = DateTime.UtcNow;
        product.SellerId = sellerId;
        product.Rating = 5;

        var createdProduct = await _context.Products.AddAsync(product);

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(500, new ErrorModel("Some error has occurred")),
            _ => Ok(createdProduct.Entity)
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
    /// <response code="200">Return created product</response>
    /// <response code="400">The input data is empty</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Product not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpPut]
    [Authorize(Roles = "seller")]
    [ProducesResponseType(typeof(Product), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(void), (int) HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<Product>> Put([FromBody] Product product)
    {
        var sellerId = int.Parse(User.Claims.First(cl => cl.Type == "id").Value);

        if (!User.Claims.Any(cl => cl.Type == "id" && cl.Value == $"{sellerId}"))
            return Unauthorized(new ErrorModel("Access is denied"));

        if (!_context.Products.Any(p => p.Id == product.Id))
            return NotFound(new ErrorModel("Product not found"));

        product.SellerId = sellerId;

        _logger.LogDebug("Update existing product with id = {Id}", product.Id);
        _context.Entry(product).State = EntityState.Modified;

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(500, new ErrorModel("Some error has occurred")),
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
    [ProducesResponseType(typeof(Product), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<Product>> Patch([FromBody] JsonPatchDocument<Product> productPatch, int id)
    {
        if (productPatch is null || id <= 0)
            return BadRequest(new ErrorModel("Input data is empty"));

        var product = await _context.Products.FirstOrDefaultAsync(u => u.Id == id);

        if (product is null)
            return NotFound(new ErrorModel("Product not found"));

        if (!User.Claims.Any(cl => cl.Type == "id" && cl.Value == $"{product.SellerId}"))
            return Unauthorized(new ErrorModel("Access is denied"));

        _logger.LogDebug("Update existing product with id = {Id}", id);

        productPatch.ApplyTo(product, ModelState);

        if (!ModelState.IsValid)
            return BadRequest(new ErrorModel("Model state is invalid"));

        _context.Entry(product).State = EntityState.Modified;

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(500, new ErrorModel("Some error has occurred")),
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
    [ProducesResponseType(typeof(void), (int) HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<Product>> Delete(long id)
    {
        if (id <= 0)
            return BadRequest(new ErrorModel("The input data is empty"));

        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
            return NotFound(new ErrorModel("Product not found"));

        if (!User.Claims.Any(cl => cl.Type == "id" && cl.Value == $"{product.SellerId}"))
            return Unauthorized(new ErrorModel("Access is denied"));

        _logger.LogDebug("Delete existing product with id = {Id}", id);

        _context.Products.Remove(product);

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel("Some error has occurred")),
            _ => NoContent()
        };
    }
}