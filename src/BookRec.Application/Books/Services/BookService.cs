using BookRec.Application.Books.Dtos;
using BookRec.Application.Books.Interface;
using BookRec.Domain.BookModel;

namespace BookRec.Application.Books.Services;

/// <summary>
/// This Book Service defines the implemented functions from the Book Service Interface
/// </summary>
public sealed class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }
    
    public async Task<BookDto?> GetByIdAsync(Guid id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        return book is null ? null : MapToDto(book);
    }

    public async Task<BookDto?> GetByTitleAsync(string title)
    {
        var book = await _bookRepository.GetByTitleAsync(title);
        return book is null ? null : MapToDto(book);
    }

    public async Task<IReadOnlyList<BookDto>> GetByAuthor(string author)
    {
       var books = await _bookRepository.GetByAuthorAsync(author) ?? new List<Book>();
       return books.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<BookDto>> GetAllAsync()
    {
        var books = await _bookRepository.GetAllAsync() ?? new List<Book>();
        return books.Select(MapToDto).ToList();
    }

    public async Task<Guid> AddBook(CreateBookDto createBookDto)
    {
        var id = Guid.NewGuid();

        var book = new Book(
            id,
            createBookDto.Title,
            createBookDto.Author,
            createBookDto.Description,
            createBookDto.Genre,
            createBookDto.PublishDate
        );

        await _bookRepository.AddAsync(book);
        return id;
    }

    public async Task DeleteBook(Guid id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book is null)
        {
            return;
        }
        await _bookRepository.DeleteAsync(id);
    }

    private static BookDto MapToDto(Book book) => new BookDto(
        book.Id,
        book.Title,
        book.Author,
        book.Description,
        book.Genre,
        book.PublishDate
    );
}