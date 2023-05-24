using System.Net;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPM_Project_Backend.Domain;
using RPM_Project_Backend.Models;
using RPM_Project_Backend.Services.Database;

namespace RPM_Project_Backend.Controllers;

/// <inheritdoc />
[ApiController]
[ApiVersion("1.0")]
[Route("api/payment")]
public class PaymentController : ControllerBase
{
    private readonly ILogger<PaymentController> _logger;
    private readonly ApplicationContext _context;
    private readonly IMapper _mapper;

    /// <inheritdoc />
    public PaymentController(ILogger<PaymentController> logger, ApplicationContext context, IMapper mapper)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Get payments list
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// GET api/payment
    /// 
    /// </remarks>
    /// <response code="200">Return payments list</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Payments not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<Payment>), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(void), (int) HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<Payment>>> Get()
    {
        _logger.LogDebug("Get list of payments");

        var userId = int.Parse(User.Claims.First(cl => cl.Type == "id").Value);

        var payments = _context.Payments
            .OrderByDescending(g => g.Id)
            .Where(p => p.UserId == userId && p.IsActive == true)
            .AsQueryable();

        var paginationMetadata = new
        {
            totalCount = payments.Count()
        };

        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

        return await payments.CountAsync() switch
        {
            0 => NotFound(new ErrorModel("Payments not found")),
            _ => Ok(
                payments
                    .ToList()
            )
        };
    }
    
    /// <summary>
    /// Get ALL payments list
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// GET api/payment
    /// 
    /// </remarks>
    /// <response code="200">Return payments list</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Payments not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpGet]
    [Authorize(Roles = "admin")]
    [Route("admin")]
    [ProducesResponseType(typeof(IEnumerable<Payment>), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(void), (int) HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<Payment>>> GetAdmin()
    {
        _logger.LogDebug("Get list of payments");

        var payments = _context.Payments
            .OrderByDescending(g => g.Id)
            .AsQueryable();

        var paginationMetadata = new
        {
            totalCount = payments.Count()
        };

        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

        return await payments.CountAsync() switch
        {
            0 => NotFound(new ErrorModel("Payments not found")),
            _ => Ok(
                payments
                    .ToList()
            )
        };
    }

    /// <summary>
    /// Create new payment method
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// POST api/payment&#xA;&#xD;
    ///     {
    ///         "number": "string",
    ///         "cardName": "string",
    ///         "expirationDate": "2023-05-01T14:34:17.276Z",
    ///         "cvc": "string"
    ///     }
    /// 
    /// </remarks>
    /// <param name="paymentDto"></param>
    /// <response code="200">Return created payment</response>
    /// <response code="400">Same payment found</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Payment), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(void), (int) HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<Payment>> Post(PaymentDto paymentDto)
    {
        if (paymentDto is null)
            return BadRequest(new ErrorModel("The input data is empty"));

        _logger.LogDebug("Create new payment bank card");

        var userId = int.Parse(User.Claims.First(cl => cl.Type == "id").Value);

        if (await _context.Payments
                .AnyAsync(p =>
                    p.Number == paymentDto.Number
                    && p.CardName == paymentDto.CardName
                    && p.ExpirationDate == paymentDto.ExpirationDate
                    && p.Cvc == paymentDto.Cvc))
            return BadRequest("Product with same genre already exists");

        var payment = _mapper.Map<Payment>(paymentDto);

        payment.UserId = userId;
        payment.IsActive = true;

        var createdPayment = await _context.Payments.AddAsync(payment);

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(500, new ErrorModel("Some error has occurred")),
            _ => Ok(createdPayment.Entity)
        };
    }
    
    /// <summary>
    /// Update single payment
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// PUT api/payment&#xA;&#xD;
    ///     {
    ///         "id": 11,
    ///         "number": "string",
    ///         "cardName": "string",
    ///         "expirationDate": "2023-05-01T14:34:17.276Z",
    ///         "cvc": "string"
    ///     }
    /// 
    /// </remarks>
    /// <param name="paymentEditDto"></param>
    /// <response code="200">Return created payment</response>
    /// <response code="400">The input data is empty</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Payment not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpPut]
    [Authorize]
    [ProducesResponseType(typeof(Review), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(void), (int) HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<Review>> Put([FromBody] PaymentEditDto paymentEditDto)
    {
        if (paymentEditDto is null)
            return BadRequest(new ErrorModel("The input data is empty"));

        var payment = await _context.Payments
            .FirstOrDefaultAsync(r => r.Id == paymentEditDto.Id);

        if (payment is null)
            return BadRequest("Payment with entered id does not exist");

        if (!User.Claims.Any(cl => cl.Type == "id" && cl.Value == $"{payment.UserId}"))
            return Unauthorized(new ErrorModel("Access is denied"));

        if (!_context.Payments.Any(p => p.Id == payment.Id))
            return NotFound(new ErrorModel("Payment not found"));

        _logger.LogDebug("Update existing payment with id = {id}", payment.Id);

        payment.Number = paymentEditDto.Number;
        payment.CardName = paymentEditDto.CardName;
        payment.Cvc = paymentEditDto.Cvc;
        payment.ExpirationDate = paymentEditDto.ExpirationDate;
        
        _context.Entry(payment).State = EntityState.Modified;

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(500, new ErrorModel("Some error has occurred")),
            _ => Ok(payment)
        };
    }
    
    /// <summary>
    /// Restore payment method
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// GET api/payment/restore/11
    /// 
    /// </remarks>
    /// <response code="200">Return payment</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Payments not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpGet("restore/{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(Payment), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(void), (int) HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<Payment>> Restore(int id)
    {
        if (id <= 0)
            return BadRequest(new ErrorModel("The input data is empty"));

        var payment = await _context.Payments
            .FirstOrDefaultAsync(g => g.Id == id);
        
        if (!User.Claims.Any(cl => cl.Type == "id" && cl.Value == $"{payment.UserId}"))
            return Unauthorized(new ErrorModel("Access is denied"));

        if (payment is null)
            return NotFound(new ErrorModel("Payment not found"));

        if (payment.IsActive)
            return BadRequest(new ErrorModel("Payment is already activated"));

        _logger.LogDebug("Restore existing payment with id = {id}", id);

        payment.IsActive = true;
        _context.Entry(payment).State = EntityState.Modified;

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel("Some error has occurred")),
            _ => Ok(payment)
        };
    }

