using BookRec.Domain.BookModel;

namespace BookRec.Domain.BookModel;

/// <summary>
/// This Repository Interface defines the CRUD operations that will be implemented in the Infrastructure layer.
/// </summary>
public interface IBookRepository
{
    Task<Book?> GetByIdAsync(Guid id);
    Task<Book?> GetByTitleAsync(string title);
    Task<IReadOnlyList<Book?>> GetByAuthor(string author);
    Task<IReadOnlyList<Book>> GetAllAsync();
    Task AddBook(Book book);
    Task DeleteBook(Book book);
}