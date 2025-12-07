using BookRec.Domain.BookModel;

namespace BookRec.Domain.Tests;

public class BookTests
{
    private Book CreateValidBook() => new(
        Guid.NewGuid(),
        "Test Book",
        "Test Author",
        "Test Description",
        new List<string> { "Fiction" },
        DateTime.UtcNow
    );

    [Fact]
    public void Constructor_ValidData_CreatesBook()
    {
        var book = CreateValidBook();

        Assert.Equal("Test Book", book.Title);
        Assert.Equal("Test Author", book.Author);
        Assert.Equal("Test Description", book.Description);
        Assert.Single(book.Genres);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_InvalidTitle_Throws(string? title)
    {
        Assert.Throws<ArgumentException>(() =>
            new Book(Guid.NewGuid(), title!, "Author", "Description", new List<string> { "Fiction" }, DateTime.UtcNow));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_InvalidAuthor_Throws(string? author)
    {
        Assert.Throws<ArgumentException>(() =>
            new Book(Guid.NewGuid(), "Title", author!, "Description", new List<string> { "Fiction" }, DateTime.UtcNow));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_InvalidDescription_Throws(string? description)
    {
        Assert.Throws<ArgumentException>(() =>
            new Book(Guid.NewGuid(), "Title", "Author", description!, new List<string> { "Fiction" }, DateTime.UtcNow));
    }

    [Fact]
    public void Constructor_DescriptionTooLong_Throws()
    {
        var longDescription = new string('a', 801);

        Assert.Throws<ArgumentException>(() =>
            new Book(Guid.NewGuid(), "Title", "Author", longDescription, new List<string> { "Fiction" }, DateTime.UtcNow));
    }

    [Fact]
    public void Constructor_NullGenres_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new Book(Guid.NewGuid(), "Title", "Author", "Description", null!, DateTime.UtcNow));
    }

    [Fact]
    public void Constructor_EmptyGenres_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new Book(Guid.NewGuid(), "Title", "Author", "Description", new List<string>(), DateTime.UtcNow));
    }

    [Fact]
    public void AddGenre_ValidGenre_AddsToList()
    {
        var book = CreateValidBook();

        book.addGenre("Mystery");

        Assert.Equal(2, book.Genres.Count);
        Assert.Contains("Mystery", book.Genres);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AddGenre_InvalidGenre_Throws(string? genre)
    {
        var book = CreateValidBook();

        Assert.Throws<ArgumentException>(() => book.addGenre(genre!));
    }

    [Fact]
    public void AddGenre_DuplicateGenre_Throws()
    {
        var book = CreateValidBook();

        Assert.Throws<ArgumentException>(() => book.addGenre("Fiction"));
    }

    [Fact]
    public void AddGenre_DuplicateDifferentCase_Throws()
    {
        var book = CreateValidBook();

        Assert.Throws<ArgumentException>(() => book.addGenre("FICTION"));
    }

    [Fact]
    public void UpdateGenres_ValidGenres_ReplacesAll()
    {
        var book = CreateValidBook();

        book.updateGenres(new List<string> { "Horror", "Thriller" });

        Assert.Equal(2, book.Genres.Count);
        Assert.DoesNotContain("Fiction", book.Genres);
    }

    [Fact]
    public void UpdateGenres_NullOrEmpty_Throws()
    {
        var book = CreateValidBook();

        Assert.Throws<ArgumentException>(() => book.updateGenres(null!));
        Assert.Throws<ArgumentException>(() => book.updateGenres(new List<string>()));
    }
}