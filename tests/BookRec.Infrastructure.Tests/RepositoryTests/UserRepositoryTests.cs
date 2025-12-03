using BookRec.Domain.UserModel;
using BookRec.Infrastructure.Dbos;
using BookRec.Infrastructure.Persistence;
using BookRec.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookRec.Infrastructure.Tests.Repositories;

public class UserRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _repository = new UserRepository(_context);
    }

    public void Dispose() => _context.Dispose();

    // Helper to create valid UserDBO
    private UserDBO CreateValidUserDBO(Guid? id = null, string? username = null, string? email = null)
    {
        return new UserDBO
        {
            Id = id ?? Guid.NewGuid(),
            Username = username ?? "testuser1",
            FirstName = "Test",
            LastName = "User",
            Email = email ?? $"{Guid.NewGuid()}@test.com",
            PreferredGenres = "Fiction"
        };
    }

    // Helper to create valid User
    private User CreateValidUser(Guid? id = null, string? username = null, string? email = null)
    {
        return new User(
            id ?? Guid.NewGuid(),
            username ?? "testuser1",
            "Test",
            "User",
            email ?? $"{Guid.NewGuid()}@test.com",
            new List<string> { "Fiction" },
            DateTime.UtcNow
        );
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsUser_WhenExists()
    {
        var id = Guid.NewGuid();
        _context.Users.Add(CreateValidUserDBO(id: id, username: "findme123"));
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal("findme123", result.Username);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsUser_WhenExists()
    {
        var email = "findme@example.com";
        _context.Users.Add(CreateValidUserDBO(email: email));
        await _context.SaveChangesAsync();

        var result = await _repository.GetByEmailAsync(email);

        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsNull_WhenNotExists()
    {
        var result = await _repository.GetByEmailAsync("notfound@example.com");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserAsync_ReturnsUser_WhenExists()
    {
        _context.Users.Add(CreateValidUserDBO(username: "uniqueuser1"));
        await _context.SaveChangesAsync();

        var result = await _repository.GetByUserAsync("uniqueuser1");

        Assert.NotNull(result);
        Assert.Equal("uniqueuser1", result.Username);
    }

    [Fact]
    public async Task GetByUserAsync_ReturnsNull_WhenNotExists()
    {
        var result = await _repository.GetByUserAsync("notfound1");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllUsers_OrderedById()
    {
        var id1 = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var id2 = Guid.Parse("00000000-0000-0000-0000-000000000002");
        _context.Users.Add(CreateValidUserDBO(id: id2, username: "usertwo22", email: "two@test.com"));
        _context.Users.Add(CreateValidUserDBO(id: id1, username: "userone11", email: "one@test.com"));
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal(id1, result[0].Id);
        Assert.Equal(id2, result[1].Id);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty_WhenNoUsers()
    {
        var result = await _repository.GetAllAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task AddAsync_AddsUser()
    {
        var user = CreateValidUser(username: "newuser123", email: "new@example.com");

        await _repository.AddAsync(user);

        var result = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        Assert.NotNull(result);
        Assert.Equal("newuser123", result.Username);
    }

    [Fact]
    public async Task AddAsync_SetsCreatedAtTimestamp()
    {
        var before = DateTime.UtcNow;
        var user = CreateValidUser();

        await _repository.AddAsync(user);
        var after = DateTime.UtcNow;

        var result = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        Assert.True(result!.createdAt >= before && result.createdAt <= after);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesUser_WhenExists()
    {
        var id = Guid.NewGuid();
        _context.Users.Add(CreateValidUserDBO(id: id, username: "original1", email: "orig@test.com"));
        await _context.SaveChangesAsync();

        var updated = new User(id, "updated123", "Updated", "User", "updated@test.com", 
            new List<string> { "Mystery" }, DateTime.UtcNow);
        await _repository.UpdateAsync(updated);

        var result = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        Assert.Equal("updated123", result!.Username);
        Assert.Equal("updated@test.com", result.Email);
        Assert.Equal("Mystery", result.PreferredGenres);
    }

    [Fact]
    public async Task UpdateAsync_DoesNothing_WhenNotExists()
    {
        var user = CreateValidUser();

        await _repository.UpdateAsync(user); // Should not throw

        Assert.Empty(await _context.Users.ToListAsync());
    }

    [Fact]
    public async Task DeleteAsync_RemovesUser_WhenExists()
    {
        var id = Guid.NewGuid();
        _context.Users.Add(CreateValidUserDBO(id: id));
        await _context.SaveChangesAsync();

        await _repository.DeleteAsync(id);

        Assert.Null(await _context.Users.FirstOrDefaultAsync(u => u.Id == id));
    }

    [Fact]
    public async Task DeleteAsync_DoesNothing_WhenNotExists()
    {
        await _repository.DeleteAsync(Guid.NewGuid()); // Should not throw

        Assert.Empty(await _context.Users.ToListAsync());
    }
}