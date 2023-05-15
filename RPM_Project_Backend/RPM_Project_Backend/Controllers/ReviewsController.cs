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
[Route("api/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly ILogger<ReviewsController> _logger;
    private readonly ApplicationContext _context;
    private readonly IMapper _mapper;

    /// <inheritdoc />
    public ReviewsController(ILogger<ReviewsController> logger, ApplicationContext context, IMapper mapper)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Get reviews list
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// GET api/reviews/1
    /// 
    /// </remarks>
    /// <response code="200">Return reviews list</response>
    /// <response code="404">Reviews not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ReviewResponseDto>), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<ReviewResponseDto>>> Get(
        [FromQuery] int? productId,
        [FromQuery] QueryParameters<object> queryParameters)
    {
        _logger.LogDebug("Get list of reviews with product id = {productId}", productId ?? 0);

        if (productId is not null and <= 0)
            return BadRequest(new ErrorModel("The input data is empty"));

        var reviews = productId is not null
            ? _context.Reviews
                .Include(r => r.User)
                .OrderByDescending(r => r.Ts)
                .Where(r => r.ProductId == productId)
                .AsQueryable()
            : _context.Reviews
                .Include(r => r.User)
                .OrderByDescending(r => r.Ts)
                .AsQueryable();

        var paginationMetadata = new
        {
            totalCount = reviews.Count(),
            pageSize = reviews.Count() < queryParameters.PageCount
                ? reviews.Count()
                : queryParameters.PageCount,
            currentPage = queryParameters.GetTotalPages(reviews.Count()) < queryParameters.Page
                ? (int) queryParameters.GetTotalPages(reviews.Count())
                : queryParameters.Page,
            totalPages = queryParameters.GetTotalPages(reviews.Count())
        };

        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

        if (queryParameters is {Page: <= 0} or {PageCount: <= 0})
            return NotFound(new ErrorModel("Reviews not found"));

        return await reviews.CountAsync() switch
        {
            0 => NotFound(new ErrorModel("Reviews not found")),
            _ => Ok(
                reviews
                    .Skip(paginationMetadata.pageSize * (paginationMetadata.currentPage - 1))
                    .Take(paginationMetadata.pageSize)
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
                    })
            )
        };
    }

    /// <summary>
    /// Create new review
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// POST api/reviews&#xA;&#xD;
    ///     {
    ///         "text": "SomeText",
    ///         "grade": 5,
    ///         "productId": 3
    ///     }
    /// 
    /// </remarks>
    /// <param name="reviewDto"></param>
    /// <response code="200">Return created review</response>
    /// <response code="400">Same review found</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Review), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(void), (int) HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<Product>> Post(ReviewDto reviewDto)
    {
        if (reviewDto is null)
            return BadRequest(new ErrorModel("The input data is empty"));

        _logger.LogDebug("Create new review with product id = {productId}", reviewDto.ProductId);

        var userId = int.Parse(User.Claims.First(cl => cl.Type == "id").Value);
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == reviewDto.ProductId);

        if (product is null)
            return BadRequest("Product with entered id does not exist");

        var review = _mapper.Map<Review>(reviewDto);

        review.UserId = userId;
        review.Ts = DateTime.UtcNow;
        product.Rating = (review.Grade + product.Rating) / 2; //TODO

        var createdReview = await _context.Reviews.AddAsync(review);

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(500, new ErrorModel("Some error has occurred")),
            _ => Ok(new ReviewResponseDto()
            {
                Id = createdReview.Entity.Id,
                Text = createdReview.Entity.Text,
                Grade = createdReview.Entity.Grade,
                Ts = createdReview.Entity.Ts,
                ProductId = createdReview.Entity.ProductId
            })
        };
    }

    /// <summary>
    /// Update single review
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// PUT api/reviews&#xA;&#xD;
    ///     {
    ///         "id": 11
    ///         "text": "SomeText",
    ///         "grade": 100
    ///     }
    /// 
    /// </remarks>
    /// <param name="reviewEditDto"></param>
    /// <response code="200">Return created review</response>
    /// <response code="400">The input data is empty</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Review not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpPut]
    [Authorize]
    [ProducesResponseType(typeof(Review), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(void), (int) HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<Review>> Put([FromBody] ReviewEditDto reviewEditDto)
    {
        if (reviewEditDto is null)
            return BadRequest(new ErrorModel("The input data is empty"));

        var review = await _context.Reviews
            .FirstOrDefaultAsync(r => r.Id == reviewEditDto.Id);

        if (review is null)
            return BadRequest("Review with entered id does not exist");

        if (!User.Claims.Any(cl => cl.Type == "id" && cl.Value == $"{review.UserId}"))
            return Unauthorized(new ErrorModel("Access is denied"));

        if (!_context.Reviews.Any(r => r.Id == review.Id))
            return NotFound(new ErrorModel("Review not found"));

        _logger.LogDebug("Update existing review with id = {id}", review.Id);

        review.Text = reviewEditDto.Text;
        review.Grade = reviewEditDto.Grade;
        review.Ts = DateTime.UtcNow;

        _context.Entry(review).State = EntityState.Modified;

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(500, new ErrorModel("Some error has occurred")),
            _ => Ok(review)
        };
    }

    /// <summary>
    /// Delete single review
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// DELETE api/reviews/11
    /// 
    /// </remarks>
    /// <param name="id"></param>
    /// <response code="204">Delete review</response>
    /// <response code="400">The input data is empty</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Review not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [Authorize]
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

        var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);

        if (review is null)
            return NotFound(new ErrorModel("Review not found"));

        if (!User.Claims.Any(cl => cl.Type == "id" && cl.Value == $"{review.UserId}"))
            return Unauthorized(new ErrorModel("Access is denied"));

        _logger.LogDebug("Delete existing review with id = {id}", id);

        _context.Reviews.Remove(review);

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel("Some error has occurred")),
            _ => NoContent()
        };
    }
}