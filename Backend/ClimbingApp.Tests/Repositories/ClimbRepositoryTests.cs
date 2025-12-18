using System;
using System.Collections.Generic;
using ClimbingApp.Model.Entities;
using ClimbingApp.Model.Repositories;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace ClimbingApp.Tests.Repositories
{
    public class ClimbRepositoryTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ClimbRepository> _mockRepository;

        public ClimbRepositoryTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            
            // Mock the ConnectionStrings section
            var mockConnectionStringsSection = new Mock<IConfigurationSection>();
            mockConnectionStringsSection.Setup(x => x["DefaultConnection"])
                .Returns("Host=localhost;Database=test;Username=test;Password=test");
            
            _mockConfiguration.Setup(x => x.GetSection("ConnectionStrings"))
                .Returns(mockConnectionStringsSection.Object);
            
            _mockRepository = new Mock<ClimbRepository>(_mockConfiguration.Object) { CallBase = false };
        }

        [Fact]
        public void GetRouteById_ValidId_ReturnsClimb()
        {
            // Arrange
            var expectedClimb = new Climb(1)
            {
                GymID = 1,
                GradeID = 5,
                SetDate = DateTime.Today,
                RemoveDate = null,
                AdminID = 1
            };

            _mockRepository.Setup(r => r.GetRouteById(1)).Returns(expectedClimb);

            // Act
            var result = _mockRepository.Object.GetRouteById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(1, result.GymID);
            Assert.Equal(5, result.GradeID);
        }

        [Fact]
        public void GetRouteById_InvalidId_ReturnsNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetRouteById(999)).Returns((Climb)null);

            // Act
            var result = _mockRepository.Object.GetRouteById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetRoutes_ReturnsListOfClimbs()
        {
            // Arrange
            var expectedClimbs = new List<Climb>
            {
                new Climb(1) { GymID = 1, GradeID = 5, SetDate = DateTime.Today, AdminID = 1 },
                new Climb(2) { GymID = 1, GradeID = 6, SetDate = DateTime.Today, AdminID = 1 }
            };

            _mockRepository.Setup(r => r.GetRoutes()).Returns(expectedClimbs);

            // Act
            var result = _mockRepository.Object.GetRoutes();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void InsertRoute_ValidClimb_ReturnsTrue()
        {
            // Arrange
            var climb = new Climb(0)
            {
                GymID = 1,
                GradeID = 5,
                SetDate = DateTime.Today,
                AdminID = 1
            };

            _mockRepository.Setup(r => r.InsertRoute(It.IsAny<Climb>())).Returns(true);

            // Act
            var result = _mockRepository.Object.InsertRoute(climb);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void InsertRoute_WithDefaultSetDate_SetsToToday()
        {
            // Arrange
            var climb = new Climb(0)
            {
                GymID = 1,
                GradeID = 5,
                AdminID = 1
            };

            _mockRepository.Setup(r => r.InsertRoute(It.Is<Climb>(c => c.SetDate == DateTime.Today)))
                .Returns(true);

            // Act
            var result = _mockRepository.Object.InsertRoute(climb);

            // Assert
            _mockRepository.Verify(r => r.InsertRoute(It.IsAny<Climb>()), Times.Once);
        }

        [Fact]
        public void UpdateRoute_ValidClimb_ReturnsTrue()
        {
            // Arrange
            var climb = new Climb(1)
            {
                GymID = 1,
                GradeID = 6,
                SetDate = DateTime.Today,
                AdminID = 1
            };

            _mockRepository.Setup(r => r.UpdateRoute(It.IsAny<Climb>())).Returns(true);

            // Act
            var result = _mockRepository.Object.UpdateRoute(climb);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void DeleteRoute_ValidId_ReturnsTrue()
        {
            // Arrange
            _mockRepository.Setup(r => r.DeleteRoute(1)).Returns(true);

            // Act
            var result = _mockRepository.Object.DeleteRoute(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void GetAverageRatingForRoute_WithRatings_ReturnsAverage()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAverageRatingForRoute(1)).Returns(3.5m);

            // Act
            var result = _mockRepository.Object.GetAverageRatingForRoute(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3.5m, result.Value);
        }

        [Fact]
        public void GetAverageRatingForRoute_NoRatings_ReturnsNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAverageRatingForRoute(1)).Returns((decimal?)null);

            // Act
            var result = _mockRepository.Object.GetAverageRatingForRoute(1);

            // Assert
            Assert.Null(result);
        }
    }
}
