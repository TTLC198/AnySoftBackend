using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPM_PR_LIB;
using RPM_Project_Backend.Services.Database;
using System.Text.Json;

namespace RPM_Project_Backend.Controllers.ProductsController;

[ApiController]
[Route("api/[controller]")]
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
    public async Task<ActionResult<IEnumerable<Product>>> Get([FromQuery]ProductRequest requestBody)
    {
        _logger.LogDebug("Get list of products");

        var products = _dbSet.Where(product =>
            (requestBody.Name == null ||
             product.Name.Contains(requestBody.Name)) &&
            (requestBody.Rating == null ||
             (requestBody.Rating.Min <= product.Rating && product.Rating <= requestBody.Rating.Max)) &&
            (requestBody.Name == null ||
             (requestBody.Cost.Min <= product.Cost && product.Cost <= requestBody.Cost.Max)) &&
            (requestBody.Discount == null ||
             product.Discount >= requestBody.Discount) &&
            (requestBody.Category == null ||
             product.CatId == requestBody.Category) &&
            (requestBody.Quantity == null ||
             product.Quantity >= requestBody.Quantity));
        
        /*products = requestBody.Attributes.Aggregate(products,
            (current, attribute) => current.Include(product =>
                product.ProductsHaveAttributes.Where(productsHaveAttribute => attribute.Value.Contains(productsHaveAttribute.Value))));*/
        
        return await products.CountAsync() switch
        {
            0 => NotFound(),
            _ => Ok(products)
        };
    }

    /// <summary>
    /// Get api/product/{id}
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> Get(long id)
    {
        _logger.LogDebug("Get product with id = {id}", id);
        var product = await _dbSet.FindAsync(id);
        return product switch
        {
            null => NotFound(),
            _ => Ok(product)
        };
    }

    //// Дальше то, что обычному пользователю нельзя

    /// <summary>
    /// Post api/products
    /// </summary>
    /// <param name="product"></param>
    /// <returns></returns>
    //[HttpPost]
    public async Task<ActionResult<Product>> Post(Product product)
    {
        _logger.LogDebug("Create new product with id = {id}", product.Id);
        
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
    public async Task<ActionResult<Product>> Put(Product product)
    {
        if (!_dbSet.Any(p => p.Id == product.Id)) return NotFound(product);
        
        _logger.LogDebug("Update existing product with id = {id}", product.Id);
        
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
    [HttpDelete("{id}")]
    public async Task<ActionResult<Product>> Delete(long id)
    {
        var toDelete = await _dbSet.FindAsync(id);
        if (toDelete is null) return NotFound(id);
        
        _logger.LogDebug("Delete existing product with id = {id}", id);
        
        var entityEntry = _dbSet.Remove(toDelete);

        return await _context.SaveChangesAsync() switch
        {
            0 => NotFound(),
            _ => Ok(entityEntry)
        };
    }
}