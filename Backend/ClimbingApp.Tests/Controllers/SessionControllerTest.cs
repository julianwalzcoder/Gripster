using Xunit;
using Moq;
using ClimbingApp.API.Controllers;
using ClimbingApp.Model.Entities;
using ClimbingApp.Model.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClimbingApp.Tests.Controllers
{
    /// <summary>
    /// Unit tests for SessionController
    /// Tests all endpoints for session management operations
    /// </summary>
    public class SessionControllerTests
    {
        private readonly Mock<SessionRepository> _sessionRepoMock;
        private readonly Mock<UserRouteRepository> _userRouteRepoMock;
        private readonly SessionController _controller;
        
        public SessionControllerTests()
        {
            // Initialize mocks with null configuration (mocked behavior doesn't need real config)
            _sessionRepoMock = new Mock<SessionRepository>(null);
            _userRouteRepoMock = new Mock<UserRouteRepository>(null);
            
            // Create controller with mocked dependencies
            _controller = new SessionController(_sessionRepoMock.Object, _userRouteRepoMock.Object);
        }
        
        /// <summary>
        /// Test: GetSession returns Ok with session data when session exists
        /// </summary>
        [Fact]
        public void GetSession_ReturnsOk_WhenSessionExists()
        {
            // Arrange: Create a test session
            var session = new Session(1)
            {
                UserID = 1,
                RouteID = 1,
                Status = "completed",
                LoggedAt = DateTime.Now
            };
            
            // Setup mock to return the test session
            _sessionRepoMock.Setup(r => r.GetSessionById(1)).Returns(session);
            
            // Act: Call the controller method
            var result = _controller.GetSession(1);
            
            // Assert: Verify the result is OkObjectResult
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.Equal(session, okResult.Value);
        }
        
        /// <summary>
        /// Test: GetSession returns NotFound when session does not exist
        /// </summary>
        [Fact]
        public void GetSession_ReturnsNotFound_WhenSessionDoesNotExist()
        {
            // Arrange: Setup mock to return null (session not found)
            _sessionRepoMock.Setup(r => r.GetSessionById(1)).Returns((Session)null);
            
            // Act: Call the controller method
            var result = _controller.GetSession(1);
            
            // Assert: Verify the result is NotFoundResult
            Assert.IsType<NotFoundResult>(result.Result);
        }

        /// <summary>
        /// Test: GetSessions returns Ok with list of all sessions
        /// </summary>
        [Fact]
        public void GetSessions_ReturnsOk_WithListOfSessions()
        {
            // Arrange: Create a list of test sessions
            var sessions = new List<Session>
            {
                new Session(1) { UserID = 1, RouteID = 1, Status = "completed", LoggedAt = DateTime.Now },
                new Session(2) { UserID = 2, RouteID = 2, Status = "attempted", LoggedAt = DateTime.Now }
            };
            
            // Setup mock to return the list of sessions
            _sessionRepoMock.Setup(r => r.GetSessions()).Returns(sessions);
            
            // Act: Call the controller method
            var result = _controller.GetSessions();
            
            // Assert: Verify the result is OkObjectResult with the sessions list
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            var returnedSessions = okResult.Value as IEnumerable<Session>;
            Assert.NotNull(returnedSessions);
            Assert.Equal(2, returnedSessions.Count());
        }

        /// <summary>
        /// Test: GetSessions returns Ok with empty list when no sessions exist
        /// </summary>
        [Fact]
        public void GetSessions_ReturnsOk_WithEmptyList_WhenNoSessionsExist()
        {
            // Arrange: Setup mock to return empty list
            _sessionRepoMock.Setup(r => r.GetSessions()).Returns(new List<Session>());
            
            // Act: Call the controller method
            var result = _controller.GetSessions();
            
            // Assert: Verify the result is OkObjectResult with empty list
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            var returnedSessions = okResult.Value as IEnumerable<Session>;
            Assert.NotNull(returnedSessions);
            Assert.Empty(returnedSessions);
        }

        /// <summary>
        /// Test: GetSessionsByUser returns Ok with list of sessions for specific user
        /// </summary>
        [Fact]
        public void GetSessionsByUser_ReturnsOk_WithUserSessions()
        {
            // Arrange: Create sessions for a specific user
            var userId = 1;
            var sessions = new List<Session>
            {
                new Session(1) { UserID = userId, RouteID = 1, Status = "completed", LoggedAt = DateTime.Now },
                new Session(2) { UserID = userId, RouteID = 2, Status = "attempted", LoggedAt = DateTime.Now }
            };
            
            // Setup mock to return user's sessions
            _sessionRepoMock.Setup(r => r.GetSessionsByUser(userId)).Returns(sessions);
            
            // Act: Call the controller method
            var result = _controller.GetSessionsByUser(userId);
            
            // Assert: Verify the result is OkObjectResult
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult.Value);
        }

        /// <summary>
        /// Test: GetSessionsByUser returns Ok with empty list when user has no sessions
        /// </summary>
        [Fact]
        public void GetSessionsByUser_ReturnsOk_WithEmptyList_WhenUserHasNoSessions()
        {
            // Arrange: Setup mock to return empty list for user
            var userId = 1;
            _sessionRepoMock.Setup(r => r.GetSessionsByUser(userId)).Returns(new List<Session>());
            
            // Act: Call the controller method
            var result = _controller.GetSessionsByUser(userId);
            
            // Assert: Verify the result is OkObjectResult
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult.Value);
        }

        /// <summary>
        /// Test: GetSessionsByUser returns correct count of sessions
        /// </summary>
        [Fact]
        public void GetSessionsByUser_ReturnsCorrectCount()
        {
            // Arrange: Create a session with known values
            var userId = 1;
            var testDate = DateTime.Now;
            var sessions = new List<Session>
            {
                new Session(10) { UserID = userId, RouteID = 5, Status = "completed", LoggedAt = testDate },
                new Session(11) { UserID = userId, RouteID = 6, Status = "attempted", LoggedAt = testDate }
            };
            
            // Setup mock to return the test sessions
            _sessionRepoMock.Setup(r => r.GetSessionsByUser(userId)).Returns(sessions);
            
            // Act: Call the controller method
            var result = _controller.GetSessionsByUser(userId);
            
            // Assert: Verify the correct number of sessions are returned
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            
            // The controller returns an IEnumerable, we need to check if it's enumerable
            var enumerableResult = okResult.Value as System.Collections.IEnumerable;
            Assert.NotNull(enumerableResult);
            
            // Count the items
            var count = 0;
            foreach (var item in enumerableResult)
            {
                count++;
            }
            Assert.Equal(2, count);
        }

        /// <summary>
        /// Test: Verify repository methods are called with correct parameters
        /// </summary>
        [Fact]
        public void GetSessionsByUser_CallsRepositoryWithCorrectUserId()
        {
            // Arrange
            var userId = 42;
            _sessionRepoMock.Setup(r => r.GetSessionsByUser(userId)).Returns(new List<Session>());
            
            // Act
            _controller.GetSessionsByUser(userId);
            
            // Assert: Verify the repository method was called exactly once with correct parameter
            _sessionRepoMock.Verify(r => r.GetSessionsByUser(userId), Times.Once);
        }

        /// <summary>
        /// Test: Verify GetSessionById is called with correct id
        /// </summary>
        [Fact]
        public void GetSession_CallsRepositoryWithCorrectId()
        {
            // Arrange
            var sessionId = 123;
            _sessionRepoMock.Setup(r => r.GetSessionById(sessionId)).Returns((Session?)null);
            
            // Act
            _controller.GetSession(sessionId);
            
            // Assert: Verify the repository method was called exactly once with correct parameter
            _sessionRepoMock.Verify(r => r.GetSessionById(sessionId), Times.Once);
        }

        /// <summary>
        /// Test: Verify GetSessions is called
        /// </summary>
        [Fact]
        public void GetSessions_CallsRepository()
        {
            // Arrange
            _sessionRepoMock.Setup(r => r.GetSessions()).Returns(new List<Session>());
            
            // Act
            _controller.GetSessions();
            
            // Assert: Verify the repository method was called exactly once
            _sessionRepoMock.Verify(r => r.GetSessions(), Times.Once);
        }
    }
}
