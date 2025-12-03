using BookRec.Domain.BookModel;
using BookRec.Infrastructure.Mappers;
using BookRec.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookRec.Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly AppDbContext _db;

    public BookRepository(AppDbContext db) => _db = db;

    public async Task<Book?> GetByIdAsync(Guid id)
    {
        var dbo = await _db.Books.FirstOrDefaultAsync(b => b.Id == id);
        return dbo is null ? null : BookMapper.ToDomain(dbo);
    }

    public async Task<Book?> GetByTitleAsync(string title)
    {
        var dbo = await _db.Books.FirstOrDefaultAsync(b => b.Title == title);
        return dbo is null ? null : BookMapper.ToDomain(dbo);
    }

    public async Task<List<Book>> GetByAuthorAsync(string author)
    {
        var dbos = await _db.Books.Where(b => b.Author == author).ToListAsync();
        return BookMapper.ToDomainList(dbos);
    }

    public async Task<List<Book>> GetByGenreAsync(string genre)
    {
        var dbos = await _db.Books.Where(b => b.Genre.Contains(genre)).ToListAsync();
        return BookMapper.ToDomainList(dbos);
    }

    public async Task<List<Book>> GetAllAsync()
    {
        var dbos = await _db.Books.OrderBy(b => b.Title).ToListAsync();
        return BookMapper.ToDomainList(dbos);
    }

    public async Task AddAsync(Book book)
    {
        var dbo = BookMapper.ToDBO(book);
        dbo.CreatedAt = DateTime.UtcNow;
        _db.Books.Add(dbo);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Book book)
    {
        var dbBook = await _db.Books.FirstOrDefaultAsync(b => b.Id == book.Id);
        if (dbBook is null) return;

        dbBook.Title = book.Title;
        dbBook.Author = book.Author;
        dbBook.Genre = string.Join(",", book.Genres);
        dbBook.Description = book.Description;
        dbBook.PublishDate = book.PublishDate;
        dbBook.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var dbo = await _db.Books.FirstOrDefaultAsync(b => b.Id == id);
        if (dbo is null) return;

        _db.Books.Remove(dbo);
        await _db.SaveChangesAsync();
    }
}