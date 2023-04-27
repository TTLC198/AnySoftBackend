using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RPM_Project_Backend.Controllers;
using RPM_Project_Backend.Mappings;
using RPM_Project_Backend.Models;
using RPM_Project_Backend.Tests.Helpers;
using Xunit;

namespace RPM_Project_Backend.Tests.Tests;

[Collection("ApplicationContext Collection")]
public class TestProductsController
{
    private readonly ApplicationContextDataFixture _fixture;
    private ProductsController _controller;
    private ApplicationContext _context;
    private IMapper _mapper;
    private readonly Random _random = new Random();
    
    public TestProductsController(ApplicationContextDataFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetProducts_EmptyQuery_ShouldReturnSameProductsList()
    {
        // Arrange
        var id = 3; //seller
        var user = TestValues.Users.First(u => u.Id == id);
        //Setup DB context
        _context = _fixture.ApplicationContext;

        //Setup logger
        var logger = new Logger<ProductsController>(new LoggerFactory());
        
        //Setup AutoMapper config
        var config = new MapperConfiguration(cfg => cfg.AddProfile<ProductMappingProfile>());
        _mapper = config.CreateMapper();
        
        _controller = new ProductsController(
            logger,
            _context,
            _mapper
        );
        // Setup JWT claims
        _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        _controller.ControllerContext.HttpContext.User = JwtClaimsHelper.GetClaims(user.Id, user.Role.Name);
        Assert.NotNull(_controller.User);

        // Act
        var result = await _controller.Get(new QueryParameters<ProductRequestDto>());

        // Assert
        Assert.NotNull(result);
        
        var okResult = result.Result as OkObjectResult;
        var valueResult = okResult!.Value as IEnumerable<Product>;
        
        Assert.NotNull(okResult!.Value);
        Assert.Equal(_context.Products.ToList(), valueResult!.ToList());
    }
    
    [Fact]
    public async Task GetProducts_WithQuery_ShouldReturnSameProductsList()
    {
        // Arrange
        var id = 3; //seller
        var user = TestValues.Users.First(u => u.Id == id);
        //Setup DB context
        _context = _fixture.ApplicationContext;

        //Setup logger
        var logger = new Logger<ProductsController>(new LoggerFactory());
        
        //Setup AutoMapper config
        var config = new MapperConfiguration(cfg => cfg.AddProfile<ProductMappingProfile>());
        _mapper = config.CreateMapper();
        
        _controller = new ProductsController(
            logger,
            _context,
            _mapper
        );
        // Setup JWT claims
        _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        _controller.ControllerContext.HttpContext.User = JwtClaimsHelper.GetClaims(user.Id, user.Role.Name);
        Assert.NotNull(_controller.User);

        var selectedProducts = _context.Products.ToList().Where(
            p => p.Name.Contains("First product"));
        
        // Act
        var result = await _controller.Get(new QueryParameters<ProductRequestDto>()
        {
            Query = JsonSerializer.Serialize(new ProductRequestDto()
            {
                Name = "First product"
            })
        });

        // Assert
        Assert.NotNull(result);
        
        var okResult = result.Result as OkObjectResult;
        var valueResult = okResult!.Value as IEnumerable<Product>;
        
        Assert.NotNull(okResult!.Value);
        Assert.Equal(selectedProducts.ToList(), valueResult!.ToList());
    }
    
    [Fact]
    public async Task GetProduct_ShouldReturnSameProduct()
    {
        // Arrange
        var id = 3; //seller
        var user = TestValues.Users.First(u => u.Id == id);
        var product = TestValues.Products.First(p => p.Id == id);
        
        //Setup DB context
        _context = _fixture.ApplicationContext;

        //Setup logger
        var logger = new Logger<ProductsController>(new LoggerFactory());
        
        //Setup AutoMapper config
        var config = new MapperConfiguration(cfg => cfg.AddProfile<ProductMappingProfile>());
        _mapper = config.CreateMapper();
        
        _controller = new ProductsController(
            logger,
            _context,
            _mapper
        );
        // Setup JWT claims
        _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        _controller.ControllerContext.HttpContext.User = JwtClaimsHelper.GetClaims(user.Id, user.Role.Name);
        Assert.NotNull(_controller.User);

        // Act
        var result = await _controller.Get(id);

        // Assert
        Assert.NotNull(result);
        
        var okResult = result.Result as OkObjectResult;
        var valueResult = okResult!.Value as Product;
        
        Assert.NotNull(valueResult);
        Assert.Equal(product, valueResult);
    }
    
    [Fact]
    public async Task PostProduct_ShouldReturnSameProduct()
    {
        // Arrange
        var id = 3; //seller
        var user = TestValues.Users.First(u => u.Id == id);
        var product = TestValues.SingleProduct;
        
        //Setup DB context
        _context = _fixture.ApplicationContext;

        //Setup logger
        var logger = new Logger<ProductsController>(new LoggerFactory());
        
        //Setup AutoMapper config
        var config = new MapperConfiguration(cfg => cfg.AddProfile<ProductMappingProfile>());
        _mapper = config.CreateMapper();
        
        _controller = new ProductsController(
            logger,
            _context,
            _mapper
        );
        
        // Setup JWT claims
        _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        _controller.ControllerContext.HttpContext.User = JwtClaimsHelper.GetClaims(user.Id, user.Role.Name);
        Assert.NotNull(_controller.User);
        
        var productDto = _mapper.Map<ProductDto>(product);
        
        // Act
        var result = await _controller.Post(productDto);

        // Assert
        Assert.NotNull(result);

        var okResult = result.Result as ObjectResult;
        var valueResult = okResult!.Value as Product;

        Assert.NotNull(valueResult);
        
        Assert.Equal(product.Name, valueResult.Name);
        Assert.Equal(product.Id, valueResult.Id);
        Assert.Equal(product.Cost, valueResult.Cost);
        Assert.Equal(product.Discount, valueResult.Discount);
        //Assert.Equal(product.Rating, valueResult.Rating); // Rating by default = 5
    }
    
    [Fact]
    public async Task PutProduct_ShouldReturnModifiedProduct()
    {
        // Arrange
        var id = _random.Next(1,3);
        var user = TestValues.Users.First(u => u.Id == 3); //seller
        var product = TestValues.Products.First(p => p.Id == id);
        
        //Setup DB context
        _context = _fixture.ApplicationContext;

        //Setup logger
        var logger = new Logger<ProductsController>(new LoggerFactory());
        
        //Setup AutoMapper config
        var config = new MapperConfiguration(cfg => cfg.AddProfile<ProductMappingProfile>());
        _mapper = config.CreateMapper();
        
        _controller = new ProductsController(
            logger,
            _context,
            _mapper
        );
        
        // Setup JWT claims
        _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        _controller.ControllerContext.HttpContext.User = JwtClaimsHelper.GetClaims(user.Id, user.Role.Name);
        Assert.NotNull(_controller.User);
        
        // Modify PRODUCT

        product.Name = "modifedProductName";

        // Act
        var result = await _controller.Put(product);
        
        // Assert
        Assert.NotNull(result);
        
        var okResult = result.Result as OkObjectResult;
        var valueResult = okResult!.Value as Product;
        
        Assert.NotNull(valueResult);
        Assert.Equal(product, valueResult);
    }

    [Fact]
    public async Task PatchProduct_ShouldReturnModifiedProduct()
    {
        // Arrange
        var id = _random.Next(1,3);
        var user = TestValues.Users.First(u => u.Id == 3); //seller
        var product = TestValues.Products.First(p => p.Id == id);
        
        //Setup DB context
        _context = _fixture.ApplicationContext;

        //Setup logger
        var logger = new Logger<ProductsController>(new LoggerFactory());
        
        //Setup AutoMapper config
        var config = new MapperConfiguration(cfg => cfg.AddProfile<ProductMappingProfile>());
        _mapper = config.CreateMapper();
        
        _controller = new ProductsController(
            logger,
            _context,
            _mapper
        );
        
        // Setup JWT claims
        _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        _controller.ControllerContext.HttpContext.User = JwtClaimsHelper.GetClaims(user.Id, user.Role.Name);
        Assert.NotNull(_controller.User);
        
        // Setup Patch
        
        var productPatch = new JsonPatchDocument<Product>();

        productPatch.Add(u => u.Name, "somemodifiedname");
        product.Name = "somemodifiedname";

        // Act
        var result = await _controller.Patch(productPatch, id);
        
        // Assert
        Assert.NotNull(result);
        
        var okResult = result.Result as OkObjectResult;
        var valueResult = okResult!.Value as Product;
        
        Assert.NotNull(valueResult);
        Assert.Equal(product.Name, valueResult.Name);
        Assert.Equal(product.Id, valueResult.Id);
        Assert.Equal(product.Cost, valueResult.Cost);
        Assert.Equal(product.Discount, valueResult.Discount);
        Assert.Equal(product.Rating, valueResult.Rating);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnNull()
    {
        // Arrange
        var id = _random.Next(1,3);
        var user = TestValues.Users.First(u => u.Id == 3); //seller
        var product = TestValues.Products.First(p => p.Id == id);
        
        //Setup DB context
        _context = _fixture.ApplicationContext;

        //Setup logger
        var logger = new Logger<ProductsController>(new LoggerFactory());
        
        //Setup AutoMapper config
        var config = new MapperConfiguration(cfg => cfg.AddProfile<ProductMappingProfile>());
        _mapper = config.CreateMapper();
        
        _controller = new ProductsController(
            logger,
            _context,
            _mapper
        );
        
        // Setup JWT claims
        _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        _controller.ControllerContext.HttpContext.User = JwtClaimsHelper.GetClaims(user.Id, user.Role.Name);
        Assert.NotNull(_controller.User);

        // Act
        var result = await _controller.Delete(id);
        
        // Assert
        Assert.NotNull(result);
        
        var okResult = result.Result as NoContentResult;
        
        Assert.Null(await _context.Products.FirstOrDefaultAsync(u => u.Id == id));

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }
}