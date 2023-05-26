using System.Web.Http.Results;
using AnySoftBackend.Library.DataTransferObjects.Order;
using AnySoftBackend.Library.DataTransferObjects.User;
using AnySoftBackend.Library.Misc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AnySoftBackend.Controllers;
using AnySoftBackend.Helpers;
using AnySoftBackend.Library.DataTransferObjects.Product;
using AnySoftBackend.Mappings;
using AnySoftBackend.Models;
using AnySoftBackend.Tests;
using AnySoftBackend.Tests.Helpers;
using Xunit;
using BadRequestResult = Microsoft.AspNetCore.Mvc.BadRequestResult;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace RPM_Project_Backend.Tests.Tests;

[Collection("ApplicationContext Collection")]
public class TestUsersController
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
        var userResponseDto = new UserResponseDto()
        {
            Id = user.Id,
            Email = user.Email,
            Login = user.Login,
            Orders = new List<OrderResponseDto>(),
            ShoppingCart = new List<ProductResponseDto>(),
            Image = ""
        };
        
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
        var valueResult = okResult!.Value as UserResponseDto;
        
        Assert.NotNull(valueResult);
        Assert.Equal(JsonConvert.SerializeObject(userResponseDto), JsonConvert.SerializeObject(valueResult));
    }
    
    [Theory]
    [InlineData("", "", "")]
    [InlineData("", "", "examplePassword")]
    [InlineData("exampleMail", "", "")]
    [InlineData("", "exampleLogin", "examplePassword")]
    [InlineData("exampleMail", "", "examplePassword")]
    public async Task PostUserTheory_ShouldReturnSameUser(string email, string login, string password)
    {
        // Arrange
        var userDto = new UserCreateDto()
        {
            Email = email,
            Login = login,
            Password = password
        };
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

        // Act
        var result = await _controller.Post(userDto);

        // Assert
        Assert.NotNull(result);

        var okResult = result.Result as ObjectResult;
        if (okResult is CreatedResult)
        {
            var valueResult = okResult!.Value as User;
            Assert.NotNull(valueResult);
        
            Assert.NotEqual(PasswordVerificationResult.Failed, hasher.VerifyHashedPassword(valueResult, valueResult.Password, userDto.Password));
            Assert.Equal(userDto.Email, valueResult.Email);
            Assert.Equal(userDto.Login, valueResult.Login);
        }
        else if (okResult is BadRequestResult)
        {
            var valueResult = okResult!.Value as ErrorModel;
            Assert.NotNull(valueResult);
        }
        else if (okResult is InternalServerErrorResult)
        {
            var valueResult = okResult!.Value as ErrorModel;
            Assert.NotNull(valueResult);
        }
    }
    
    [Fact]
    public async Task PostUserFact_ShouldReturnSameUser()
    {
        // Arrange
        var user = TestValues.SingleUser;
        
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
        
        var userFields = _mapper.Map<UserCreateDto>(user);
        
        // Act
        var result = await _controller.Post(userFields);

        // Assert
        Assert.NotNull(result);

        var okResult = result.Result as ObjectResult;
        var valueResult = okResult!.Value as UserResponseDto;

        Assert.NotNull(valueResult);
        
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
        var userEditDto = new UserEditDto()
        {
            Id = user.Id,
            Email = user.Email,
            Login = user.Login,
            Password = user.Password
        };

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

        userEditDto.Email = "modifed@gmail.com";

        // Act
        var result = await _controller.Put(userEditDto);
        
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
    public async Task DeleteUser_ShouldReturnNull()
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
        _context.Reviews.Add(TestValues.Reviews.First(r => r.UserId == user.Id));
        await _context.SaveChangesAsync();
    }
}