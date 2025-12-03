using BookRec.Application.Books.Dtos;

namespace BookRec.Application.Books.Interface;

/// <summary>
/// This Book Service Interface implements but not defines the functions of the Application Layer.
/// </summary>
public interface IBookService
{
    Task<BookDto?> GetByIdAsync(Guid id);
    Task<BookDto?> GetByTitleAsync(string title);
    Task<IReadOnlyList<BookDto>> GetByAuthor(string author);
    Task<IReadOnlyList<BookDto>> GetByGenre(string genre);
    Task<IReadOnlyList<BookDto>> GetAllAsync();
    Task<Guid> AddBook(CreateBookDto book);
    Task<Guid> UpdateBook(CreateBookDto book, Guid id);
    Task DeleteBook(Guid id);
}