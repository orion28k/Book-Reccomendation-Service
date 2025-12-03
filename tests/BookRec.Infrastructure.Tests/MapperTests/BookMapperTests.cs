using BookRec.Domain.BookModel;
using BookRec.Infrastructure.Dbos;
using BookRec.Infrastructure.Mappers;
using Xunit;

namespace BookRec.Infrastructure.Tests.Mappers;

public class BookMapperTests
{
    [Fact]
    public void ToDomain_ShouldMapAllProperties()
    {
        var id = Guid.NewGuid();
        var publishDate = new DateTime(2023, 6, 15, 0, 0, 0, DateTimeKind.Utc);
        var dbo = new BookDBO
        {
            Id = id,
            Title = "Test Book",
            Author = "Test Author",
            Description = "Test Description",
            Genre = "Fiction",
            PublishDate = publishDate
        };

        var result = BookMapper.ToDomain(dbo);

        Assert.Equal(id, result.Id);
        Assert.Equal("Test Book", result.Title);
        Assert.Equal("Test Author", result.Author);
        Assert.Equal("Test Description", result.Description);
        Assert.Contains("Fiction", result.Genres);
        Assert.Equal(publishDate, result.PublishDate);
    }

    [Fact]
    public void ToDBO_ShouldMapAllProperties()
    {
        var id = Guid.NewGuid();
        var publishDate = new DateTime(2023, 6, 15, 0, 0, 0, DateTimeKind.Utc);
        var book = new Book(id, "Test Book", "Test Author", "Test Description", new[] { "Fiction" }, publishDate);

        var result = BookMapper.ToDBO(book);

        Assert.Equal(id, result.Id);
        Assert.Equal("Test Book", result.Title);
        Assert.Equal("Test Author", result.Author);
        Assert.Equal("Test Description", result.Description);
        Assert.Equal("Fiction", result.Genre);
        Assert.Equal(publishDate, result.PublishDate);
    }

    [Fact]
    public void ToDBO_ShouldSetTimestamps()
    {
        var before = DateTime.UtcNow;
        var book = new Book(Guid.NewGuid(), "Title", "Author", "Desc", new[] { "Genre" }, DateTime.UtcNow);

        var result = BookMapper.ToDBO(book);
        var after = DateTime.UtcNow;

        Assert.True(result.CreatedAt >= before && result.CreatedAt <= after);
        Assert.True(result.UpdatedAt >= before && result.UpdatedAt <= after);
    }

    [Fact]
    public void ToDomainList_ShouldMapAllItems()
    {
        var dbos = new List<BookDBO>
        {
            new BookDBO { Id = Guid.NewGuid(), Title = "Book 1" },
            new BookDBO { Id = Guid.NewGuid(), Title = "Book 2" },
            new BookDBO { Id = Guid.NewGuid(), Title = "Book 3" }
        };

        var result = BookMapper.ToDomainList(dbos);

        Assert.Equal(3, result.Count);
        Assert.Equal("Book 1", result[0].Title);
        Assert.Equal("Book 2", result[1].Title);
        Assert.Equal("Book 3", result[2].Title);
    }

    [Fact]
    public void ToDomainList_ShouldReturnEmptyList_WhenEmpty()
    {
        var dbos = new List<BookDBO>();

        var result = BookMapper.ToDomainList(dbos);

        Assert.Empty(result);
    }
}