
using System.Data.Common;
using BookRec.Domain.BookModel;
using BookRec.Domain.UserModel;
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
            bookDBO.Genre,
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
            Genre = book.Genre,
            PublishDate = book.PublishDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}