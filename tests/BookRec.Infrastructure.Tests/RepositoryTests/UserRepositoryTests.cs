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

    [Fact]
    public async Task GetByIdAsync_ReturnsUser_WhenExists()
    {
        var id = Guid.NewGuid();
        _context.Users.Add(new UserDBO { Id = id, Username = "test" });
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal("test", result.Username);
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
        _context.Users.Add(new UserDBO { Id = Guid.NewGuid(), Email = "test@example.com" });
        await _context.SaveChangesAsync();

        var result = await _repository.GetByEmailAsync("test@example.com");

        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
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
        _context.Users.Add(new UserDBO { Id = Guid.NewGuid(), Username = "uniqueuser" });
        await _context.SaveChangesAsync();

        var result = await _repository.GetByUserAsync("uniqueuser");

        Assert.NotNull(result);
        Assert.Equal("uniqueuser", result.Username);
    }

    [Fact]
    public async Task GetByUserAsync_ReturnsNull_WhenNotExists()
    {
        var result = await _repository.GetByUserAsync("notfound");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllUsers_OrderedById()
    {
        var id1 = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var id2 = Guid.Parse("00000000-0000-0000-0000-000000000002");
        _context.Users.Add(new UserDBO { Id = id2, Username = "user2" });
        _context.Users.Add(new UserDBO { Id = id1, Username = "user1" });
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
        var user = new User(Guid.NewGuid(), "newuser", "First", "Last", "new@example.com", 
            new List<string> { "Fiction" }, DateTime.UtcNow);

        await _repository.AddAsync(user);

        var result = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        Assert.NotNull(result);
        Assert.Equal("newuser", result.Username);
    }

    [Fact]
    public async Task AddAsync_SetsCreatedAtTimestamp()
    {
        var before = DateTime.UtcNow;
        var user = new User(Guid.NewGuid(), "user", "F", "L", "e@e.com", new List<string>(), DateTime.UtcNow);

        await _repository.AddAsync(user);
        var after = DateTime.UtcNow;

        var result = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        Assert.True(result!.createdAt >= before && result.createdAt <= after);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesUser_WhenExists()
    {
        var id = Guid.NewGuid();
        _context.Users.Add(new UserDBO { Id = id, Username = "original", Email = "orig@test.com" });
        await _context.SaveChangesAsync();

        var updated = new User(id, "updated", "First", "Last", "updated@test.com", 
            new List<string> { "Mystery" }, DateTime.UtcNow);
        await _repository.UpdateAsync(updated);

        var result = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        Assert.Equal("updated", result!.Username);
        Assert.Equal("updated@test.com", result.Email);
        Assert.Equal("Mystery", result.PreferredGenres);
    }

    [Fact]
    public async Task UpdateAsync_DoesNothing_WhenNotExists()
    {
        var user = new User(Guid.NewGuid(), "notexists", "F", "L", "e@e.com", new List<string>(), DateTime.UtcNow);

        await _repository.UpdateAsync(user); // Should not throw

        Assert.Empty(await _context.Users.ToListAsync());
    }

    [Fact]
    public async Task UpdateAsync_HandlesNullPreferredGenres()
    {
        var id = Guid.NewGuid();
        _context.Users.Add(new UserDBO { Id = id, Username = "user", PreferredGenres = "Fiction" });
        await _context.SaveChangesAsync();

        var updated = new User(id, "user", "F", "L", "e@e.com", null!, DateTime.UtcNow);
        await _repository.UpdateAsync(updated);

        var result = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        Assert.Equal(string.Empty, result!.PreferredGenres);
    }

    [Fact]
    public async Task DeleteAsync_RemovesUser_WhenExists()
    {
        var id = Guid.NewGuid();
        _context.Users.Add(new UserDBO { Id = id, Username = "todelete" });
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