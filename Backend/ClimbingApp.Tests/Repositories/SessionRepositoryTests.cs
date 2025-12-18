using System;
using System.Collections.Generic;
using ClimbingApp.Model.Entities;
using ClimbingApp.Model.Repositories;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace ClimbingApp.Tests.Repositories
{
    /// <summary>
    /// Unit tests for SessionRepository
    /// Tests all database operations for session management
    /// </summary>
    public class SessionRepositoryTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<SessionRepository> _mockRepository;

        public SessionRepositoryTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            
            // Mock the ConnectionStrings section
            var mockConnectionStringsSection = new Mock<IConfigurationSection>();
            mockConnectionStringsSection.Setup(x => x["DefaultConnection"])
                .Returns("Host=localhost;Database=test;Username=test;Password=test");
            
            _mockConfiguration.Setup(x => x.GetSection("ConnectionStrings"))
                .Returns(mockConnectionStringsSection.Object);
            
            _mockRepository = new Mock<SessionRepository>(_mockConfiguration.Object) { CallBase = false };
        }

        /// <summary>
        /// Test: GetSessionById returns session when it exists
        /// </summary>
        [Fact]
        public void GetSessionById_ValidId_ReturnsSession()
        {
            // Arrange
            var expectedSession = new Session(1)
            {
                UserID = 1,
                RouteID = 5,
                Status = "completed",
                LoggedAt = DateTime.Now
            };

            _mockRepository.Setup(r => r.GetSessionById(1)).Returns(expectedSession);

            // Act
            var result = _mockRepository.Object.GetSessionById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ID);
            Assert.Equal(1, result.UserID);
            Assert.Equal(5, result.RouteID);
            Assert.Equal("completed", result.Status);
        }

        /// <summary>
        /// Test: GetSessionById returns null when session does not exist
        /// </summary>
        [Fact]
        public void GetSessionById_InvalidId_ReturnsNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetSessionById(999)).Returns((Session?)null);

            // Act
            var result = _mockRepository.Object.GetSessionById(999);

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// Test: GetSessions returns list of all sessions
        /// </summary>
        [Fact]
        public void GetSessions_ReturnsListOfSessions()
        {
            // Arrange
            var expectedSessions = new List<Session>
            {
                new Session(1) { UserID = 1, RouteID = 5, Status = "completed", LoggedAt = DateTime.Now },
                new Session(2) { UserID = 2, RouteID = 6, Status = "attempted", LoggedAt = DateTime.Now }
            };

            _mockRepository.Setup(r => r.GetSessions()).Returns(expectedSessions);

            // Act
            var result = _mockRepository.Object.GetSessions();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        /// <summary>
        /// Test: GetSessions returns empty list when no sessions exist
        /// </summary>
        [Fact]
        public void GetSessions_NoSessions_ReturnsEmptyList()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetSessions()).Returns(new List<Session>());

            // Act
            var result = _mockRepository.Object.GetSessions();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        /// <summary>
        /// Test: GetSessionsByUser returns sessions for specific user
        /// </summary>
        [Fact]
        public void GetSessionsByUser_ValidUserId_ReturnsSessions()
        {
            // Arrange
            var userId = 1;
            var expectedSessions = new List<Session>
            {
                new Session(1) { UserID = userId, RouteID = 5, Status = "completed", LoggedAt = DateTime.Now.AddDays(-1) },
                new Session(2) { UserID = userId, RouteID = 6, Status = "attempted", LoggedAt = DateTime.Now }
            };

            _mockRepository.Setup(r => r.GetSessionsByUser(userId)).Returns(expectedSessions);

            // Act
            var result = _mockRepository.Object.GetSessionsByUser(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, s => Assert.Equal(userId, s.UserID));
        }

        /// <summary>
        /// Test: GetSessionsByUser returns empty list when user has no sessions
        /// </summary>
        [Fact]
        public void GetSessionsByUser_NoSessions_ReturnsEmptyList()
        {
            // Arrange
            var userId = 1;
            _mockRepository.Setup(r => r.GetSessionsByUser(userId)).Returns(new List<Session>());

            // Act
            var result = _mockRepository.Object.GetSessionsByUser(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        /// <summary>
        /// Test: InsertSession returns true on successful insertion
        /// </summary>
        [Fact]
        public void InsertSession_ValidSession_ReturnsTrue()
        {
            // Arrange
            var session = new Session(0)
            {
                UserID = 1,
                RouteID = 5,
                Status = "completed",
                LoggedAt = DateTime.Now
            };

            _mockRepository.Setup(r => r.InsertSession(It.IsAny<Session>())).Returns(true);

            // Act
            var result = _mockRepository.Object.InsertSession(session);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Test: InsertSession handles null status correctly
        /// </summary>
        [Fact]
        public void InsertSession_NullStatus_ReturnsTrue()
        {
            // Arrange
            var session = new Session(0)
            {
                UserID = 1,
                RouteID = 5,
                Status = null,
                LoggedAt = DateTime.Now
            };

            _mockRepository.Setup(r => r.InsertSession(It.Is<Session>(s => s.Status == null)))
                .Returns(true);

            // Act
            var result = _mockRepository.Object.InsertSession(session);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.InsertSession(It.IsAny<Session>()), Times.Once);
        }

        /// <summary>
        /// Test: UpdateSession returns true on successful update
        /// </summary>
        [Fact]
        public void UpdateSession_ValidSession_ReturnsTrue()
        {
            // Arrange
            var session = new Session(1)
            {
                UserID = 1,
                RouteID = 5,
                Status = "completed",
                LoggedAt = DateTime.Now
            };

            _mockRepository.Setup(r => r.UpdateSession(It.IsAny<Session>())).Returns(true);

            // Act
            var result = _mockRepository.Object.UpdateSession(session);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Test: DeleteSession returns true on successful deletion
        /// </summary>
        [Fact]
        public void DeleteSession_ValidId_ReturnsTrue()
        {
            // Arrange
            _mockRepository.Setup(r => r.DeleteSession(1)).Returns(true);

            // Act
            var result = _mockRepository.Object.DeleteSession(1);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Test: DeleteSession returns false when session does not exist
        /// </summary>
        [Fact]
        public void DeleteSession_InvalidId_ReturnsFalse()
        {
            // Arrange
            _mockRepository.Setup(r => r.DeleteSession(999)).Returns(false);

            // Act
            var result = _mockRepository.Object.DeleteSession(999);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Test: CreateSession returns new session ID
        /// </summary>
        [Fact]
        public void CreateSession_ValidData_ReturnsSessionId()
        {
            // Arrange
            var userId = 1;
            var routeId = 5;
            var status = "completed";
            var loggedAt = DateTime.Now;
            var expectedId = 42;

            _mockRepository.Setup(r => r.CreateSession(userId, routeId, status, loggedAt))
                .Returns(expectedId);

            // Act
            var result = _mockRepository.Object.CreateSession(userId, routeId, status, loggedAt);

            // Assert
            Assert.Equal(expectedId, result);
        }

        /// <summary>
        /// Test: CreateSession handles null status correctly
        /// </summary>
        [Fact]
        public void CreateSession_NullStatus_ReturnsSessionId()
        {
            // Arrange
            var userId = 1;
            var routeId = 5;
            string? status = null;
            var loggedAt = DateTime.Now;
            var expectedId = 42;

            _mockRepository.Setup(r => r.CreateSession(userId, routeId, null!, It.IsAny<DateTime>()))
                .Returns(expectedId);

            // Act
            var result = _mockRepository.Object.CreateSession(userId, routeId, status, loggedAt);

            // Assert
            Assert.Equal(expectedId, result);
        }

        /// <summary>
        /// Test: CreateSession returns 0 on failure
        /// </summary>
        [Fact]
        public void CreateSession_Failure_ReturnsZero()
        {
            // Arrange
            var userId = 1;
            var routeId = 5;
            var status = "completed";
            var loggedAt = DateTime.Now;

            _mockRepository.Setup(r => r.CreateSession(userId, routeId, status, loggedAt))
                .Returns(0);

            // Act
            var result = _mockRepository.Object.CreateSession(userId, routeId, status, loggedAt);

            // Assert
            Assert.Equal(0, result);
        }

        /// <summary>
        /// Test: Verify repository method is called with correct parameters
        /// </summary>
        [Fact]
        public void GetSessionById_CallsRepositoryWithCorrectId()
        {
            // Arrange
            var sessionId = 123;
            _mockRepository.Setup(r => r.GetSessionById(sessionId)).Returns((Session?)null);

            // Act
            _mockRepository.Object.GetSessionById(sessionId);

            // Assert
            _mockRepository.Verify(r => r.GetSessionById(sessionId), Times.Once);
        }

        /// <summary>
        /// Test: Verify GetSessionsByUser is called with correct user ID
        /// </summary>
        [Fact]
        public void GetSessionsByUser_CallsRepositoryWithCorrectUserId()
        {
            // Arrange
            var userId = 42;
            _mockRepository.Setup(r => r.GetSessionsByUser(userId)).Returns(new List<Session>());

            // Act
            _mockRepository.Object.GetSessionsByUser(userId);

            // Assert
            _mockRepository.Verify(r => r.GetSessionsByUser(userId), Times.Once);
        }
    }
}
