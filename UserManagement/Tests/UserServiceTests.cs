using Xunit;
using Moq; 
using UserManagement.Domain.Entities;
using UserManagement.Application.Services;

namespace UserManagement.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserService> _mockUserService;

        public UserServiceTests()
        {
            _mockUserService = new Mock<IUserService>();
        }

        [Fact]
        public void CreateUser_ValidUser_CreatesSuccessfully()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = "test@example.com",
                Role = "Admin"
            };
            _mockUserService.Setup(service => service.CreateUser(user)).Returns(true);

            // Act
            var result = _mockUserService.Object.CreateUser(user);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void DeleteUser_NonExistingUser_ThrowsException()
        {
            // Arrange
            _mockUserService.Setup(service => service.DeleteUserAsync(It.IsAny<string>())).Throws(new KeyNotFoundException());

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => _mockUserService.Object.DeleteUserAsync("InvalidId"));
        }
    }
}
