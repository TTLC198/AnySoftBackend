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
[Route("api/orders")]
[EnableCors("MyPolicy")]
public class OrdersController : ControllerBase
{
    private readonly ILogger<OrdersController> _logger;
    private readonly ApplicationContext _context;
    private readonly IMapper _mapper;

    /// <inheritdoc />
    public OrdersController(ILogger<OrdersController> logger, ApplicationContext context, IMapper mapper)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
    }
    
    /// <summary>
    /// Get orders list
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// GET api/orders
    /// 
    /// </remarks>
    /// <response code="200">Return orders list</response>
    /// <response code="404">Orders not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<OrderResponseDto>), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProductResponseDto>>> Get(
        [FromQuery] QueryParameters<object> queryParameters)
    {
        _logger.LogDebug("Get list of orders");

        var userId = int.Parse(User.Claims.First(cl => cl.Type == "id").Value);

        var orders = _context.Orders
            .Where(o => o.UserId == userId)
            .OrderBy(queryParameters.OrderBy, queryParameters.IsDescending());

        var paginationMetadata = new
        {
            totalCount = orders.Count(),
            pageSize = orders.Count() < queryParameters.PageCount
                ? orders.Count()
                : queryParameters.PageCount,
            currentPage = queryParameters.GetTotalPages(orders.Count()) < queryParameters.Page
                ? (int)queryParameters.GetTotalPages(orders.Count())
                : queryParameters.Page,
            totalPages = queryParameters.GetTotalPages(orders.Count())
        };

        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
        
        if (queryParameters is {Page: <= 0} or {PageCount: <= 0})
            return NotFound(new ErrorModel("Orders not found"));

        return await orders.CountAsync() switch
        {
            0 => NotFound(new ErrorModel("Orders not found")),
            _ => Ok(
                orders
                    .Skip(paginationMetadata.pageSize * (paginationMetadata.currentPage - 1))
                    .Take(paginationMetadata.pageSize)
                    .Select(o => _mapper.Map<OrderResponseDto>(o))
            )
        };
    }
    
    /// <summary>
    /// Get single order
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// GET api/orders/1
    /// 
    /// </remarks>
    /// <response code="200">Return single order</response>
    /// <response code="404">Order not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpGet("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(OrderResponseDto), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<OrderResponseDto>> Get(
        int id)
    {
        if (id <= 0)
            return BadRequest(new ErrorModel("Input data is empty"));
        
        _logger.LogDebug("Get order with id = {id}", id);

        var userId = int.Parse(User.Claims.First(cl => cl.Type == "id").Value);

        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.UserId == userId && o.Id == id);

        return order switch
        {
            null => NotFound(new ErrorModel("Order not found")),
            _ => Ok(order)
        };
    }
}