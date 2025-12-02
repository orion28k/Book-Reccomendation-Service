using BookRec.Domain.BookModel;

namespace BookRec.Domain.BookModel;

/// <summary>
/// This Repository Interface defines the CRUD operations that will be implemented in the Infrastructure layer.
/// </summary>
public interface IBookRepository
{
    Task<Book?> GetByIdAsync(Guid id);
    Task<Book?> GetByTitleAsync(string title);
    Task<List<Book>> GetByAuthorAsync(string author);
    Task<List<Book>> GetByGenreAsync(string genre);
    Task<List<Book>> GetAllAsync();
    Task AddAsync(Book book);
    Task UpdateAsync(Book book);
    Task DeleteAsync(Guid id);
}