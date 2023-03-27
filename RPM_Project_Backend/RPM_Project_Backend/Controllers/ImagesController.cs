using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPM_PR_LIB;
using RPM_Project_Backend.Enums;
using RPM_Project_Backend.Helpers;
using RPM_Project_Backend.Models;
using RPM_Project_Backend.Services.Database;

namespace RPM_Project_Backend.Controllers;

/// <inheritdoc />
[ApiController]
[ApiVersion("1.0")]
[Route("api/image")]
public class ImagesController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AccountController> _logger;
    private readonly IWebHostEnvironment _environment;
    private readonly ApplicationContext _context;
    private readonly DbSet<Image> _dbSet;

    /// <inheritdoc />
    public ImagesController(
        IConfiguration configuration, ILogger<AccountController> logger, ApplicationContext context, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _logger = logger;
        _context = context;
        _environment = environment;
        _dbSet = _context.Set<Image>();
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<Image>> Get(int id)
    {
        if (id <= 0)
            return BadRequest(new ErrorModel("Input data is empty"));
        
        _logger.LogDebug("Get image with id = {id}", id);

        var image = await _dbSet.FirstOrDefaultAsync(i => i.Id == id);
        
        if (image is null)
            return NotFound(new ErrorModel("Image not found"));
        
        var imageData = await System.IO.File.ReadAllBytesAsync(image.ImagePath);

        return File(imageData, image.ContentType);
    }
    
    [HttpGet("{filename}")]
    [AllowAnonymous]
    public async Task<ActionResult<Image>> Get(string filename)
    {
        if (filename is null or {Length: 0})
            return BadRequest(new ErrorModel("Input data is empty"));
        
        _logger.LogDebug("Get image with filename = {filename}", filename);
        
        var image = await _dbSet.FirstOrDefaultAsync(i => i.ImagePath.Contains(filename));
        
        if (image is null)
            return NotFound(new ErrorModel("Image not found"));
        
        var imageData = await System.IO.File.ReadAllBytesAsync(image.ImagePath);

        return File(imageData, image.ContentType);
    }

    [HttpPost]
    [Authorize]
    [AllowAnonymous]
    [Route("upload")]
    [RequestSizeLimit(8 * 1024 * 1024)]
    public async Task<ActionResult<Image>> Upload([FromForm]ImageDto imageDto)
    {
        if (imageDto is null or {ResourceId: <= 0} or {Type: <= 0} or {Image: null})
            return BadRequest(new ErrorModel("Input data is empty"));
        
        var uniqueFileName = FileNameHelper.GetUniqueFileName(imageDto.Image.FileName);
        
        var imagesPath = Path.Combine(_environment.WebRootPath,
            "images",
            imageDto.ResourceId.ToString());
        
        var filePath = Path.Combine(imagesPath,
            imageDto.Type.GetDescription(),
            uniqueFileName);
        
        _logger.LogDebug("Upload image with path = {name}", filePath);
        
        var image = new Image
        {
            ResourceId = imageDto.ResourceId,
            Type = imageDto.Type,
            ContentType = imageDto.Image.ContentType,
            Description = imageDto.Description,
            Ts = DateTime.Now,
            ImagePath = filePath
        };

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException("Directory name is null"));
            
            _dbSet.Add(image);
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
            await imageDto.Image.CopyToAsync(stream);
        }

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(StatusCodes.Status500InternalServerError,
                new ErrorModel("Some error has occurred")),
            _ => Ok(image)
        };
    }
    
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<Image>> Delete(int id)
    {
        if (id <= 0)
            return BadRequest(new ErrorModel("Input data is empty"));
        
        _logger.LogDebug("Delete image with id = {id}", id);

        var image = await _dbSet.FirstOrDefaultAsync(i => i.Id == id);
        
        if (image is null)
            return NotFound(new ErrorModel("Image not found"));
        
        var entityEntry = _dbSet.Remove(image);

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(StatusCodes.Status500InternalServerError,
                new ErrorModel("Some error has occurred")),
            _ => Ok(entityEntry)
        };
    }
    
    [HttpDelete("{filename}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<Image>> Delete(string filename)
    {
        if (filename is null or {Length: 0})
            return BadRequest(new ErrorModel("Input data is empty"));
        
        _logger.LogDebug("Delete image with filename = {filename}", filename);
        
        var image = await _dbSet.FirstOrDefaultAsync(i => i.ImagePath.Contains(filename));
        
        if (image is null)
            return NotFound(new ErrorModel("Image not found"));

        var entityEntry = _dbSet.Remove(image);

        return await _context.SaveChangesAsync() switch
        {
            0 => StatusCode(StatusCodes.Status500InternalServerError,
                new ErrorModel("Some error has occurred")),
            _ => Ok(entityEntry)
        };
    }
}