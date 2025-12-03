using BookRec.Infrastructure.Dbos;
using Xunit;

namespace BookRec.Infrastructure.Tests.Dbos;

public class BookDBOTests
{
    [Fact]
    public void BookDBO_ShouldInitializeWithDefaultValues()
    {
        var book = new BookDBO();

        Assert.Equal(Guid.Empty, book.Id);
        Assert.Equal(string.Empty, book.Title);
        Assert.Equal(string.Empty, book.Author);
        Assert.Equal(string.Empty, book.Description);
        Assert.Equal(string.Empty, book.Genre);
    }

    [Fact]
    public void BookDBO_ShouldSetAndGetProperties()
    {
        var id = Guid.NewGuid();
        var publishDate = new DateTime(2023, 1, 15, 0, 0, 0, DateTimeKind.Utc);

        var book = new BookDBO
        {
            Id = id,
            Title = "Test Title",
            Author = "Test Author",
            Description = "Test Description",
            Genre = "Fiction",
            PublishDate = publishDate
        };

        Assert.Equal(id, book.Id);
        Assert.Equal("Test Title", book.Title);
        Assert.Equal("Test Author", book.Author);
        Assert.Equal("Test Description", book.Description);
        Assert.Equal("Fiction", book.Genre);
        Assert.Equal(publishDate, book.PublishDate);
    }

    [Fact]
    public void BookDBO_CreatedAt_ShouldDefaultToUtcNow()
    {
        var before = DateTime.UtcNow;
        var book = new BookDBO();
        var after = DateTime.UtcNow;

        Assert.True(book.CreatedAt >= before);
        Assert.True(book.CreatedAt <= after);
    }

    [Fact]
    public void BookDBO_UpdatedAt_ShouldDefaultToUtcNow()
    {
        var before = DateTime.UtcNow;
        var book = new BookDBO();
        var after = DateTime.UtcNow;

        Assert.True(book.UpdatedAt >= before);
        Assert.True(book.UpdatedAt <= after);
    }
}