    /// <summary>
    /// Delete single payment
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// DELETE api/payment/11
    /// 
    /// </remarks>
    /// <param name="id"></param>
    /// <response code="204">Delete payment</response>
    /// <response code="400">The input data is empty</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Payment not found</response>
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

        var payment = await _context.Payments
            .FirstOrDefaultAsync(g => g.Id == id);
        
        if (!User.Claims.Any(cl => cl.Type == "id" && cl.Value == $"{payment.UserId}"))
            return Unauthorized(new ErrorModel("Access is denied"));

        if (payment is null)
            return NotFound(new ErrorModel("Payment not found"));

        _logger.LogDebug("Delete existing payment with id = {id}", id);

        payment.IsActive = false;
        _context.Entry(payment).State = EntityState.Modified;

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel("Some error has occurred")),
            _ => NoContent()
        };
    }

    /// <summary>
    /// Full delete single payment with admin rights
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// DELETE api/payment/11
    /// 
    /// </remarks>
    /// <param name="id"></param>
    /// <response code="204">Delete payment</response>
    /// <response code="400">The input data is empty</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Payment not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [Authorize(Roles = "admin")]
    [HttpDelete("admin/{id:int}")]
    [ProducesResponseType(typeof(void), (int) HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorModel), (int) HttpStatusCode.InternalServerError)]
    public async Task<ActionResult> DeleteAdmin(int id)
    {
        if (id <= 0)
            return BadRequest(new ErrorModel("The input data is empty"));

        var payment = await _context.Payments.FirstOrDefaultAsync(g => g.Id == id);

        if (payment is null)
            return NotFound(new ErrorModel("Payment not found"));

        _logger.LogDebug("Delete existing genre with id = {id}", id);

        _context.Payments.Remove(payment);

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel("Some error has occurred")),
            _ => NoContent()
        };
    }
}