using BookRec.Infrastructure.Dbos;
using BookRec.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookRec.Infrastructure.Tests.Persistence;

public class AppDbContextTests : IDisposable
{
    private readonly AppDbContext _context;

    public AppDbContextTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public void Books_DbSet_ShouldNotBeNull()
    {
        Assert.NotNull(_context.Books);
    }

    [Fact]
    public void Users_DbSet_ShouldNotBeNull()
    {
        Assert.NotNull(_context.Users);
    }

    [Fact]
    public async Task CanAddAndRetrieveBook()
    {
        var book = new BookDBO
        {
            Id = Guid.NewGuid(),
            Title = "Test Book",
            Author = "Author",
            Description = "Desc",
            Genre = "Fiction",
            PublishDate = DateTime.UtcNow
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        var result = await _context.Books.FirstOrDefaultAsync(b => b.Id == book.Id);

        Assert.NotNull(result);
        Assert.Equal("Test Book", result.Title);
    }

    [Fact]
    public async Task CanAddAndRetrieveUser()
    {
        var user = new UserDBO
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            PreferredGenres = "Fiction"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var result = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);

        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
    }

    [Fact]
    public async Task CanUpdateBook()
    {
        var book = new BookDBO { Id = Guid.NewGuid(), Title = "Original" };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        book.Title = "Updated";
        await _context.SaveChangesAsync();

        var result = await _context.Books.FirstOrDefaultAsync(b => b.Id == book.Id);
        Assert.Equal("Updated", result!.Title);
    }

    [Fact]
    public async Task CanDeleteBook()
    {
        var book = new BookDBO { Id = Guid.NewGuid(), Title = "ToDelete" };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();

        var result = await _context.Books.FirstOrDefaultAsync(b => b.Id == book.Id);
        Assert.Null(result);
    }
}