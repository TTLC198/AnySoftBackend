using Microsoft.AspNetCore.Mvc;
using RPM_PR_LIB;
using RPM_Project_Backend.Repositories;

namespace RPM_Project_Backend.Controllers.ProductsController;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly IBaseRepository<Product> _productsRepository;

    public ProductsController(ILogger<ProductsController> logger, IBaseRepository<Product> productsRepository)
    {
        _logger = logger;
        _productsRepository = productsRepository;
    }
    /// <summary>
    /// Get api/products
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> Get()
    {
        _logger.LogDebug("Get list of products");
        var products = await _productsRepository.GetAllAsync();
        return products.Count() switch
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
        var product = await _productsRepository.GetAsync(id);
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
    /// <param name="user"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<Product>> Post(Product product)
    {
        _logger.LogDebug("Create new product with id = {id}", product.Id);
        return product switch
        {
            null => BadRequest(),
            _ => Ok(await _productsRepository.CreateAsync(product))
        };
    }
    /// <summary>
    /// Put api/products/
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<ActionResult<Product>> Put(Product product)
    {
        _logger.LogDebug("Update existing product with id = {id}", product.Id);
        return product switch
        {
            null => BadRequest(),
            _ when _productsRepository.GetAllAsync().Result.All(u => u.Id != product.Id) => NotFound(),
            _ => Ok(await _productsRepository.UpdateAsync(product))
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
        _logger.LogDebug("Delete existing product with id = {id}", id);
        return await _productsRepository.GetAsync(id) switch
        {
            null => NotFound(),
            _ => Ok(await _productsRepository.DeleteAsync(id))
        };
    }
}