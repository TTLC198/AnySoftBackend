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
[Route("api/genres")]
[EnableCors("MyPolicy")]
public class GenresController : ControllerBase
{
    private readonly ILogger<GenresController> _logger;
    private readonly ApplicationContext _context;
    private readonly IMapper _mapper;

    /// <inheritdoc />
    public GenresController(ILogger<GenresController> logger, ApplicationContext context, IMapper mapper)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
    }
    
    /// <summary>
    /// Get genres list
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// GET api/genres
    /// 
    /// </remarks>
    /// <response code="200">Return genres list</response>
    /// <response code="404">Genres not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<Genre>), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<Genre>>> Get(
        [FromQuery] int? productId,
        [FromQuery] QueryParameters<object> queryParameters)
    {
        _logger.LogDebug("Get list of genres");

        var genres = productId is not null
            ? _context.Genres
                .Include(g => g.ProductsHaveGenres)
                .OrderByDescending(g => g.Id)
                .Where(g => g.ProductsHaveGenres.Any(phg => phg.ProductId == productId))
                .AsQueryable()
            : _context.Genres
                .OrderByDescending(g => g.Id)
                .AsQueryable();

        var paginationMetadata = new
        {
            totalCount = genres.Count(),
            pageSize = queryParameters.PageCount,
            currentPage = queryParameters.Page,
            totalPages = queryParameters.GetTotalPages(genres.Count())
        };

        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

        return await genres.CountAsync() switch
        {
            0 => NotFound(new ErrorModel("Genres not found")),
            _ => Ok(
                genres
                    .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                    .Take(queryParameters.PageCount)
                    .ToList()
            )
        };
    }

    /// <summary>
    /// Create new genre or add genre to product
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// POST api/genres&#xA;&#xD;
    ///     {
    ///         "name": "SomeName",
    ///         "productId": 3
    ///     }
    /// 
    /// </remarks>
    /// <param name="genreDto"></param>
    /// <response code="200">Return created genre</response>
    /// <response code="400">Same genre found</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Genre), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(void), (int) HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<Genre>> Post(GenreDto genreDto)
    {
        if (genreDto is null)
            return BadRequest(new ErrorModel("The input data is empty"));

        _logger.LogDebug("Create new genre with product id = {productId}", genreDto.ProductId ?? 0);

        var existedGenre = await _context.Genres
            .Include(g => g.ProductsHaveGenres)
            .FirstOrDefaultAsync(p => p.Name == genreDto.Name);
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == genreDto.ProductId);
        
        switch (existedGenre)
        {
            case not null 
                when product is not null 
                     && existedGenre.ProductsHaveGenres!.Any(phg => phg.ProductId == product.Id):
                return BadRequest("Product with same genre already exists");
            case not null when product is not null:
                var productHaveGenre = new ProductsHaveGenres()
                {
                    GenreId = existedGenre.Id,
                    ProductId = genreDto.ProductId ?? throw new InvalidOperationException("Product id = null")
                };
                await _context.ProductsHaveGenres.AddAsync(productHaveGenre);
                return await _context.SaveChangesAsync() switch
                {
                    0 => StatusCode(500, new ErrorModel("Some error has occurred")),
                    _ => Ok(existedGenre)
                };
        }

        var genre = _mapper.Map<Genre>(genreDto);
        var createdGenre = await _context.Genres.AddAsync(genre);

        if (await _context.SaveChangesAsync() == 0)
        {
            return StatusCode(500, new ErrorModel("Some error has occurred"));
        }

        switch (product)
        {
            case null when genreDto.ProductId is not null:
                return BadRequest("Product with entered id does not exist");
            case null when genreDto.ProductId is null:
                return Ok(createdGenre.Entity);
            default:
                var productHaveGenre = new ProductsHaveGenres()
                {
                    GenreId = createdGenre.Entity.Id,
                    ProductId = genreDto.ProductId ?? throw new InvalidOperationException("Product id = null")
                };
                await _context.ProductsHaveGenres.AddAsync(productHaveGenre);
                return await _context.SaveChangesAsync() switch
                {
                    0 => StatusCode(500, new ErrorModel("Some error has occurred")),
                    _ => Ok(createdGenre.Entity)
                };
        }
    }
    
    /// <summary>
    /// Delete single genre
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// DELETE api/genres/11
    /// 
    /// </remarks>
    /// <param name="id"></param>
    /// <response code="204">Delete genre</response>
    /// <response code="400">The input data is empty</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Genre not found</response>
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

        var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Id == id);

        if (genre is null)
            return NotFound(new ErrorModel("Genre not found"));

        _logger.LogDebug("Delete existing genre with id = {id}", id);

        _context.Genres.Remove(genre);

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel("Some error has occurred")),
            _ => NoContent()
        };
    }

    /// <summary>
    /// Delete single genre from product
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// DELETE api/genres/11?productId=1
    /// 
    /// </remarks>
    /// <param name="id"></param>
    /// <param name="productId"></param>
    /// <response code="204">Delete genre</response>
    /// <response code="400">The input data is empty</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Genre not found</response>
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

        var genre = await _context.Genres
            .Include(g => g.ProductsHaveGenres)
            .FirstOrDefaultAsync(g => g.Id == id);
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (genre is null)
            return NotFound(new ErrorModel("Genre not found"));
        if (product is null)
            return NotFound(new ErrorModel("Product not found"));
        if (!User.Claims.Any(cl => cl.Type == "id" && cl.Value == $"{product.SellerId}"))
            return Unauthorized(new ErrorModel("Access is denied"));
        if (genre.ProductsHaveGenres is null)
            return NotFound(new ErrorModel("Product with entered genre not found"));
        if (genre.ProductsHaveGenres.All(phg => phg.ProductId != productId))
            return NotFound(new ErrorModel("Product with entered genre not found"));
        
        _logger.LogDebug("Delete genre product with id = {id}", id);

        var productHaveGenres = genre.ProductsHaveGenres
            .FirstOrDefault(phg => phg.ProductId == productId && phg.GenreId == id);

        if (productHaveGenres is null)
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel("Some error has occurred"));

        _context.ProductsHaveGenres.Remove(productHaveGenres);

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel("Some error has occurred")),
            _ => NoContent()
        };
    }
}