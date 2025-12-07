using BookRec.Infrastructure.Dbos;
using BookRec.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookRec.Infrastructure.Tests.Persistence;

public class BookConfigurationTests : IDisposable
{
    private readonly AppDbContext _context;

    public BookConfigurationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public void BookDBO_ShouldHavePrimaryKey()
    {
        var entityType = _context.Model.FindEntityType(typeof(BookDBO));
        var primaryKey = entityType?.FindPrimaryKey();

        Assert.NotNull(primaryKey);
        Assert.Equal("Id", primaryKey.Properties[0].Name);
    }

    [Fact]
    public void BookDBO_Title_ShouldBeRequired()
    {
        var entityType = _context.Model.FindEntityType(typeof(BookDBO));
        var property = entityType?.FindProperty("Title");

        Assert.NotNull(property);
        Assert.False(property.IsNullable);
    }

    [Fact]
    public void BookDBO_Title_ShouldHaveMaxLength200()
    {
        var entityType = _context.Model.FindEntityType(typeof(BookDBO));
        var property = entityType?.FindProperty("Title");

        Assert.Equal(200, property?.GetMaxLength());
    }

    [Fact]
    public void BookDBO_Author_ShouldHaveMaxLength200()
    {
        var entityType = _context.Model.FindEntityType(typeof(BookDBO));
        var property = entityType?.FindProperty("Author");

        Assert.Equal(200, property?.GetMaxLength());
    }

    [Fact]
    public void BookDBO_Description_ShouldHaveMaxLength800()
    {
        var entityType = _context.Model.FindEntityType(typeof(BookDBO));
        var property = entityType?.FindProperty("Description");

        Assert.Equal(800, property?.GetMaxLength());
    }

    [Fact]
    public void BookDBO_ShouldHaveIndexOnTitle()
    {
        var entityType = _context.Model.FindEntityType(typeof(BookDBO));
        var indexes = entityType?.GetIndexes().ToList();

        Assert.Contains(indexes!, i => i.Properties.Any(p => p.Name == "Title"));
    }

    [Fact]
    public void BookDBO_ShouldHaveIndexOnAuthor()
    {
        var entityType = _context.Model.FindEntityType(typeof(BookDBO));
        var indexes = entityType?.GetIndexes().ToList();

        Assert.Contains(indexes!, i => i.Properties.Any(p => p.Name == "Author"));
    }

    [Fact]
    public void BookDBO_ShouldHaveIndexOnGenre()
    {
        var entityType = _context.Model.FindEntityType(typeof(BookDBO));
        var indexes = entityType?.GetIndexes().ToList();

        Assert.Contains(indexes!, i => i.Properties.Any(p => p.Name == "Genre"));
    }
}