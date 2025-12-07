using Moq;
using BookRec.Application.Users.Dtos;
using BookRec.Application.Users.Services;
using BookRec.Domain.UserModel;

namespace BookRec.Application.Tests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepo;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _mockRepo = new Mock<IUserRepository>();
        _service = new UserService(_mockRepo.Object);
    }

    private User CreateValidUser(Guid? id = null) => new(
        id ?? Guid.NewGuid(),
        "testuser",
        "John",
        "Doe",
        "john@test.com",
        new List<string> { "Fiction" },
        DateTime.UtcNow
    );

    [Fact]
    public async Task GetByIdAsync_UserExists_ReturnsUserDto()
    {
        var user = CreateValidUser();
        _mockRepo.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

        var result = await _service.GetByIdAsync(user.Id);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal("testuser", result.Username);
    }

    [Fact]
    public async Task GetByIdAsync_UserNotFound_ReturnsNull()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_UserExists_ReturnsUserDto()
    {
        var user = CreateValidUser();
        _mockRepo.Setup(r => r.GetByEmailAsync("john@test.com")).ReturnsAsync(user);

        var result = await _service.GetByEmailAsync("john@test.com");

        Assert.NotNull(result);
        Assert.Equal("john@test.com", result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_UserNotFound_ReturnsNull()
    {
        _mockRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        var result = await _service.GetByEmailAsync("notfound@test.com");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserAsync_UserExists_ReturnsUserDto()
    {
        var user = CreateValidUser();
        _mockRepo.Setup(r => r.GetByUserAsync("testuser")).ReturnsAsync(user);

        var result = await _service.GetByUserAsync("testuser");

        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
    }

    [Fact]
    public async Task GetByUserAsync_UserNotFound_ReturnsNull()
    {
        _mockRepo.Setup(r => r.GetByUserAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        var result = await _service.GetByUserAsync("notfound");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserPreferredGenresAsync_UserExists_ReturnsGenres()
    {
        var user = CreateValidUser();
        _mockRepo.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

        var result = await _service.GetUserPreferredGenresAsync(user.Id);

        Assert.Single(result);
        Assert.Contains("Fiction", result);
    }

    [Fact]
    public async Task GetUserPreferredGenresAsync_UserNotFound_ReturnsEmptyList()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        var result = await _service.GetUserPreferredGenresAsync(Guid.NewGuid());

        Assert.Empty(result);
    }

    [Fact]
    public async Task AddUser_ValidDto_ReturnsNewGuid()
    {
        var dto = new CreateUserDto("newuser1", "Jane", "Doe", "jane@test.com", new List<string> { "Mystery" }, DateTime.UtcNow);
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var result = await _service.AddUser(dto);

        Assert.NotEqual(Guid.Empty, result);
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_UserExists_UpdatesAndReturnsId()
    {
        var user = CreateValidUser();
        var updateDto = new UpdateUserDto("newusername", null, null, null, null, DateTime.UtcNow);
        _mockRepo.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var result = await _service.UpdateUser(updateDto, user.Id);

        Assert.Equal(user.Id, result);
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_UserNotFound_ThrowsException()
    {
        var updateDto = new UpdateUserDto("newusername", null, null, null, null, DateTime.UtcNow);
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<Exception>(() => _service.UpdateUser(updateDto, Guid.NewGuid()));
    }

    [Fact]
    public async Task DeleteUser_UserExists_CallsDeleteAsync()
    {
        var user = CreateValidUser();
        _mockRepo.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
        _mockRepo.Setup(r => r.DeleteAsync(user.Id)).Returns(Task.CompletedTask);

        await _service.DeleteUser(user.Id);

        _mockRepo.Verify(r => r.DeleteAsync(user.Id), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_UserNotFound_DoesNotCallDelete()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        await _service.DeleteUser(Guid.NewGuid());

        _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }
}