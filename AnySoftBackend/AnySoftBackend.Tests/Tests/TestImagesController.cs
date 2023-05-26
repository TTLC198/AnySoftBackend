using AnySoftBackend.Controllers;
using AnySoftBackend.Mappings;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RPM_Project_Backend.Tests;
using Xunit;

namespace AnySoftBackend.Tests.Tests;

[Collection("ApplicationContext Collection")]
public class TestImagesController
{
    private readonly ApplicationContextDataFixture _fixture;
    private ImagesController _controller;
    private ApplicationContext _context;
    private IMapper _mapper;
    private readonly Random _random = new Random();
    
    public TestImagesController(ApplicationContextDataFixture fixture)
    {
        _fixture = fixture;
    }

    //[Fact]
    public async Task GetUsers_EmptyQuery_ShouldReturnSameUsersList()
    {
        // Arrange
        var id = _random.Next(1, 3);
        
        //Setup DB context
        _context = _fixture.ApplicationContext;

        //Setup logger
        var logger = new Logger<ImagesController>(new LoggerFactory());
        
        //Setup AutoMapper config
        var config = new MapperConfiguration(cfg => cfg.AddProfile<UserMappingProfile>());
        _mapper = config.CreateMapper();
        
        //Setup Mock Environment
        var mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
        //...Setup the mock as needed
        mockWebHostEnvironment
            .Setup(m => m.WebRootPath)
            .Returns(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName);

        /*_controller = new ImagesController(
            logger,
            _context,
            mockWebHostEnvironment.Object

        );*/

        // Act
        var result = await _controller.Get(id);

        // Assert
        Assert.NotNull(result);
        
        var okResult = result.Result as OkObjectResult;
        var valueResult = okResult!.Value as FileContentResult;
        
        Assert.NotNull(valueResult);
    }
}