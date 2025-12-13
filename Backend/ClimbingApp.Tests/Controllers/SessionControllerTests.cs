using System;
using System.Collections.Generic;
using System.Linq;
using ClimbingApp.API.Controllers;
using ClimbingApp.Model.Entities;
using ClimbingApp.Model.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ClimbingApp.Tests.Controllers
{
    public class SessionControllerTests
    {
        private readonly Mock<SessionRepository> _mockSessionRepo;
        private readonly Mock<UserRouteRepository> _mockUserRouteRepo;
        private readonly SessionController _controller;

        public SessionControllerTests()
        {
            _mockSessionRepo = new Mock<SessionRepository>(null);
            _mockUserRouteRepo = new Mock<UserRouteRepository>(null);
            _controller = new SessionController(_mockSessionRepo.Object, _mockUserRouteRepo.Object);
        }

        [Fact]
        public void GetSession_ValidId_ReturnsOkWithSession()
        {
            // Arrange
            var session = new Session { ID = 1, UserID = 8, RouteID = 15, Status = "Flash", LoggedAt = DateTime.Now };
            _mockSessionRepo.Setup(r => r.GetSessionById(1)).Returns(session);

            // Act
            var result = _controller.GetSession(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedSession = Assert.IsType<Session>(okResult.Value);
            Assert.Equal(1, returnedSession.ID);
        }

        [Fact]
        public void GetSession_InvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockSessionRepo.Setup(r => r.GetSessionById(999)).Returns((Session)null);

            // Act
            var result = _controller.GetSession(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void GetSessions_ReturnsOkWithSessions()
        {
            // Arrange
            var sessions = new List<Session>
            {
                new Session { ID = 1, UserID = 8, RouteID = 15, Status = "Flash", LoggedAt = DateTime.Now },
                new Session { ID = 2, UserID = 8, RouteID = 16, Status = "Top", LoggedAt = DateTime.Now }
            };
            _mockSessionRepo.Setup(r => r.GetSessions()).Returns(sessions);

            // Act
            var result = _controller.GetSessions();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedSessions = Assert.IsAssignableFrom<IEnumerable<Session>>(okResult.Value);
            Assert.Equal(2, returnedSessions.Count());
        }

        [Fact]
        public void GetSessionsByUser_ValidUserId_ReturnsOkWithSessions()
        {
            // Arrange
            var sessions = new List<Session>
            {
                new Session { ID = 1, UserID = 8, RouteID = 15, Status = "Flash", LoggedAt = DateTime.Now }
            };
            _mockSessionRepo.Setup(r => r.GetSessionsByUser(8)).Returns(sessions);

            // Act
            var result = _controller.GetSessionsByUser(8);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public void DeleteSession_ValidId_ReturnsNoContent()
        {
            // Arrange
            var session = new Session { ID = 1, UserID = 8, RouteID = 15, Status = "Flash", LoggedAt = DateTime.Now };
            _mockSessionRepo.Setup(r => r.GetSessionById(1)).Returns(session);
            _mockSessionRepo.Setup(r => r.DeleteSession(1)).Returns(true);

            // Act
            var result = _controller.DeleteSession(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void DeleteSession_InvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockSessionRepo.Setup(r => r.GetSessionById(999)).Returns((Session)null);

            // Act
            var result = _controller.DeleteSession(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
