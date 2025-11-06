using System.Runtime.InteropServices.Marshalling;

namespace BookRec.Domain.BookModel;

/// <summary>
/// Represents a book model entity with 
/// </summary>
/// 
public class Book : Entity
{
    private Guid Id { get; set; } = Guid.NewGuid;
    private string Title { get; set; }
    private string Author { get; set; }
    private string Description { get; set; }
    private DateTime PublishDate { get; set; } 

    public Book(string title, string author, string description, DateTime publishDate )
    {
        Title = title;
        Author = author;
        Description = description;
        PublishDate = publishDate;
    }
}