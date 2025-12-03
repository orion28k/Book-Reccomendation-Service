using BookRec.Domain.BookModel;
using BookRec.Infrastructure.Dbos;
using BookRec.Infrastructure.Persistence;
using BookRec.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookRec.Infrastructure.Tests.Repositories;

public class BookRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly BookRepository _repository;

    public BookRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _repository = new BookRepository(_context);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task GetByIdAsync_ReturnsBook_WhenExists()
    {
        var id = Guid.NewGuid();
        _context.Books.Add(new BookDBO { Id = id, Title = "Test", Author = "A", Description = "D", Genre = "Fiction" });
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal("Test", result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByTitleAsync_ReturnsBook_WhenExists()
    {
        _context.Books.Add(new BookDBO { Id = Guid.NewGuid(), Title = "UniqueTitle", Author = "A", Description = "D", Genre = "Fiction" });
        await _context.SaveChangesAsync();

        var result = await _repository.GetByTitleAsync("UniqueTitle");

        Assert.NotNull(result);
        Assert.Equal("UniqueTitle", result.Title);
    }

    [Fact]
    public async Task GetByTitleAsync_ReturnsNull_WhenNotExists()
    {
        var result = await _repository.GetByTitleAsync("NonExistent");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByAuthorAsync_ReturnsBooks_WhenExists()
    {
        _context.Books.Add(new BookDBO { Id = Guid.NewGuid(), Author = "Author1", Title = "Book1", Description = "D", Genre = "Fiction" });
        _context.Books.Add(new BookDBO { Id = Guid.NewGuid(), Author = "Author1", Title = "Book2", Description = "D", Genre = "Fiction" });
        _context.Books.Add(new BookDBO { Id = Guid.NewGuid(), Author = "Author2", Title = "Book3", Description = "D", Genre = "Fiction" });
        await _context.SaveChangesAsync();

        var result = await _repository.GetByAuthorAsync("Author1");

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetByAuthorAsync_ReturnsEmpty_WhenNotExists()
    {
        var result = await _repository.GetByAuthorAsync("Unknown");

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByGenreAsync_ReturnsBooks_WhenGenreMatches()
    {
        _context.Books.Add(new BookDBO { Id = Guid.NewGuid(), Genre = "Fiction", Title = "Book1", Author = "A", Description = "D" });
        _context.Books.Add(new BookDBO { Id = Guid.NewGuid(), Genre = "Mystery", Title = "Book2", Author = "A", Description = "D" });
        _context.Books.Add(new BookDBO { Id = Guid.NewGuid(), Genre = "Romance", Title = "Book3", Author = "A", Description = "D" });
        await _context.SaveChangesAsync();

        var result = await _repository.GetByGenreAsync("Fiction");

        Assert.Single(result);
        Assert.Equal("Book1", result[0].Title);
    }

    [Fact]
    public async Task GetByGenreAsync_ReturnsBooks_WhenGenreIsInList()
    {
        _context.Books.Add(new BookDBO { Id = Guid.NewGuid(), Genre = "Fiction,Mystery", Title = "Book1", Author = "A", Description = "D" });
        _context.Books.Add(new BookDBO { Id = Guid.NewGuid(), Genre = "Romance", Title = "Book2", Author = "A", Description = "D" });
        await _context.SaveChangesAsync();

        var result = await _repository.GetByGenreAsync("Mystery");

        Assert.Single(result);
        Assert.Equal("Book1", result[0].Title);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllBooks_OrderedByTitle()
    {
        _context.Books.Add(new BookDBO { Id = Guid.NewGuid(), Title = "Zebra", Author = "A", Description = "D", Genre = "Fiction" });
        _context.Books.Add(new BookDBO { Id = Guid.NewGuid(), Title = "Apple", Author = "A", Description = "D", Genre = "Fiction" });
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("Apple", result[0].Title);
        Assert.Equal("Zebra", result[1].Title);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty_WhenNoBooks()
    {
        var result = await _repository.GetAllAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task AddAsync_AddsBook()
    {
        var book = new Book(Guid.NewGuid(), "New Book", "Author", "Description", 
            new List<string> { "Fiction" }, DateTime.UtcNow);

        await _repository.AddAsync(book);

        var result = await _context.Books.FirstOrDefaultAsync(b => b.Id == book.Id);
        Assert.NotNull(result);
        Assert.Equal("New Book", result.Title);
        Assert.Equal("Fiction", result.Genre);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesBook_WhenExists()
    {
        var id = Guid.NewGuid();
        _context.Books.Add(new BookDBO { Id = id, Title = "Original", Author = "A", Description = "D", Genre = "Fiction" });
        await _context.SaveChangesAsync();

        var updated = new Book(id, "Updated", "Author", "Description", 
            new List<string> { "Mystery", "Thriller" }, DateTime.UtcNow);
        await _repository.UpdateAsync(updated);

        var result = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
        Assert.Equal("Updated", result!.Title);
        Assert.Equal("Mystery,Thriller", result.Genre);
    }

    [Fact]
    public async Task UpdateAsync_DoesNothing_WhenNotExists()
    {
        var book = new Book(Guid.NewGuid(), "NotExists", "Author", "Description", 
            new List<string> { "Fiction" }, DateTime.UtcNow);

        await _repository.UpdateAsync(book); // Should not throw

        Assert.Empty(await _context.Books.ToListAsync());
    }

    [Fact]
    public async Task DeleteAsync_RemovesBook_WhenExists()
    {
        var id = Guid.NewGuid();
        _context.Books.Add(new BookDBO { Id = id, Title = "ToDelete", Author = "A", Description = "D", Genre = "Fiction" });
        await _context.SaveChangesAsync();

        await _repository.DeleteAsync(id);

        Assert.Null(await _context.Books.FirstOrDefaultAsync(b => b.Id == id));
    }

    [Fact]
    public async Task DeleteAsync_DoesNothing_WhenNotExists()
    {
        await _repository.DeleteAsync(Guid.NewGuid()); // Should not throw

        Assert.Empty(await _context.Books.ToListAsync());
    }
}