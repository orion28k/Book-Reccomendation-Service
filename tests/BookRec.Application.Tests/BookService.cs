using Moq;
using BookRec.Application.Books.Dtos;
using BookRec.Application.Books.Services;
using BookRec.Domain.BookModel;

namespace BookRec.Application.Tests;

public class BookServiceTests
{
    private readonly Mock<IBookRepository> _mockRepo;
    private readonly BookService _service;

    public BookServiceTests()
    {
        _mockRepo = new Mock<IBookRepository>();
        _service = new BookService(_mockRepo.Object);
    }

    private Book CreateValidBook(Guid? id = null) => new(
        id ?? Guid.NewGuid(),
        "Test Book",
        "Test Author",
        "Test Description",
        new List<string> { "Fiction" },
        DateTime.UtcNow
    );

    [Fact]
    public async Task GetByIdAsync_BookExists_ReturnsBookDto()
    {
        var book = CreateValidBook();
        _mockRepo.Setup(r => r.GetByIdAsync(book.Id)).ReturnsAsync(book);

        var result = await _service.GetByIdAsync(book.Id);

        Assert.NotNull(result);
        Assert.Equal(book.Id, result.Id);
        Assert.Equal("Test Book", result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_BookNotFound_ReturnsNull()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Book?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByTitleAsync_BookExists_ReturnsBookDto()
    {
        var book = CreateValidBook();
        _mockRepo.Setup(r => r.GetByTitleAsync("Test Book")).ReturnsAsync(book);

        var result = await _service.GetByTitleAsync("Test Book");

        Assert.NotNull(result);
        Assert.Equal("Test Book", result.Title);
    }

    [Fact]
    public async Task GetByTitleAsync_BookNotFound_ReturnsNull()
    {
        _mockRepo.Setup(r => r.GetByTitleAsync(It.IsAny<string>())).ReturnsAsync((Book?)null);

        var result = await _service.GetByTitleAsync("Nonexistent");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByAuthor_BooksExist_ReturnsBookDtos()
    {
        var books = new List<Book> { CreateValidBook(), CreateValidBook() };
        _mockRepo.Setup(r => r.GetByAuthorAsync("Test Author")).ReturnsAsync(books);

        var result = await _service.GetByAuthorAsync("Test Author");

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetByAuthor_NoBooksFound_ReturnsEmptyList()
    {
        _mockRepo.Setup(r => r.GetByAuthorAsync(It.IsAny<string>())).ReturnsAsync(new List<Book>());

        var result = await _service.GetByAuthorAsync("Unknown Author");

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByGenre_BooksExist_ReturnsBookDtos()
    {
        var books = new List<Book> { CreateValidBook(), CreateValidBook() };
        _mockRepo.Setup(r => r.GetByGenreAsync("Fiction")).ReturnsAsync(books);

        var result = await _service.GetByGenreAsync("Fiction");

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetByGenre_NoBooksFound_ReturnsEmptyList()
    {
        _mockRepo.Setup(r => r.GetByGenreAsync(It.IsAny<string>())).ReturnsAsync(new List<Book>());

        var result = await _service.GetByGenreAsync("Unknown Genre");

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_BooksExist_ReturnsAllBookDtos()
    {
        var books = new List<Book> { CreateValidBook(), CreateValidBook(), CreateValidBook() };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(books);

        var result = await _service.GetAllAsync();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetAllAsync_NoBooksExist_ReturnsEmptyList()
    {
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Book>());

        var result = await _service.GetAllAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task AddBook_ValidDto_ReturnsNewGuid()
    {
        var dto = new CreateBookDto("Title", "Author", "Description", new List<string> { "Fiction" }, DateTime.UtcNow);
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Book>())).Returns(Task.CompletedTask);

        var result = await _service.AddBook(dto);

        Assert.NotEqual(Guid.Empty, result);
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Once);
    }

    [Fact]
    public async Task UpdateBook_BookExists_UpdatesAndReturnsId()
    {
        var book = CreateValidBook();
        var updateDto = new UpdateBookDto("New Title", null, null, null, null, DateTime.UtcNow);
        _mockRepo.Setup(r => r.GetByIdAsync(book.Id)).ReturnsAsync(book);
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Book>())).Returns(Task.CompletedTask);

        var result = await _service.UpdateBook(updateDto, book.Id);

        Assert.Equal(book.Id, result);
    }

    [Fact]
    public async Task UpdateBook_BookNotFound_ThrowsException()
    {
        var updateDto = new UpdateBookDto("New Title", null, null, null, null, DateTime.UtcNow);
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Book?)null);

        await Assert.ThrowsAsync<Exception>(() => _service.UpdateBook(updateDto, Guid.NewGuid()));
    }

    [Fact]
    public async Task DeleteBook_BookExists_CallsDeleteAsync()
    {
        var book = CreateValidBook();
        _mockRepo.Setup(r => r.GetByIdAsync(book.Id)).ReturnsAsync(book);
        _mockRepo.Setup(r => r.DeleteAsync(book.Id)).Returns(Task.CompletedTask);

        await _service.DeleteBook(book.Id);

        _mockRepo.Verify(r => r.DeleteAsync(book.Id), Times.Once);
    }

    [Fact]
    public async Task DeleteBook_BookNotFound_DoesNotCallDelete()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Book?)null);

        await _service.DeleteBook(Guid.NewGuid());

        _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }

}