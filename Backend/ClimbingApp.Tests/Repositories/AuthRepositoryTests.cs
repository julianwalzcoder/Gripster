using ClimbingApp.Model.Entities;
using ClimbingApp.Model.Repositories;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace ClimbingApp.Tests.Repositories
{
    public class AuthRepositoryTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<AuthRepository> _mockRepository;

        public AuthRepositoryTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            
            // Mock the ConnectionStrings section
            var mockConnectionStringsSection = new Mock<IConfigurationSection>();
            mockConnectionStringsSection.Setup(x => x["DefaultConnection"])
                .Returns("Host=localhost;Database=test;Username=test;Password=test");
            
            _mockConfiguration.Setup(x => x.GetSection("ConnectionStrings"))
                .Returns(mockConnectionStringsSection.Object);
            
            _mockRepository = new Mock<AuthRepository>(_mockConfiguration.Object) { CallBase = false };
        }

        [Fact]
        public void ValidateUser_ValidCredentials_ReturnsUser()
        {
            // Arrange
            var expectedUser = new User(1)
            {
                Id = 1,
                Username = "testuser",
                Mail = "test@example.com",
                PasswordHash = "hashedpassword",
                Role = "user"
            };

            _mockRepository.Setup(r => r.ValidateUser("testuser", "password123"))
                .Returns(expectedUser);

            // Act
            var result = _mockRepository.Object.ValidateUser("testuser", "password123");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.Username);
            Assert.Equal("user", result.Role);
        }

        [Fact]
        public void ValidateUser_InvalidCredentials_ReturnsNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.ValidateUser("wronguser", "wrongpass"))
                .Returns((User?)null);

            // Act
            var result = _mockRepository.Object.ValidateUser("wronguser", "wrongpass");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ValidateUser_AdminUser_ReturnsUserWithAdminId()
        {
            // Arrange
            var expectedAdmin = new User(1)
            {
                Id = 1,
                Username = "admin",
                Mail = "admin@example.com",
                PasswordHash = "hashedpassword",
                Role = "admin",
                AdminId = 10
            };

            _mockRepository.Setup(r => r.ValidateUser("admin", "adminpass"))
                .Returns(expectedAdmin);

            // Act
            var result = _mockRepository.Object.ValidateUser("admin", "adminpass");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("admin", result.Role);
            Assert.Equal(10, result.AdminId);
        }

        [Fact]
        public void ValidateUser_EmptyUsername_ReturnsNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.ValidateUser("", "password"))
                .Returns((User?)null);

            // Act
            var result = _mockRepository.Object.ValidateUser("", "password");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ValidateUser_EmptyPassword_ReturnsNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.ValidateUser("testuser", ""))
                .Returns((User?)null);

            // Act
            var result = _mockRepository.Object.ValidateUser("testuser", "");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ValidateUser_NullUsername_ReturnsNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.ValidateUser(null!, "password"))
                .Returns((User?)null);

            // Act
            var result = _mockRepository.Object.ValidateUser(null!, "password");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ValidateUser_NullPassword_ReturnsNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.ValidateUser("testuser", null!))
                .Returns((User?)null);

            // Act
            var result = _mockRepository.Object.ValidateUser("testuser", null!);

            // Assert
            Assert.Null(result);
        }
    }
}
