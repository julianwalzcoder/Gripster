using Xunit;
using Moq;
using ClimbingApp.API.Controllers;
using ClimbingApp.Model.Entities;
using ClimbingApp.Model.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace ClimbingApp.Tests;

public class AuthControllerTests
{
    private readonly Mock<AuthRepository> _authRepoMock;
    private readonly Mock<UserRepository> _userRepoMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly AuthController _controller;
    
    public AuthControllerTests()
    {
        _authRepoMock = new Mock<AuthRepository>(MockBehavior.Default, null);
        _userRepoMock = new Mock<UserRepository>(MockBehavior.Default, null);
        _configMock = new Mock<IConfiguration>();
        _configMock.Setup(c => c["Jwt:Key"]).Returns("test-secret-key-with-at-least-32-characters-for-hmac-sha256");
        _controller = new AuthController(_authRepoMock.Object, _userRepoMock.Object, _configMock.Object);
    }

    [Fact]
    public void Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
    {
        _authRepoMock.Setup(r => r.ValidateUser("invalid", "wrong")).Returns((User)null!);
        var result = _controller.Login(new LoginRequest { Username = "invalid", Password = "wrong" });
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public void Login_ReturnsOk_WhenCredentialsAreValid()
    {
        var user = new User(1) { Username = "testuser", Role = "user" };
        _authRepoMock.Setup(r => r.ValidateUser("testuser", "password")).Returns(user);
        var result = _controller.Login(new LoginRequest { Username = "testuser", Password = "password" });
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void Register_ReturnsBadRequest_WhenUsernameIsEmpty()
    {
        var result = _controller.Register(new RegisterRequest 
        { 
            Username = "", 
            Password = "password",
            Mail = "test@test.com"
        });
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void Register_ReturnsBadRequest_WhenPasswordIsEmpty()
    {
        var result = _controller.Register(new RegisterRequest 
        { 
            Username = "testuser", 
            Password = "",
            Mail = "test@test.com"
        });
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void Register_ReturnsBadRequest_WhenRequestIsNull()
    {
        var result = _controller.Register(null!);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void Register_ReturnsOk_WhenRegistrationSucceeds()
    {
        var user = new User(1) { Username = "newuser", Role = "user" };
        _userRepoMock.Setup(r => r.InsertUser(It.IsAny<User>(), It.IsAny<string>())).Returns(true);
        _authRepoMock.Setup(r => r.ValidateUser("newuser", "password")).Returns(user);
        
        var result = _controller.Register(new RegisterRequest 
        { 
            Username = "newuser", 
            Password = "password",
            Mail = "new@test.com"
        });
        
        Assert.IsType<OkObjectResult>(result);
    }

}
