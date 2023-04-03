using System.Security.Claims;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.VisualBasic;
using Moq;
using RPM_Project_Backend.Controllers;
using RPM_Project_Backend.Mappings;
using RPM_Project_Backend.Models;
using RPM_Project_Backend.Tests.Helpers;
using Xunit;

namespace RPM_Project_Backend.Tests.Tests;

public class TestUsersController : IClassFixture<ApplicationContextDataFixture>
{
    private readonly ApplicationContextDataFixture _fixture;
    private UsersController _controller;
    private ApplicationContext _context;
    private IMapper _mapper;
    private readonly Random _random = new Random();
    
    public TestUsersController(ApplicationContextDataFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetUsers_EmptyQuery_ShouldReturnSameUsersList()
    {
        // Arrange
        //Setup DB context
        _context = _fixture.ApplicationContext;

        //Setup logger
        var logger = new Logger<UsersController>(new LoggerFactory());
        
        //Setup AutoMapper config
        var config = new MapperConfiguration(cfg => cfg.AddProfile<UserMappingProfile>());
        _mapper = config.CreateMapper();
        
        _controller = new UsersController(
            logger,
            _context,
            _mapper
        );
        // Setup JWT claims
        _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        _controller.ControllerContext.HttpContext.User = JwtClaimsHelper.GetClaims(_random.Next(1, 3), "admin");
        Assert.NotNull(_controller.User);

        // Act
        var result = await _controller.Get(new QueryParameters<User>());

        // Assert
        Assert.NotNull(result);
        
        var okResult = result.Result as OkObjectResult;
        var valueResult = okResult!.Value as IEnumerable<User>;
        
        Assert.NotNull(okResult!.Value);
        Assert.Equal(_context.Users.ToList(), valueResult!.ToList());
    }
    
    [Fact]
    public async Task GetUsers_WithQuery_ShouldReturnSameUsersList()
    {
        // Arrange
        //Setup DB context
        _context = _fixture.ApplicationContext;

        //Setup logger
        var logger = new Logger<UsersController>(new LoggerFactory());
        
        //Setup AutoMapper config
        var config = new MapperConfiguration(cfg => cfg.AddProfile<UserMappingProfile>());
        _mapper = config.CreateMapper();
        
        _controller = new UsersController(
            logger,
            _context,
            _mapper
        );
        // Setup JWT claims
        _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        _controller.ControllerContext.HttpContext.User = JwtClaimsHelper.GetClaims(_random.Next(1, 3), "admin");
        Assert.NotNull(_controller.User);

        // Act
        var result = await _controller.Get(new QueryParameters<User>()
        {
            Query = JsonSerializer.Serialize(new User()
            {
                Login = "ex1"
            })
        });

        // Assert
        Assert.NotNull(result);
        
        var okResult = result.Result as OkObjectResult;
        var valueResult = okResult!.Value as IEnumerable<User>;
        
        Assert.NotNull(okResult!.Value);
        Assert.NotEqual(_context.Users.ToList(), valueResult!.ToList());
    }
    
    [Fact]
    public async Task GetUser_ShouldReturnSameUser()
    {
        // Arrange
        var id = _random.Next(1, 3);
        var user = TestValues.Users.First(u => u.Id == id);
        
        //Setup DB context
        _context = _fixture.ApplicationContext;

        //Setup logger
        var logger = new Logger<UsersController>(new LoggerFactory());
        
        //Setup AutoMapper config
        var config = new MapperConfiguration(cfg => cfg.AddProfile<UserMappingProfile>());
        _mapper = config.CreateMapper();
        
        _controller = new UsersController(
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
        var valueResult = okResult!.Value as User;
        
        Assert.NotNull(valueResult);
        Assert.Equal(user, valueResult);
    }
    
    [Fact]
    public async Task PostUser_ShouldReturnSameUser()
    {
        // Arrange
        var user = TestValues.SingleUser;
        var hasher = new PasswordHasher<User>();
        
        //Setup DB context
        _context = _fixture.ApplicationContext;

        //Setup logger
        var logger = new Logger<UsersController>(new LoggerFactory());
        
        //Setup AutoMapper config
        var config = new MapperConfiguration(cfg => cfg.AddProfile<UserMappingProfile>());
        _mapper = config.CreateMapper();
        
        _controller = new UsersController(
            logger,
            _context,
            _mapper
        );
        
        _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        
        var userFields = _mapper.Map<UserDto>(user);
        
        // Act
        var result = await _controller.Post(userFields);

        // Assert
        Assert.NotNull(result);

        var okResult = result.Result as ObjectResult;
        var valueResult = okResult!.Value as User;

        Assert.NotNull(valueResult);
        
        Assert.NotEqual(PasswordVerificationResult.Failed, hasher.VerifyHashedPassword(user, valueResult.Password, user.Password));
        Assert.Equal(user.Id, valueResult.Id);
        Assert.Equal(user.Email, valueResult.Email);
        Assert.Equal(user.Login, valueResult.Login);
    }
    
    [Fact]
    public async Task PutUser_ShouldReturnModifiedUser()
    {
        // Arrange
        var id = _random.Next(1, 3);
        var user = TestValues.Users.First(u => u.Id == id);
        
        //Setup DB context
        _context = _fixture.ApplicationContext;

        //Setup logger
        var logger = new Logger<UsersController>(new LoggerFactory());
        
        //Setup AutoMapper config
        var config = new MapperConfiguration(cfg => cfg.AddProfile<UserMappingProfile>());
        _mapper = config.CreateMapper();
        
        _controller = new UsersController(
            logger,
            _context,
            _mapper
        );
        
        // Setup JWT claims
        _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        _controller.ControllerContext.HttpContext.User = JwtClaimsHelper.GetClaims(id, user.Role.Name);
        Assert.NotNull(_controller.User);
        
        // Modify USER

        user.Email = "modifed@gmail.com";

        // Act
        var result = await _controller.Put(user);
        
        // Assert
        Assert.NotNull(result);
        
        var okResult = result.Result as OkObjectResult;
        var valueResult = okResult!.Value as User;
        
        Assert.NotNull(valueResult);
        Assert.Equal(user, valueResult);
    }

    [Fact]
    public async Task PatchUser_ShouldReturnModifiedUser()
    {
        // Arrange
        var id = _random.Next(1, 3);
        var user = TestValues.Users.First(u => u.Id == id);
        
        //Setup DB context
        _context = _fixture.ApplicationContext;

        //Setup logger
        var logger = new Logger<UsersController>(new LoggerFactory());
        
        //Setup AutoMapper config
        var config = new MapperConfiguration(cfg => cfg.AddProfile<UserMappingProfile>());
        _mapper = config.CreateMapper();
        
        _controller = new UsersController(
            logger,
            _context,
            _mapper
        );
        
        // Setup JWT claims
        _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        _controller.ControllerContext.HttpContext.User = JwtClaimsHelper.GetClaims(id, user.Role.Name);
        Assert.NotNull(_controller.User);
        
        // Setup Patch
        
        var userPatch = new JsonPatchDocument<User>();

        userPatch.Add(u => u.Email, "somemodifiedmail@email.com");
        user.Email = "somemodifiedmail@email.com";

        // Act
        var result = await _controller.Patch(userPatch, id);
        
        // Assert
        Assert.NotNull(result);
        
        var okResult = result.Result as OkObjectResult;
        var valueResult = okResult!.Value as User;
        
        Assert.NotNull(valueResult);
        Assert.Equal(user, valueResult);
    }

    [Fact]
    public async Task DeleteUser_ShouldReturnModifiedUser()
    {
        // Arrange
        var id = _random.Next(1, 3);
        var user = TestValues.Users.First(u => u.Id == id);
        
        //Setup DB context
        _context = _fixture.ApplicationContext;

        //Setup logger
        var logger = new Logger<UsersController>(new LoggerFactory());
        
        //Setup AutoMapper config
        var config = new MapperConfiguration(cfg => cfg.AddProfile<UserMappingProfile>());
        _mapper = config.CreateMapper();
        
        _controller = new UsersController(
            logger,
            _context,
            _mapper
        );
        
        // Setup JWT claims
        _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        _controller.ControllerContext.HttpContext.User = JwtClaimsHelper.GetClaims(id, user.Role.Name);
        Assert.NotNull(_controller.User);

        // Act
        var result = await _controller.Delete(id);
        
        // Assert
        Assert.NotNull(result);
        
        var okResult = result.Result as NoContentResult;
        
        Assert.Null(await _context.Users.FirstOrDefaultAsync(u => u.Id == id));

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }
}