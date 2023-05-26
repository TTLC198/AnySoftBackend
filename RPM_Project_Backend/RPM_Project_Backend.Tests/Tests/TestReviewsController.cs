using System.Text.Json;
using System.Web.Http.Results;
using AnySoftBackend.Library.DataTransferObjects.Review;
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
using RPM_Project_Backend.Controllers;
using RPM_Project_Backend.Helpers;
using RPM_Project_Backend.Mappings;
using RPM_Project_Backend.Models;
using RPM_Project_Backend.Tests.Helpers;
using Xunit;
using BadRequestResult = Microsoft.AspNetCore.Mvc.BadRequestResult;

namespace RPM_Project_Backend.Tests.Tests;

[Collection("ApplicationContext Collection")]
public class TestReviewsController
{
    private readonly ApplicationContextDataFixture _fixture;
    private ReviewsController _controller;
    private ApplicationContext _context;
    private IMapper _mapper;
    private readonly Random _random = new Random();
    
    public TestReviewsController(ApplicationContextDataFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetReviews_EmptyQuery_ShouldReturnSameReviewsList()
    {
        // Arrange
        //Setup DB context
        _context = _fixture.ApplicationContext;

        //Setup logger
        var logger = new Logger<ReviewsController>(new LoggerFactory());
        
        //Setup AutoMapper config
        var config = new MapperConfiguration(cfg => cfg.AddProfile<ReviewMappingProfile>());
        _mapper = config.CreateMapper();
        
        _controller = new ReviewsController(
            logger,
            _context,
            _mapper
        );
        // Setup JWT claims
        _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        _controller.ControllerContext.HttpContext.User = JwtClaimsHelper.GetClaims(_random.Next(1, 3), "admin");
        Assert.NotNull(_controller.User);
        var reviewResponseDtos = _context.Reviews
            .Include(r => r.User)
            .OrderByDescending(r => r.Ts)
            .AsQueryable()
            .Select(r => new ReviewResponseDto()
            {
                Id = r.Id,
                Text = r.Text,
                Grade = r.Grade,
                Ts = r.Ts,
                ProductId = r.ProductId,
                User = new UserResponseDto()
                {
                    Id = r.User.Id,
                    Login = r.User.Login,
                    Image = ImageUriHelper.GetImagePathAsUri(r.User.Images.FirstOrDefault().ImagePath)
                }
            });

        // Act
        var result = await _controller.Get( null, new QueryParameters<object>());

        // Assert
        Assert.NotNull(result);
        
        var okResult = result.Result as OkObjectResult;
        var valueResult = okResult!.Value as IEnumerable<ReviewResponseDto>;
        
        Assert.NotNull(okResult!.Value);
        Assert.Equal(JsonConvert.SerializeObject(reviewResponseDtos.ToList()), JsonConvert.SerializeObject(valueResult!.ToList()));
    }
    
    [Fact]
    public async Task GetReviewsByProductID_EmptyQuery_ShouldReturnSameReviewsList()
    {
        // Arrange
        var id = _random.Next(1, 3);
        var product = TestValues.Products.First(u => u.Id == id);
        
        //Setup DB context
        _context = _fixture.ApplicationContext;

        //Setup logger
        var logger = new Logger<ReviewsController>(new LoggerFactory());
        
        //Setup AutoMapper config
        var config = new MapperConfiguration(cfg => cfg.AddProfile<ReviewMappingProfile>());
        _mapper = config.CreateMapper();
        
        _controller = new ReviewsController(
            logger,
            _context,
            _mapper
        );
        // Setup JWT claims
        _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        _controller.ControllerContext.HttpContext.User = JwtClaimsHelper.GetClaims(_random.Next(1, 3), "admin");
        Assert.NotNull(_controller.User);
        var reviewResponseDtos = _context.Reviews
            .Include(r => r.User)
            .OrderByDescending(r => r.Ts)
            .AsQueryable()
            .Where(r => r.ProductId == id)
            .Select(r => new ReviewResponseDto()
            {
                Id = r.Id,
                Text = r.Text,
                Grade = r.Grade,
                Ts = r.Ts,
                ProductId = r.ProductId,
                User = new UserResponseDto()
                {
                    Id = r.User.Id,
                    Login = r.User.Login,
                    Image = ImageUriHelper.GetImagePathAsUri(r.User.Images.FirstOrDefault().ImagePath)
                }
            });

        // Act
        var result = await _controller.Get(id , new QueryParameters<object>());

        // Assert
        Assert.NotNull(result);
        
        var okResult = result.Result as OkObjectResult;
        var valueResult = okResult!.Value as IEnumerable<ReviewResponseDto>;
        
        Assert.NotNull(okResult!.Value);
        Assert.Equal(JsonConvert.SerializeObject(reviewResponseDtos.ToList()), JsonConvert.SerializeObject(valueResult!.ToList()));
    }
    
    [Theory]
    [InlineData("", 0, 0)]
    [InlineData("", 2, 0)]
    [InlineData("example text", 2, 1)]
    [InlineData("", 2, 1)]
    [InlineData("exampleMail", 0, 1)]
    public async Task PostReviewTheory_ShouldReturnSameReview(string text, int grade, int productId)
    {
        // Arrange
        var id = _random.Next(1, 3);
        var user = TestValues.Users.First(u => u.Id == id);
        var reviewDto = new ReviewCreateDto()
        {
            Text = text,
            Grade = grade,
            ProductId = productId
        };
        
        //Setup DB context
        _context = _fixture.ApplicationContext;

        //Setup logger
        var logger = new Logger<ReviewsController>(new LoggerFactory());
        
        //Setup AutoMapper config
        var config = new MapperConfiguration(cfg => cfg.AddProfile<ReviewMappingProfile>());
        _mapper = config.CreateMapper();
        
        _controller = new ReviewsController(
            logger,
            _context,
            _mapper
        );
        
        // Setup JWT claims
        _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        _controller.ControllerContext.HttpContext.User = JwtClaimsHelper.GetClaims(id, user.Role.Name);
        Assert.NotNull(_controller.User);

        // Act
        var result = await _controller.Post(reviewDto);

        var okResult = result.Result as ObjectResult;
        if (okResult is CreatedResult)
        {
            var valueResult = okResult!.Value as ReviewResponseDto;
            Assert.NotNull(valueResult);
            
            Assert.Equal(reviewDto.Text, valueResult.Text);
            Assert.Equal(reviewDto.Grade, valueResult.Grade);
            Assert.Equal(reviewDto.ProductId, valueResult.ProductId);
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
    public async Task PostReviewFact_ShouldReturnSameReview()
    {
        // Arrange
        var id = _random.Next(1, 3);
        var user = TestValues.Users.First(u => u.Id == id);
        var review = TestValues.SingleReview;
        var reviewDto = new ReviewCreateDto()
        {
            Text = review.Text,
            Grade = review.Grade,
            ProductId = review.ProductId
        };
        
        //Setup DB context
        _context = _fixture.ApplicationContext;

        //Setup logger
        var logger = new Logger<ReviewsController>(new LoggerFactory());
        
        //Setup AutoMapper config
        var config = new MapperConfiguration(cfg => cfg.AddProfile<ReviewMappingProfile>());
        _mapper = config.CreateMapper();
        
        _controller = new ReviewsController(
            logger,
            _context,
            _mapper
        );
        
        // Setup JWT claims
        _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        _controller.ControllerContext.HttpContext.User = JwtClaimsHelper.GetClaims(id, user.Role.Name);
        Assert.NotNull(_controller.User);

        // Act
        var result = await _controller.Post(reviewDto);

        // Assert
        Assert.NotNull(result);

        var okResult = result.Result as ObjectResult;
        var valueResult = okResult!.Value as ReviewResponseDto;

        Assert.NotNull(valueResult);
        
        Assert.Equal(review.Text, valueResult.Text);
        Assert.Equal(review.Grade, valueResult.Grade);
        Assert.Equal(review.ProductId, valueResult.ProductId);
    }
    
    [Fact]
    public async Task PutReview_ShouldReturnModifiedReview()
    {
        // Arrange
        var id = _random.Next(1, 3);
        var user = TestValues.Users.First(u => u.Id == id);
        var review = TestValues.Reviews.First(u => u.UserId == user.Id);
        var reviewEditDto = new ReviewEditDto()
        {
            Id = review.Id,
            Grade = review.Grade,
            Text = review.Text
        };
        
        //Setup DB context
        _context = _fixture.ApplicationContext;

        //Setup logger
        var logger = new Logger<ReviewsController>(new LoggerFactory());
        
        //Setup AutoMapper config
        var config = new MapperConfiguration(cfg => cfg.AddProfile<ReviewMappingProfile>());
        _mapper = config.CreateMapper();
        
        _controller = new ReviewsController(
            logger,
            _context,
            _mapper
        );
        
        // Setup JWT claims
        _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        _controller.ControllerContext.HttpContext.User = JwtClaimsHelper.GetClaims(id, user.Role.Name);
        Assert.NotNull(_controller.User);
        
        // Modify USER

        reviewEditDto.Text = "modifed text";

        // Act
        var result = await _controller.Put(reviewEditDto);
        
        // Assert
        Assert.NotNull(result);
        
        var okResult = result.Result as OkObjectResult;
        var valueResult = okResult!.Value as Review;
        
        Assert.NotNull(valueResult);
        Assert.Equal(review, valueResult);
    }

    [Fact]
    public async Task DeleteReview_ShouldReturnNull()
    {
        // Arrange
        var id = _random.Next(1, 3);
        var user = TestValues.Users.First(u => u.Id == id);
        var review = TestValues.Reviews.First(u => u.UserId == user.Id);
        
        //Setup DB context
        _context = _fixture.ApplicationContext;

        //Setup logger
        var logger = new Logger<ReviewsController>(new LoggerFactory());
        
        //Setup AutoMapper config
        var config = new MapperConfiguration(cfg => cfg.AddProfile<ReviewMappingProfile>());
        _mapper = config.CreateMapper();
        
        _controller = new ReviewsController(
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

        var temp = await _context.Reviews.FirstOrDefaultAsync(u => u.Id == id);
        Assert.Null(await _context.Reviews.FirstOrDefaultAsync(u => u.Id == id));

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
    }
}