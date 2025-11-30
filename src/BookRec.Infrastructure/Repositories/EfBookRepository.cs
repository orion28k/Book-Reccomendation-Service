using Microsoft.EntityFrameworkCore;
using BookRec.Infrastructure.Data;

namespace BookRec.Infrastructure.Repositories;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(Guid id);
    Task<Book?> GetByTitleAsync(string title);
    Task<IReadOnlyList<Book?>> GetByAuthor(string author);
    Task<IReadOnlyList<Book>> GetAllAsync();
    Task AddBook(Book book);
    Task UpdateBook(Book book);
    Task DeleteBook(Book book);
}

public class EfBookRepository : IBookRepository
{
    private readonly BookRecDbContext _db;
    public EfBookRepository(BookRecDbContext db) => _db = db;

    public Task<Book?> GetByIdAsync(Guid id) =>
        _db.Books.FirstOrDefaultAsync(b => b.Id == id)!;

    public Task<Book?> GetByTitleAsync(string title) =>
        _db.Books.FirstOrDefaultAsync(b => b.Title == title)!;

    public async Task<IReadOnlyList<Book?>> GetByAuthor(string author) =>
        await _db.Books.Where(b => b.Author == author).Cast<Book?>().ToListAsync();

    public async Task<IReadOnlyList<Book>> GetAllAsync() =>
        await _db.Books.OrderBy(b => b.Title).ToListAsync();

    public async Task AddBook(Book book)
    {
        _db.Books.Add(book);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateBook(Book book)
    {
        _db.Books.Update(book);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteBook(Book book)
    {
        _db.Books.Remove(book);
        await _db.SaveChangesAsync();
    }
}
