using System.Net;
using System.Text.Json;
using AnySoftBackend.Library.DataTransferObjects.Property;
using AnySoftBackend.Library.Misc;
using AnySoftBackend.Models;
using AnySoftBackend.Services.Database;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnySoftBackend.Domain;
using AnySoftBackend.Helpers;

namespace AnySoftBackend.Controllers;

/// <inheritdoc />
[ApiController]
[ApiVersion("1.0")]
[Route("api/properties")]
public class PropertiesController : ControllerBase
{
    private readonly ILogger<PropertiesController> _logger;
    private readonly ApplicationContext _context;
    private readonly IMapper _mapper;

    /// <inheritdoc />
    public PropertiesController(ILogger<PropertiesController> logger, ApplicationContext context, IMapper mapper)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Get properties list
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// GET api/properties
    /// 
    /// </remarks>
    /// <response code="200">Return properties list</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Properties not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<Property>), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<Property>>> Get(
        [FromQuery] int? productId,
        [FromQuery] QueryParameters<object> queryParameters)
    {
        _logger.LogDebug("Get list of properties");

        var properties = productId is not null
            ? _context.Properties
                .Include(p => p.ProductsHaveProperties)
                .OrderByDescending(g => g.Id)
                .Where(p => p.ProductsHaveProperties.Any(php => php.ProductId == productId))
                .AsQueryable()
            : _context.Properties
                .OrderByDescending(g => g.Id)
                .AsQueryable();

        var paginationMetadata = new
        {
            totalCount = properties.Count(),
            pageSize = properties.Count() < queryParameters.PageCount
                ? properties.Count()
                : queryParameters.PageCount,
            currentPage = queryParameters.GetTotalPages(properties.Count()) < queryParameters.Page
                ? (int)queryParameters.GetTotalPages(properties.Count())
                : queryParameters.Page,
            totalPages = queryParameters.GetTotalPages(properties.Count())
        };

        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
        
        if (queryParameters is {Page: <= 0} or {PageCount: <= 0})
            return NotFound(new ErrorModel("Properties not found"));

        return await properties.CountAsync() switch
        {
            0 => NotFound(new ErrorModel("Properties not found")),
            _ => Ok(
                properties
                    .Skip(paginationMetadata.pageSize * (paginationMetadata.currentPage - 1))
                    .Take(paginationMetadata.pageSize)
                    .ToList()
            )
        };
    }

    /// <summary>
    /// Create new property or add property to product
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// POST api/properties&#xA;&#xD;
    ///     {
    ///         "name": "SomeName",
    ///         "productId": 3
    ///     }
    /// 
    /// </remarks>
    /// <param name="propertyCreateDto"></param>
    /// <response code="200">Return created property</response>
    /// <response code="400">Same property found</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Property), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(void), (int) HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<Property>> Post(PropertyCreateDto propertyCreateDto)
    {
        if (propertyCreateDto is null)
            return BadRequest(new ErrorModel("The input data is empty"));

        _logger.LogDebug("Create new property with product id = {ProductId}", propertyCreateDto.ProductId ?? 0);

        var existedProperty = await _context.Properties
            .Include(g => g.ProductsHaveProperties)
            .FirstOrDefaultAsync(p => p.Name == propertyCreateDto.Name);
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == propertyCreateDto.ProductId);

        switch (existedProperty)
        {
            case not null
                when product is not null
                     && existedProperty.ProductsHaveProperties!.Any(phg => phg.ProductId == product.Id):
                return BadRequest("Product with same property already exists");
            case not null when product is not null:
                var productHaveProperties = new ProductsHaveProperties()
                {
                    PropertyId = existedProperty.Id,
                    ProductId = propertyCreateDto.ProductId ?? throw new InvalidOperationException("Product id = null")
                };
                await _context.ProductsHaveProperties.AddAsync(productHaveProperties);
                return await _context.SaveChangesAsync() switch
                {
                    0 => StatusCode(500, new ErrorModel("Some error has occurred")),
                    _ => Ok(existedProperty)
                };
        }

        var property = _mapper.Map<Property>(propertyCreateDto);
        var createdProperty = await _context.Properties.AddAsync(property);

        if (await _context.SaveChangesAsync() == 0)
        {
            return StatusCode(500, new ErrorModel("Some error has occurred"));
        }

        switch (product)
        {
            case null when propertyCreateDto.ProductId is not null:
                return BadRequest("Product with entered id does not exist");
            case null when propertyCreateDto.ProductId is null:
                return Ok(createdProperty.Entity);
            default:
                var productHaveProperty = new ProductsHaveProperties()
                {
                    PropertyId = createdProperty.Entity.Id,
                    ProductId = propertyCreateDto.ProductId ?? throw new InvalidOperationException("Product id = null")
                };
                await _context.ProductsHaveProperties.AddAsync(productHaveProperty);
                return await _context.SaveChangesAsync() switch
                {
                    0 => StatusCode(500, new ErrorModel("Some error has occurred")),
                    _ => Ok(createdProperty.Entity)
                };
        }
    }

    /// <summary>
    /// Delete single property
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// DELETE api/properties/11
    /// 
    /// </remarks>
    /// <param name="id"></param>
    /// <response code="204">Delete property</response>
    /// <response code="400">The input data is empty</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Property not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [Authorize(Roles = "admin")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(void), (int) HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult> Delete(int id)
    {
        if (id <= 0)
            return BadRequest(new ErrorModel("The input data is empty"));

        var property = await _context.Properties.FirstOrDefaultAsync(p => p.Id == id);

        if (property is null)
            return NotFound(new ErrorModel("Property not found"));

        _logger.LogDebug("Delete existing property with id = {Id}", id);

        _context.Properties.Remove(property);

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel("Some error has occurred")),
            _ => NoContent()
        };
    }

    /// <summary>
    /// Delete single property from product
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// DELETE api/properties/11?productId=1
    /// 
    /// </remarks>
    /// <param name="id"></param>
    /// <param name="productId"></param>
    /// <response code="204">Delete property</response>
    /// <response code="400">The input data is empty</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Property not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [Authorize(Roles = "seller")]
    [HttpDelete("delete/{id:int}")]
    [ProducesResponseType(typeof(void), (int) HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult> Delete(
        int id,
        [FromQuery] int productId)
    {
        if (id <= 0 || productId <= 0)
            return BadRequest(new ErrorModel("The input data is empty"));

        var property = await _context.Properties
            .Include(p => p.ProductsHaveProperties)
            .FirstOrDefaultAsync(p => p.Id == id);
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (property is null)
            return NotFound(new ErrorModel("Property not found"));
        if (product is null)
            return NotFound(new ErrorModel("Product not found"));
        if (!User.Claims.Any(cl => cl.Type == "id" && cl.Value == $"{product.SellerId}"))
            return Unauthorized(new ErrorModel("Access is denied"));
        if (property.ProductsHaveProperties is null)
            return NotFound(new ErrorModel("Product with entered property not found"));
        if (property.ProductsHaveProperties.All(php => php.ProductId != productId))
            return NotFound(new ErrorModel("Product with entered property not found"));

        _logger.LogDebug("Delete property product with id = {Id}", id);

        var productHaveProperties = property.ProductsHaveProperties
            .FirstOrDefault(phg => phg.ProductId == productId && phg.PropertyId == id);

        if (productHaveProperties is null)
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel("Some error has occurred"));

        _context.ProductsHaveProperties.Remove(productHaveProperties);

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel("Some error has occurred")),
            _ => NoContent()
        };
    }
}