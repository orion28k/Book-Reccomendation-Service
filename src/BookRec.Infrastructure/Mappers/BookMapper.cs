using BookRec.Domain.BookModel;
using BookRec.Infrastructure.Dbos;

namespace BookRec.Infrastructure.Mappers;

public static class BookMapper
{
    public static Book ToDomain(BookDBO bookDBO)
    {
        return new Book(
            bookDBO.Id,
            bookDBO.Title,
            bookDBO.Author,
            bookDBO.Description,
            string.IsNullOrEmpty(bookDBO.Genre) 
                ? new List<string>() 
                : bookDBO.Genre.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
            bookDBO.PublishDate
        );
    }
    
    public static BookDBO ToDBO(Book book)
    {
        return new BookDBO
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Description = book.Description,
            Genre = string.Join(",", book.Genres),
            PublishDate = book.PublishDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static List<Book> ToDomainList(IEnumerable<BookDBO> dbos)
    {
        return dbos.Select(ToDomain).ToList();
    }
}