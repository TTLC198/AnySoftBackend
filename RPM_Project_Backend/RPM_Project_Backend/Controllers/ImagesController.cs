using System.Net;
using AnySoftBackend.Library.DataTransferObjects.Image;
using AnySoftBackend.Library.Misc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPM_Project_Backend.Domain;
using RPM_Project_Backend.Helpers;
using RPM_Project_Backend.Services.Database;

namespace RPM_Project_Backend.Controllers;

/// <inheritdoc />
[ApiController]
[ApiVersion("1.0")]
[Route("resources/image")]
public class ImagesController : ControllerBase
{
    private readonly ILogger<ImagesController> _logger;
    private readonly IWebHostEnvironment _environment;
    private readonly ApplicationContext _context;

    /// <inheritdoc />
    public ImagesController(ILogger<ImagesController> logger, ApplicationContext context, IWebHostEnvironment environment)
    {
        _logger = logger;
        _context = context;
        _environment = environment;
    }

    /// <summary>
    /// Get single image by id
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// GET api/image/4
    ///
    /// </remarks>
    /// <param name="id"></param>
    /// <response code="200">Return image as file</response>
    /// <response code="400">The input data is empty</response>
    /// <response code="404">Image not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [AllowAnonymous]
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Image), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<Image>> Get(int id)
    {
        if (id <= 0)
            return BadRequest(new ErrorModel("Input data is empty"));
        
        _logger.LogDebug("Get image with id = {Id}", id);

        var image = await _context.Images.FirstOrDefaultAsync(i => i.Id == id);
        
        if (image is null)
            return NotFound(new ErrorModel("Image not found"));
        
        var imageData = await System.IO.File.ReadAllBytesAsync(image.ImagePath!);

        return File(imageData, image.ContentType!);
    }
    
    /// <summary>
    /// Get single image by path
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// GET api/image/some_path.png
    ///
    /// </remarks>
    /// <param name="path"></param>
    /// <response code="200">Return image as file</response>
    /// <response code="400">The input data is empty</response>
    /// <response code="404">Image not found</response>
    /// <response code="500">Oops! Server internal error</response>
    [AllowAnonymous]
    [HttpGet("{*path}")]
    [ProducesResponseType(typeof(Image), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<Image>> Get([FromRoute]string path)
    {
        if (path is null or {Length: 0})
            return BadRequest(new ErrorModel("Input data is empty"));
        
        _logger.LogDebug("Get image with filename = {Filename}", path);

        var image = _context.Images
            .Select(i => new
            {
                i.ImagePath,
                i.ContentType
            })
            .ToList()
            .FirstOrDefault(i => string.Join(@"/", i.ImagePath.Split('\\').SkipWhile(s => s != "wwwroot").Skip(1)) == path);
        
        if (image is null)
            return NotFound(new ErrorModel("Image not found"));
        
        var imageData = await System.IO.File.ReadAllBytesAsync(image.ImagePath);

        return File(imageData, image.ContentType);
    }

    /// <summary>
    /// Upload single image
    /// </summary>
    /// <remarks>
    /// Example request
    ///
    /// POST /resources/images/upload&#xA;&#xD;
    ///
    /// Form data:&#xA;&#xD;
    ///
    /// resourceId:2
    /// type:1
    /// description:Some description
    /// image:[image file]
    /// 
    /// </remarks>
    /// <param name="imageCreateDto"></param>
    /// <response code="200">Return image as created object</response>
    /// <response code="400">The input data is empty</response>
    /// <response code="400">Input data is empty</response>
    /// <response code="500">Oops! Server internal error</response>
    /// <exception cref="InvalidOperationException"></exception>
    [HttpPost]
    [Authorize]
    [Route("upload")]
    [RequestSizeLimit(8 * 1024 * 1024)]
    [ProducesResponseType(typeof(Image), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<Image>> Upload([FromForm]ImageCreateDto imageCreateDto)
    {
        if (imageCreateDto is null or {UserId: <= 0} or {ProductId: <= 0} or {Image: null})
            return BadRequest(new ErrorModel("Input data is empty"));
        
        var uniqueFileName = FileNameHelper.GetUniqueFileName(imageCreateDto.Image.FileName);
        
        var filePath = Path.Combine(_environment.WebRootPath,
            "images",
            uniqueFileName);

        _logger.LogDebug("Upload image with path = {Name}", filePath);
        
        var image = new Image
        {
            UserId = imageCreateDto.UserId,
            ProductId = imageCreateDto.ProductId,
            ContentType = imageCreateDto.Image.ContentType,
            Ts = DateTime.Now,
            ImagePath = filePath
        };

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException("Directory name is null"));
            
            _context.Images.Add(image);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ErrorModel("Some error has occurred"));
        }
        finally
        {
            await using var stream = new FileStream(filePath, FileMode.CreateNew);
            await imageCreateDto.Image.CopyToAsync(stream);
        }

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(StatusCodes.Status500InternalServerError,
                new ErrorModel("Some error has occurred")),
            _ => Ok(ImageUriHelper.GetImagePathAsUri(image.ImagePath))
        };
    }

    /// <summary>
    /// Delete single image by filename
    /// </summary>
    /// <remarks>
    /// Example request
    /// 
    /// DELETE /resources/images/delete/filename.png
    /// 
    /// </remarks>
    /// <param name="filename"></param>
    /// <response code="204">Deleted successful</response>
    /// <response code="400">Input data is empty</response>
    /// <response code="500">Oops! Server internal error</response>
    [HttpDelete("delete/{filename}")]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult> Delete(string filename)
    {
        if (filename is null or {Length: 0})
            return BadRequest(new ErrorModel("Input data is empty"));
        
        _logger.LogDebug("Delete image with filename = {Filename}", filename);
        
        var image = await _context.Images
            .FirstOrDefaultAsync(i => i.ImagePath.Contains(filename));
        
        if (image is null)
            return NotFound(new ErrorModel("Image not found"));
        
        var userIdClaim = User.Claims.SingleOrDefault(cl => cl.Type == "id") ??
                          throw new InvalidOperationException("Invalid auth. Null id claims");
        var userRoleClaim = User.Claims.SingleOrDefault(cl => cl.Type.Contains("role")) ??
                            throw new InvalidOperationException("Invalid auth. Null role claims");

        if (image.UserId is not null)
            if (userIdClaim.Value != $"{image.UserId}" && userRoleClaim.Value != "admin")
                return Unauthorized(new ErrorModel("Access is denied"));
        
        if (image.ProductId is not null)
            if (userIdClaim.Value != $"{_context.Products.FirstOrDefault(p => p.Id == image.ProductId)?.SellerId}" && userRoleClaim.Value != "admin")
                return Unauthorized(new ErrorModel("Access is denied"));

        if (System.IO.File.Exists(image.ImagePath))
        {
            System.IO.File.Delete(image.ImagePath);
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ErrorModel("Some error has occurred"));
        }
        
        _context.Images.Remove(image);

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(StatusCodes.Status500InternalServerError,
                new ErrorModel("Some error has occurred")),
            _ => NoContent()
        };
    }
}