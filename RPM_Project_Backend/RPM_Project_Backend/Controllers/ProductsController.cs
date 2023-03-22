using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPM_PR_LIB;
using RPM_Project_Backend.Services.Database;
using System.Text.Json;
using Microsoft.AspNetCore.JsonPatch;
using RPM_Project_Backend.Helpers;
using RPM_Project_Backend.Models;

namespace RPM_Project_Backend.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly ApplicationContext _context;
    private readonly DbSet<Product> _dbSet;

    public ProductsController(ILogger<ProductsController> logger, ApplicationContext context)
    {
        _logger = logger;
        _context = context;
        _dbSet = _context.Set<Product>();
    }

    /// <summary>
    /// Get api/products
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> Get([FromQuery]QueryParameters<ProductRequestDto> queryParameters)
    {
        _logger.LogDebug("Get list of products");
        
        IQueryable<Product> allProducts = 
            _dbSet.OrderBy(queryParameters.OrderBy, queryParameters.IsDescending());

        if (queryParameters.HasQuery())
        {
            try
            {
                var productQuery = (ProductRequestDto)queryParameters.Object;
               allProducts = _dbSet.Where(product =>
                    (productQuery.Name == null ||
                     product.Name.Contains(productQuery.Name)) &&
                    (productQuery.Rating == null ||
                     (productQuery.Rating.Min <= product.Rating && product.Rating <= productQuery.Rating.Max)) &&
                    (productQuery.Name == null ||
                     (productQuery.Cost.Min <= product.Cost && product.Cost <= productQuery.Cost.Max)) &&
                    (productQuery.Discount == null ||
                     product.Discount >= productQuery.Discount) &&
                    (productQuery.Category == null ||
                     product.CatId == productQuery.Category) &&
                    (productQuery.Quantity == null ||
                     product.Quantity >= productQuery.Quantity));
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
        
        return allProducts.Count() switch
        {
            0 => NotFound(),
            _ => Ok(
                allProducts
                    .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                    .Take(queryParameters.PageCount)
            )
        };
    }

    /// <summary>
    /// Get api/product/{id}
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> Get(long id)
    {
        _logger.LogDebug("Get product with id = {id}", id);
        
        var product = await _dbSet
            .FindAsync(id);
        
        return product switch
        {
            null => NotFound(),
            _ => Ok(product)
        };
    }

    //// Publisher territory

    /// <summary>
    /// Post api/products
    /// </summary>
    /// <param name="product"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<Product>> Post(Product product)
    {
        _logger.LogDebug("Create new product with id = {id}", product.Id);
        
        if (product is null)
            return BadRequest();
        
        await _dbSet.AddAsync(product);

        return await _context.SaveChangesAsync() switch
        {
            0 => BadRequest(),
            _ => Ok(product)
        };
    }

    /// <summary>
    /// Put api/products/
    /// </summary>
    /// <param name="product"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<ActionResult<Product>> Put([FromBody]Product product)
    {
        if (!_dbSet.Any(p => p.Id == product.Id)) 
            return NotFound(product);
        
        _logger.LogDebug("Update existing product with id = {id}", product.Id);
        _context.Entry(product).State = EntityState.Modified;

        return await _context.SaveChangesAsync() switch
        {
            0 => BadRequest(),
            _ => Ok(product)
        };
    }
    /// <summary>
    /// Patch api/products/
    /// </summary>
    /// <param name="productPatch"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPatch("{id:int}")]
    public async Task<ActionResult<Product>> Patch([FromBody]JsonPatchDocument<Product> productPatch, int id)
    {
        if (productPatch is null)
            return BadRequest();

        var product = await _dbSet.FirstOrDefaultAsync(u => u.Id == id);
        
        if (product is null) 
            return NotFound();
        
        _logger.LogDebug("Update existing product with id = {id}", id);
        
        productPatch.ApplyTo(product, ModelState);
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Entry(product).State = EntityState.Modified;

        return await _context.SaveChangesAsync() switch
        {
            0 => BadRequest(),
            _ => Ok(product)
        };
    }
    /// <summary>
    /// Delete api/products
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<Product>> Delete(long id)
    {
        var toDelete = await _dbSet.FindAsync(id);
        if (toDelete is null)
            return NotFound(id);
        
        _logger.LogDebug("Delete existing product with id = {id}", id);
        
        var entityEntry = _dbSet.Remove(toDelete);

        return await _context.SaveChangesAsync() switch
        {
            0 => NotFound(),
            _ => Ok(entityEntry)
        };
    }
}