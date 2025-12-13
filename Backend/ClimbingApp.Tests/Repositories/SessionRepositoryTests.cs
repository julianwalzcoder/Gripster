using System;
using System.Collections.Generic;
using ClimbingApp.Model.Entities;
using ClimbingApp.Model.Repositories;
using Microsoft.Extensions.Configuration;
using Xunit;
using Moq;

namespace ClimbingApp.Tests.Repositories
{
    public class SessionRepositoryTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly SessionRepository _repository;

        public SessionRepositoryTests()
        {
            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.Setup(c => c.GetConnectionString("DefaultConnection"))
                .Returns("Host=localhost;Database=testdb;Username=test;Password=test");
            _repository = new SessionRepository(_mockConfig.Object);
        }

        [Fact]
        public void GetSessionById_ValidId_ReturnsSession()
        {
            // Arrange
            int testId = 1;

            // Act
            var result = _repository.GetSessionById(testId);

            // Assert
            // Note: This requires actual DB or mocking DbConnection
            Assert.NotNull(result);
        }

        [Fact]
        public void GetSessions_ReturnsListOfSessions()
        {
            // Act
            var result = _repository.GetSessions();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<Session>>(result);
        }

        [Fact]
        public void GetSessionsByUser_ValidUserId_ReturnsSessions()
        {
            // Arrange
            int userId = 8;

            // Act
            var result = _repository.GetSessionsByUser(userId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<Session>>(result);
        }

        [Fact]
        public void InsertSession_ValidSession_ReturnsTrue()
        {
            // Arrange
            var session = new Session
            {
                UserID = 8,
                RouteID = 15,
                Status = "Flash",
                LoggedAt = DateTime.Now
            };

            // Act
            var result = _repository.InsertSession(session);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void UpdateSession_ValidSession_ReturnsTrue()
        {
            // Arrange
            var session = new Session
            {
                ID = 1,
                UserID = 8,
                RouteID = 15,
                Status = "Top",
                LoggedAt = DateTime.Now
            };

            // Act
            var result = _repository.UpdateSession(session);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void DeleteSession_ValidId_ReturnsTrue()
        {
            // Arrange
            int sessionId = 1;

            // Act
            var result = _repository.DeleteSession(sessionId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CreateSession_ValidData_ReturnsNewId()
        {
            // Arrange
            int userId = 8;
            int routeId = 15;
            string status = "Flash";
            DateTime loggedAt = DateTime.Now;

            // Act
            var result = _repository.CreateSession(userId, routeId, status, loggedAt);

            // Assert
            Assert.True(result > 0);
        }
    }
}
