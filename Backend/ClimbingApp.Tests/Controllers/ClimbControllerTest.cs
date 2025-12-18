using Xunit;
using Moq;
using ClimbingApp.API.Controllers;
using ClimbingApp.Model.Entities;
using ClimbingApp.Model.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace ClimbingApp.Tests;

public class ClimbControllerTests
{
    private readonly Mock<ClimbRepository> _repoMock;
    private readonly Mock<AdminRepository> _adminRepoMock;
    private readonly ClimbingRouteController _controller;
    
    public ClimbControllerTests()
    {
        _repoMock = new Mock<ClimbRepository>(null);
        _adminRepoMock = new Mock<AdminRepository>(null);
        _controller = new ClimbingRouteController(_repoMock.Object, _adminRepoMock.Object);
    }
    
    [Fact]
    public void GetClimb_ReturnsOk_WhenClimbExists()
    {
        var climb = new Climb(1) { GymID = 1, GradeID = 12, SetDate = DateTime.Now, AdminID = 1 };
        _repoMock.Setup(r => r.GetRouteById(1)).Returns(climb);
        var result = _controller.GetRoute(1);
        Assert.IsType<OkObjectResult>(result.Result);
    }
    
    [Fact]
    public void GetClimb_ReturnsNotFound_WhenClimbDoesNotExist()
    {
        _repoMock.Setup(r => r.GetRouteById(1)).Returns((Climb)null!);
        var result = _controller.GetRoute(1);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public void GetRoutes_ReturnsOk_WithListOfClimbs()
    {
        var climbs = new List<Climb> { new Climb(1), new Climb(2) };
        _repoMock.Setup(r => r.GetRoutes()).Returns(climbs);
        var result = _controller.GetRoutes();
        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public void DeleteRoute_ReturnsNoContent_WhenDeleteSucceeds()
    {
        var climb = new Climb(1);
        _repoMock.Setup(r => r.GetRouteById(1)).Returns(climb);
        _repoMock.Setup(r => r.DeleteRoute(1)).Returns(true);
        var result = _controller.DeleteRoute(1);
        Assert.IsType<NoContentResult>(result);
    }
    
    [Fact]
    public void DeleteRoute_ReturnsNotFound_WhenClimbDoesNotExist()
    {
        _repoMock.Setup(r => r.GetRouteById(1)).Returns((Climb)null!);
        var result = _controller.DeleteRoute(1);
        Assert.IsType<NotFoundObjectResult>(result);
    }
}

